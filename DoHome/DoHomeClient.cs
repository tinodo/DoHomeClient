namespace DoHome
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    public class DoHomeClient
    {
        private List<DoHomeUdpClient> commandResponseClients = new List<DoHomeUdpClient>();
        private List<DoHomeUdpClient> discoveryClients = new List<DoHomeUdpClient>();
        private List<Task> listenerTasks = new List<Task>();

        public event EventHandler<DeviceDiscoveredEventArgs> DeviceDiscovered;


        public delegate void DeviceDiscoveredEventHandler(object sender, DeviceDiscoveredEventArgs e);

        protected virtual void OnDeviceDiscovered(DoHomeDevice device)
        {
            var handler = this.DeviceDiscovered;
            var args = new DeviceDiscoveredEventArgs(device);
            handler?.Invoke(this, args);
        }

        /// <summary>
        /// UDP Broadcast port for device control.
        /// </summary>
        private const int mainUdpPort = 6091;

        /// <summary>
        /// The UDP port to listen for device broadcasts.
        /// </summary>
        /// <remarks>
        /// When a device is connected to a router, it actively sends a UDP Broadcast to port 6095, every two seconds, 10 times.
        /// The broadcast content is the same as the response to a cmd=ping command.
        /// </remarks>
        private const int autoDiscoverUdpPort = 6095;
        
        private DoHomeListenerState listenerState = DoHomeListenerState.Stopped;

        public DoHomeListenerState ListenerState
        {
            get
            {
                return this.listenerState;
            }
        }

        private List<DoHomeDevice> devices = new List<DoHomeDevice>();

        public DoHomeDevice[] Devices
        {
            get
            {
                return this.devices.ToArray();
            }
        }

        /// <summary>
        /// Creates a new instance of the DoHomeHelper class.
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
                var client = new DoHomeUdpClient(new IPEndPoint(ip, mainUdpPort));
                this.commandResponseClients.Add(client);

                var discoveryClient = new DoHomeUdpClient(new IPEndPoint(ip, autoDiscoverUdpPort));
                this.discoveryClients.Add(discoveryClient);
            }
        }

        /// <summary>
        /// Starts the UDP Listeners to listen for devices being powered on, and broadcasts from other devices.
        /// </summary>
        /// <param name="updateInterval">The interval to actively ask devices for updates. Set to 0 to disable, or to something above 500 to enable.</param>
        /// <param name="discoverInterval">The interval to actively search for new devices. Set to 0 to disable, or to something above 500 to enable.</param>
        /// <remarks>
        /// Starts 2 UDP listeners on all IPv4 network interfaces.
        /// If updateInterval is not 0, it broadcasts a message to all discovered devices for a status update.
        /// If discoverInterval is is not 0, it broadcasts a message on all network interfaces to discover new devices.
        /// </remarks>
        public void StartListener(int updateInterval = 0, int discoverInterval = 0)
        {
            if (updateInterval < 500 && updateInterval != 0) throw new ArgumentOutOfRangeException(nameof(updateInterval));
            if (discoverInterval < 500 && discoverInterval != 0) throw new ArgumentOutOfRangeException(nameof(discoverInterval));
            if (this.listenerState != DoHomeListenerState.Stopped) return;

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
                        if (Devices.Length > 0) this.Send("cmd=ctrl&devices={[" + string.Join(',', Devices.Select(d => d.UdpDeviceId)) + "]}&op={\"cmd\":25}");
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

        //public void TEST()
        //{
        //    Console.WriteLine(this.ListenerState.ToString());
        //    foreach (var task in this.listenerTasks)
        //    {
        //        Console.WriteLine($"Task {task.Id}: {task.Status}");
        //    }

        //    Console.WriteLine("");
        //}
        public void StopListener()
        {
            if (this.listenerState != DoHomeListenerState.Running && this.listenerState != DoHomeListenerState.Starting) return;
            this.listenerState = DoHomeListenerState.Stopping;
            this.Send("cmd=ping", true);
            Task.Run(() =>
            {
                Task.WaitAll(this.listenerTasks.ToArray());
                this.listenerState = DoHomeListenerState.Stopped;
            });
        }

        internal void Send(string command, params DoHomeDevice[] devices)
        {
            var message = "cmd=ctrl&devices={[" + string.Join(',', devices.Select(d => d.UdpDeviceId)) + "]}&op=" + command;
            this.Send(message);
        }

        private void Send(string message, bool alsoSendToAutoDiscoverClients = false)
        {
            var data = Encoding.ASCII.GetBytes(message);
            foreach (var client in this.commandResponseClients)
            {
                client.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, mainUdpPort));
            }

            if (alsoSendToAutoDiscoverClients)
            {
                foreach (var discoveryClient in this.discoveryClients)
                {
                    discoveryClient.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, autoDiscoverUdpPort));
                }
            }
        }

        public void On(params DoHomeDevice[] devices)
        {
            var deviceIds = string.Join(',', devices.Select(d => d.UdpDeviceId));
            this.Send("cmd=ctrl&devices={[" + deviceIds + "]}&op={\"cmd\":6,\"r\":0,\"g\":0,\"b\":0,\"w\":5000,\"m\":4000,\"on\":1}");
        }
        public void Off(params DoHomeDevice[] devices)
        {
            var deviceIds = string.Join(',', devices.Select(d => d.UdpDeviceId));
            this.Send("cmd=ctrl&devices={[" + deviceIds + "]}&op={\"cmd\":6,\"r\":0,\"g\":0,\"b\":0,\"w\":0,\"m\":0,\"on\":0}");
        }

        public void Test (string command, params DoHomeDevice[] devices)
        {
            var deviceIds = string.Join(',', devices.Select(d => d.UdpDeviceId));
            this.Send("cmd=ctrl&devices={[" + deviceIds + "]}&op=" + command + "\r\n");
        }
        public void Set(int red, int green, int blue, int white, int warmth, int on, int temp, int smooth, int t, params DoHomeDevice[] devices)
        {
            var deviceIds = string.Join(',', devices.Select(d => d.UdpDeviceId));
            //this.Send("cmd=ctrl&devices={[" + deviceIds + $"]}}&op={{\"cmd\":6,\"r\":{red},\"g\":{green},\"b\":{blue},\"w\":{white},\"m\":{warmth},\"on\":{on},\"temp\":{temp},\"smooth\":{smooth},\"t\":{t}}}");
            this.Send("cmd=ctrl&devices={[" + deviceIds + $"]}}&op={{\"cmd\":6,\"r\":{red},\"g\":{green},\"b\":{blue},\"w\":{white},\"smooth\":{smooth},\"t\":{t}}}");
        }

        protected virtual void OnDataReceived(UdpClient client, IPEndPoint endPoint, byte[] data)
        {
            if (client.Client.LocalEndPoint.ToString() == endPoint.ToString()) return;

            string message;
            try
            {
                message = Encoding.ASCII.GetString(data);
            }
            catch (ArgumentException)
            {
                return;
            }

            if (string.IsNullOrEmpty(message)) return;

            var parts = message.Split('&');
            if (parts.Length < 2) return;

            var response = new Dictionary<string, string>();
            foreach (var part in parts)
            {
                var p = part.Split('=');
                if (p.Length != 2) return;

                response.Add(p[0], p[1]);
            }

            try
            {
                switch (response["cmd"])
                {
                    case "ctrl":
                        // This is an echo of a command sent to a device.
                        // THIS SHOULD NOT BE SEEN, CHECKED AT THE BEGINNING!
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
                        if (device2 == null) return; // echo from unknown device. Perhaps discover it?
                        var op = JsonSerializer.Deserialize<JsonElement>(response["op"]);
                        Console.WriteLine($"Device {device2.UdpDeviceId} sent: {response["op"]}");
                        switch (op.GetProperty("cmd").GetInt32())
                        {
                            case 25:
                                var color = JsonSerializer.Deserialize<DoHomeColor>(response["op"]);
                                device2.UpdateColor(color);
                                break;
                            default:
                                Console.WriteLine($"RECEIVED [{endPoint}] ON [{client.Client.LocalEndPoint}]: {message}");
                                break;
                        }

                        break;
                    default:
                        Console.WriteLine($"Received unhandled message from {endPoint}: {message}");
                        break;
                }
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine($"Received invalid message from {endPoint}: {message}");
                return;
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine($"Received invalid message from {endPoint}: {message}");
                return;
            }
            catch (JsonException)
            {
                Console.WriteLine($"Received invalid message from {endPoint}: {message}");
                return;
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine($"Received invalid message from {endPoint}: {message}");
                return;
            }
        }
    }
}