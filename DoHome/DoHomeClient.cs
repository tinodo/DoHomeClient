//-----------------------------------------------------------------------
// <copyright file="DoHomeClient.cs" company="Company">
//    Copyright (c) Tino Donderwinkel. All rights reserved.
// </copyright>
// <author>Tino Donderwinkel</author>
//-----------------------------------------------------------------------

namespace DoHome
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The main client to discover and communicate with DoHome devices.
    /// </summary>
    public class DoHomeClient
    {
        /// <summary>
        /// UDP Broadcast port for device control.
        /// </summary>
        private const int MainUdpPort = 6091;

        /// <summary>
        /// The UDP port to listen for device broadcasts.
        /// </summary>
        /// <remarks>
        /// When a device is connected to a router, it actively sends a UDP Broadcast to port 6095, every two seconds, 10 times, whenever it is powered on.
        /// The broadcast content is the same as the response to a ping command.
        /// </remarks>
        private const int AutoDiscoverUdpPort = 6095;

        /// <summary>
        /// List of <see cref="DoHomeUdpClient"/> which listen for responses to UDP requests from the client to the device(s).
        /// </summary>
        private List<DoHomeUdpClient> commandResponseClients = new List<DoHomeUdpClient>();

        /// <summary>
        /// List of <see cref="DoHomeUdpClient"/> which listen for announcements from DoHome devices over UDP.
        /// </summary>
        private List<DoHomeUdpClient> discoveryClients = new List<DoHomeUdpClient>();

        /// <summary>
        /// A list of unmanaged tasks for the <see cref="DoHomeUdpClient"/> that listen for UDP broadcasts.
        /// </summary>
        private List<Task> listenerTasks = new List<Task>();
        
        /// <summary>
        /// List of discovered DoHome devices.
        /// </summary>
        private List<DoHomeDevice> devices = new List<DoHomeDevice>();

        /// <summary>
        /// The current state of the UDP listeners.
        /// </summary>
        private DoHomeListenerState listenerState = DoHomeListenerState.Stopped;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoHomeClient"/> class.
        /// </summary>
        /// <remarks>
        /// This is the main client class to communicate with your DoHome devices.
        /// </remarks>
        public DoHomeClient()
        {
            var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            var ips = hostEntry.AddressList.Where(a => a.AddressFamily == AddressFamily.InterNetwork);
            foreach (var ip in ips)
            {
                var client = new DoHomeUdpClient(new IPEndPoint(ip, MainUdpPort));
                this.commandResponseClients.Add(client);

                var discoveryClient = new DoHomeUdpClient(new IPEndPoint(ip, AutoDiscoverUdpPort));
                this.discoveryClients.Add(discoveryClient);
            }
        }

        /// <summary>
        /// The delegate for the <see cref="DeviceDiscovered"/> event handler.
        /// </summary>
        /// <param name="sender">The <see cref="DoHomeClient"/> initiating the event.</param>
        /// <param name="e">The <see cref="DeviceDiscoveredEventArgs"/> arguments.</param>
        public delegate void DeviceDiscoveredEventHandler(object sender, DeviceDiscoveredEventArgs e);

        /// <summary>
        /// Event handler that is triggered when a new DoHome device is discovered.
        /// </summary>
        public event EventHandler<DeviceDiscoveredEventArgs> DeviceDiscovered;

        /// <summary>
        /// Gets the current state of the UDP listeners.
        /// </summary>
        public DoHomeListenerState ListenerState
        {
            get
            {
                return this.listenerState;
            }
        }

        /// <summary>
        /// Gets the list of discovered DoHome devices.
        /// </summary>
        public DoHomeDevice[] Devices
        {
            get
            {
                return this.devices.ToArray();
            }
        }

        /// <summary>
        /// Broadcasts a message on all network interfaces to discover new devices.
        /// </summary>
        /// <remarks>
        /// Required the listeners to be started, so first call StartListener.
        /// If you call StartListener with a discoverInterval larger than 0, discovery should happen automatically.
        /// </remarks>
        public void DiscoverDevices()
        {
            this.Send("cmd=ping");
        }

        /// <summary>
        /// Stops the broadcast listeners.
        /// </summary>
        public void StopListener()
        {
            if (this.listenerState != DoHomeListenerState.Running && this.listenerState != DoHomeListenerState.Starting)
            {
                return;
            }

            this.listenerState = DoHomeListenerState.Stopping;
            this.Send("cmd=ping", true);
            Task.Run(() =>
            {
                Task.WaitAll(this.listenerTasks.ToArray());
                this.listenerState = DoHomeListenerState.Stopped;
            });
        }

        /// <summary>
        /// Turns one or more DoHome devices on.
        /// </summary>
        /// <param name="devices">The DoHome devices to turn on.</param>
        public void On(params DoHomeDevice[] devices)
        {
            var command = "{\"cmd\":6,\"r\":0,\"g\":0,\"b\":0,\"w\":5000,\"m\":4000,\"on\":1}";
            this.Send(command, devices);
        }

        /// <summary>
        /// Turns one or more DoHome devices off.
        /// </summary>
        /// <param name="devices">The DoHome devices to turn off.</param>
        public void Off(params DoHomeDevice[] devices)
        {
            var command = "{\"cmd\":6,\"r\":0,\"g\":0,\"b\":0,\"w\":0,\"m\":0,\"on\":0}";
            this.Send(command, devices);
        }

        /// <summary>
        /// Changes the color of a DoHome device.
        /// </summary>
        /// <param name="color">The new <see cref="DoHomeColor"/> of the device.</param>
        /// <param name="smoothing">True, to create a smooth transition between the previous color and the new color.</param>
        /// <param name="devices">The DoHome devices to change the color on.</param>
        public void ChangeColor(DoHomeColor color, bool smoothing, params DoHomeDevice[] devices)
        {
            var deviceIds = string.Join(',', devices.Select(d => d.UdpDeviceId));
            var smooth = smoothing ? 1 : 0;
            var command = $"{{\"cmd\":6,\"r\":{color.Red},\"g\":{color.Green},\"b\":{color.Blue},\"w\":{color.White},\"m\":{color.Warmth},\"smooth\":{smooth}}}";
            this.Send(command, devices);
        }

        /// <summary>
        /// Changes the color of a DoHome device.
        /// </summary>
        /// <param name="color">The new <see cref="DoHomeColor"/> of the device.</param>
        /// <param name="smoothingDuration">The duration (in 100 millisecond intervals) to smoothly transition from the previous color to the new color.</param>
        /// <param name="devices">The DoHome devices to change the color on.</param>
        public void ChangeColor(DoHomeColor color, int smoothingDuration, params DoHomeDevice[] devices)
        {
            var deviceIds = string.Join(',', devices.Select(d => d.UdpDeviceId));
            var smooth = 1;
            var command = $"{{\"cmd\":6,\"r\":{color.Red},\"g\":{color.Green},\"b\":{color.Blue},\"w\":{color.White},\"m\":{color.Warmth},\"smooth\":{smooth},\"t\":{smoothingDuration}}}";
            this.Send(command, devices);
        }

        /// <summary>
        /// Starts the UDP Listeners to listen for devices being powered on, and listen for broadcast responses.
        /// </summary>
        /// <param name="updateInterval">The interval to actively ask devices for updates. Set to 0 to disable, or to something above 500 to enable.</param>
        /// <param name="discoverInterval">The interval to actively search for new devices. Set to 0 to disable, or to something above 500 to enable.</param>
        /// <remarks>
        /// Starts 2 UDP listeners on all IP v4 network interfaces.
        /// If updateInterval is not 0, it broadcasts a message to all discovered devices for a status update.
        /// If discoverInterval is is not 0, it broadcasts a message on all network interfaces to discover new devices.
        /// </remarks>
        public void StartListener(int updateInterval = 0, int discoverInterval = 0)
        {
            if (updateInterval < 500 && updateInterval != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(updateInterval));
            }

            if (discoverInterval < 500 && discoverInterval != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(discoverInterval));
            }

            if (this.listenerState != DoHomeListenerState.Stopped)
            {
                return;
            }

            this.listenerState = DoHomeListenerState.Starting;

            var source = new IPEndPoint(0, 0);

            this.listenerTasks = new List<Task>();
            foreach (var client in this.commandResponseClients)
            {
                var task = Task.Run(() =>
                {
                    while (this.listenerState == DoHomeListenerState.Running || this.listenerState == DoHomeListenerState.Starting)
                    {
                        var bytes = client.Receive(ref source);
                        OnDataReceived(client, source, bytes);
                    }

                    return Task.CompletedTask;
                });
                this.listenerTasks.Add(task);
            }

            foreach (var client in this.discoveryClients)
            {
                var task = Task.Run(() =>
                {
                    while (this.listenerState == DoHomeListenerState.Running || this.listenerState == DoHomeListenerState.Starting)
                    {
                        var bytes = client.Receive(ref source);
                        OnDataReceived(client, source, bytes);
                    }

                    return Task.CompletedTask;
                });
                this.listenerTasks.Add(task);
            }

            if (updateInterval > 0)
            {
                var updateTask = Task.Run(() =>
                {
                    while (this.listenerState == DoHomeListenerState.Running || this.listenerState == DoHomeListenerState.Starting)
                    {
                        if (Devices.Length > 0)
                        {
                            this.Send("cmd=ctrl&devices={[" + string.Join(',', Devices.Select(d => d.UdpDeviceId)) + "]}&op={\"cmd\":25}");
                        }

                        Thread.Sleep(updateInterval);
                    }

                    return Task.CompletedTask;
                });

                this.listenerTasks.Add(updateTask);
            }

            if (discoverInterval > 0)
            {
                var discoveryTask = Task.Run(() =>
                {
                    while (this.listenerState == DoHomeListenerState.Running || this.listenerState == DoHomeListenerState.Starting)
                    {
                        this.Send("cmd=ping");
                        Thread.Sleep(discoverInterval);
                    }

                    return Task.CompletedTask;
                });

                this.listenerTasks.Add(discoveryTask);
            }

            this.listenerState = DoHomeListenerState.Running;

            this.DiscoverDevices();

            Thread.Sleep(250);
        }

        /// <summary>
        /// Sends a command to one or more DoHome devices over UDP.
        /// </summary>
        /// <param name="command">The command to send.</param>
        /// <param name="devices">The DoHome devices to send the command to.</param>
        internal void Send(string command, params DoHomeDevice[] devices)
        {
            if (devices == null || devices.Length == 0)
            {
                return;
            }

            var deviceIds = string.Join(',', devices.Select(d => d.UdpDeviceId));
            var message = $"cmd=ctrl&devices={{[{deviceIds}]}}&op={command}";
            this.Send(message);
        }

        /// <summary>
        /// The method used used to trigger the <see cref="DeviceDiscovered"/> event.
        /// </summary>
        /// <param name="device">The <see cref="DoHomeDevice"/> that was discovered.</param>
        protected virtual void OnDeviceDiscovered(DoHomeDevice device)
        {
            var handler = this.DeviceDiscovered;
            var args = new DeviceDiscoveredEventArgs(device);
            handler?.Invoke(this, args);
        }

        /// <summary>
        /// The method called when one of the <see cref="DoHomeUdpClient"/> listeners receives data.
        /// </summary>
        /// <param name="client">The <see cref="DoHomeUdpClient"/> that received data.</param>
        /// <param name="endPoint">The remote <see cref="IPEndPoint"/> that sent the data.</param>
        /// <param name="data">The data that was sent.</param>
        protected virtual void OnDataReceived(UdpClient client, IPEndPoint endPoint, byte[] data)
        {
            if (client.Client.LocalEndPoint.ToString() == endPoint.ToString())
            {
                return;
            }

            string message;
            try
            {
                message = Encoding.ASCII.GetString(data);
            }
            catch (ArgumentException)
            {
                return;
            }

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            var parts = message.Split('&');
            if (parts.Length < 2)
            {
                return;
            }

            var response = new Dictionary<string, string>();
            foreach (var part in parts)
            {
                var p = part.Split('=');
                if (p.Length != 2)
                {
                    return;
                }

                response.Add(p[0], p[1]);
            }

            try
            {
                switch (response["cmd"])
                {
                    case "ctrl":
                        // This is an echo of a command sent to a device.
                        break;
                    case "pong":
                        if (response["compandy_id"] == "_DOIT" && response["device_type"] == "_DT-WYRGB")
                        {
                            var tmp = response["device_id"].Split('_')[0];
                            var deviceId = tmp.Substring(tmp.Length - 4, 4);
                            var device = this.devices.SingleOrDefault(d => d.DeviceId == response["device_id"]);
                            if (device == null)
                            {
                                device = new DoHomeDevice(IPAddress.Parse(response["host_ip"]), IPAddress.Parse(response["sta_ip"]), response["device_id"], response["device_key"], response["device_name"], response["device_type"], response["compandy_id"], response["chip"].Trim(), this);
                                Console.WriteLine($"Discovered device {device.UdpDeviceId}");
                                this.devices.Add(device);
                                this.OnDeviceDiscovered(device);
                            }
                        }

                        break;
                    case "echo":
                        var device2 = this.devices.SingleOrDefault(d => d.DeviceId == response["dev"]);
                        if (device2 == null)
                        {
                            return; // echo from unknown device. Perhaps discover it?
                        }

                        var op = JsonSerializer.Deserialize<JsonElement>(response["op"]);
                        Debug.WriteLine($"Device {device2.UdpDeviceId} sent: {response["op"]}");
                        switch (op.GetProperty("cmd").GetInt32())
                        {
                            case 25:
                                var color = JsonSerializer.Deserialize<DoHomeColor>(response["op"]);
                                device2.UpdateColor(color);
                                break;
                            default:
                                Debug.WriteLine($"RECEIVED [{endPoint}] ON [{client.Client.LocalEndPoint}]: {message}");
                                break;
                        }

                        break;
                    default:
                        Debug.WriteLine($"Received unhandled message from {endPoint}: {message}");
                        break;
                }
            }
            catch (KeyNotFoundException)
            {
                Debug.WriteLine($"Received invalid message from {endPoint}: {message}");
                return;
            }
            catch (IndexOutOfRangeException)
            {
                Debug.WriteLine($"Received invalid message from {endPoint}: {message}");
                return;
            }
            catch (JsonException)
            {
                Debug.WriteLine($"Received invalid message from {endPoint}: {message}");
                return;
            }
            catch (ArgumentNullException)
            {
                Debug.WriteLine($"Received invalid message from {endPoint}: {message}");
                return;
            }
        }

        /// <summary>
        /// Sends a broadcast UDP message over the network.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="alsoSendToAutoDiscoverClients">True, to also send the message over the AutoDiscovery ports, false if not.</param>
        /// <remarks>
        /// Typically, DoHome devices do not listen for messages on the AutoDiscover point.
        /// alsoSendToAutoDiscoverClients is only used to stop the listeners in case the user requests that.
        /// Since a <see cref="UdpClient"/> blocks on read, without the possibility to use cancellation tokens, this is a
        /// viable workarounds to send a message to them such that they can be then stopped.
        /// </remarks>
        private void Send(string message, bool alsoSendToAutoDiscoverClients = false)
        {
            var data = Encoding.ASCII.GetBytes(message);
            foreach (var client in this.commandResponseClients)
            {
                client.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, MainUdpPort));
            }

            if (alsoSendToAutoDiscoverClients)
            {
                foreach (var discoveryClient in this.discoveryClients)
                {
                    discoveryClient.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, AutoDiscoverUdpPort));
                }
            }
        }
    }
}