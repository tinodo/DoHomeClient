//-----------------------------------------------------------------------
// <copyright file="DoHomeDevice.cs" company="Company">
//    Copyright (c) Tino Donderwinkel. All rights reserved.
// </copyright>
// <author>Tino Donderwinkel</author>
//-----------------------------------------------------------------------

namespace DoHome
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.Json;

    /// <summary>
    /// The DoHome Device class.
    /// </summary>
    /// <remarks>
    /// Devices should be discovered by a <see cref="DoHomeClient"/>.
    /// Calling methods on this device will be executed over TCP.
    /// To issue commands over UDP, use the methods available in the <see cref="DoHomeClient"/>.
    /// </remarks>
    public class DoHomeDevice
    {
        /// <summary>
        /// The <see cref="DoHomeClient"/> that discovered the device.
        /// </summary>
        private readonly DoHomeClient client;
        
        /// <summary>
        /// The <see cref="TcpClient"/> that maintains a TCP connection to the <see cref="DoHomeDevice"/>.
        /// </summary>
        private readonly TcpClient tcpClient;

        /// <summary>
        /// The identifier for the <see cref="DoHomeDevice"/> as used in UDP broadcast messages.
        /// </summary>
        private string udpDeviceId;

        /// <summary>
        /// The IP Address of the device on it's own WiFi network.
        /// </summary>
        private IPAddress hostIp;

        /// <summary>
        /// The IP Address of the device on the connected WiFi routers network.
        /// </summary>
        private IPAddress staIp;

        /// <summary>
        /// The device identifier.
        /// </summary>
        private string deviceId;

        /// <summary>
        /// The device name.
        /// </summary>
        private string deviceName;

        /// <summary>
        /// The device type.
        /// </summary>
        private string deviceType;

        /// <summary>
        /// The name of the device manufacturer.
        /// </summary>
        private string companyId;

        /// <summary>
        /// The device key.
        /// </summary>
        private string deviceKey;

        /// <summary>
        /// The chip used in the device.
        /// </summary>
        private string chip;

        /// <summary>
        /// The last known color of the device as discovered over UDP.
        /// </summary>
        private DoHomeColor color;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoHomeDevice"/> class.
        /// </summary>
        /// <param name="hostIp">The IP Address of the device on it's own WiFi network.</param>
        /// <param name="staIp">The IP Address of the device on the connected WiFi routers network.</param>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="deviceKey">The device key.</param>
        /// <param name="deviceName">The device name.</param>
        /// <param name="deviceType">The device type.</param>
        /// <param name="companyId">The name of the device manufacturer.</param>
        /// <param name="chip">The chip used in the device.</param>
        /// <param name="client">The <see cref="DoHomeClient"/> that discovered the device.</param>
        internal DoHomeDevice(IPAddress hostIp, IPAddress staIp, string deviceId, string deviceKey, string deviceName, string deviceType, string companyId, string chip, DoHomeClient client)
        {
            var tmp = deviceId.Split('_')[0];
            this.udpDeviceId = tmp.Substring(tmp.Length - 4, 4);
            this.hostIp = hostIp;
            this.staIp = staIp;
            this.deviceId = deviceId;
            this.deviceKey = deviceKey;
            this.deviceName = deviceName;
            this.deviceType = deviceType;
            this.companyId = companyId;
            this.chip = chip;
            this.client = client;
            this.tcpClient = new TcpClient(this.StaIp.ToString(), 5555);
        }

        /// <summary>
        /// Gets the identifier for the <see cref="DoHomeDevice"/> as used in UDP broadcast messages.
        /// </summary>
        public string UdpDeviceId
        {
            get
            {
                return this.udpDeviceId;
            }
        }

        /// <summary>
        /// Gets the IP Address of the device on it's own WiFi network.
        /// </summary>
        public IPAddress HostIp
        {
            get
            {
                return this.hostIp;
            }
        }

        /// <summary>
        /// Gets the IP Address of the device on the connected WiFi routers network.
        /// </summary>
        public IPAddress StaIp
        {
            get
            {
                return this.staIp;
            }
        }

        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        public string DeviceId
        {
            get
            {
                return this.deviceId;
            }
        }

        /// <summary>
        /// Gets the device key.
        /// </summary>
        public string DeviceKey
        {
            get
            {
                return this.deviceKey;
            }
        }

        /// <summary>
        /// Gets the device name.
        /// </summary>
        public string DeviceName
        {
            get
            {
                return this.deviceName;
            }
        }

        /// <summary>
        /// Gets the device type.
        /// </summary>
        public string DeviceType
        {
            get
            {
                return this.deviceType;
            }
        }

        /// <summary>
        /// Gets the name of the device manufacturer.
        /// </summary>
        public string CompanyId
        {
            get
            {
                return this.companyId;
            }
        }

        /// <summary>
        /// Gets the chip used in the device.
        /// </summary>
        public string Chip
        {
            get
            {
                return this.chip;
            }
        }

        /// <summary>
        /// Gets the last known color of the device as discovered over UDP.
        /// </summary>
        public DoHomeColor Color
        {
            get
            {
                return this.color;
            }
        }

        /// <summary>
        /// Reboots the bulb.
        /// </summary>
        public void Reboot() // 3
        {
            var command = "{\"cmd\":3}";
            var response = this.SendCommand(command);
        }

        /// <summary>
        /// Gets hardware information about the bulb.
        /// </summary>
        /// <returns>The hardware information string as returned by the device.</returns>
        /// <remarks>
        /// Currently, not used anywhere so the response is not parsed.
        /// </remarks>
        public string GetDeviceInfo() // 4 -- "{"res":0,"cmd":4,"tz":13,"ver":"1.1.0","dev_id":"286dcd00fb6c_DT-WYRGB_W600","conn":1,"remote":0,"save_off_stat":1,"repeater":0,"portal":0,"chip":"W600"}"
        {
            // TODO: Implement response
            var command = "{\"cmd\":4}";
            var response = this.SendCommand(command);
            return response.ToString();
        }

        /// <summary>
        /// Changes the color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="smoothing">A boolean, indicating whether to move from the previous color to the new color gradually (true), or to jump to the new color (false).</param>
        /// <param name="smoothingDuration">If smoothing equals true, the duration of the transition in what appear to be 100 millisecond intervals. (e.g. 10 means 1 second)</param>
        /// <remarks>
        /// When using the Red, Green and Blue Gradients, set White and Warmth to 0 when using this.
        /// Setting White to a non-zero value will override Red, Green, Blue and Warmth. These will then be ignored.
        /// </remarks>
        public void ChangeColor(DoHomeColor color, bool smoothing, int? smoothingDuration = null) // 6
        {
            var smooth = smoothing ? 1 : 0;
            string command;
            if (smoothing && smoothingDuration.HasValue)
            {
                command = $"{{\"cmd\":6,\"r\":{color.Red},\"g\":{color.Green},\"b\":{color.Blue},\"w\":{color.White},\"m\":{color.Warmth},\"smooth\":{smooth},\"t\":{smoothingDuration.Value}}}";
            }
            else
            {
                command = $"{{\"cmd\":6,\"r\":{color.Red},\"g\":{color.Green},\"b\":{color.Blue},\"w\":{color.White},\"m\":{color.Warmth},\"smooth\":{smooth}}}";
            }

            this.SendCommand(command);
        }

        /// <summary>
        /// Switches the light off.
        /// </summary>
        /// <remarks>
        /// Since some firmware versions does not allow command 5, this is implemented as a color change with all values set to 0.
        /// </remarks>
        public void Off() // 6
        {
            var command = "{\"cmd\":6,\"r\":0,\"g\":0,\"b\":0,\"w\":0,\"m\":0,\"on\":0}";
            var response = this.SendCommand(command);
        }

        /// <summary>
        /// Set the bulb to a predefined color pattern.
        /// </summary>
        /// <param name="colorPattern">Color pattern</param>
        /// <remarks>
        /// Protocol documentation indicates that a frequency must be provided, but this appears to be ignored by the bulb.
        /// </remarks>
        public void SetPresetMode(DoHomeColorPattern colorPattern) // 7
        {
            var index = (int)colorPattern;
            var command = $"{{\"cmd\":7,\"index\":{index}}}";
            var response = this.SendCommand(command);
        }

        /// <summary>
        /// Gets the bulb's date and time.
        /// </summary>
        /// <returns>The bulb's date and time.</returns>
        public DateTime GetDeviceTime() // 9  -- "{"res":0,"cmd":9,"year":2021,"mon":6,"day":16,"hour":20,"min":21,"sec":18,"stamps":1623874878}"
        {
            var command = "{\"cmd\":9}";
            var response = this.SendCommand(command);
            return DateTime.UnixEpoch.AddSeconds(response.RootElement.GetProperty("stamps").GetInt32());
        }

        /// <summary>
        /// Sets the bulb's date and time.
        /// </summary>
        /// <param name="dateTime">The date and time to set.</param>
        public void SetDeviceTime(DateTime dateTime) // 10
        {
            var y = dateTime.Year;
            var m = dateTime.Month;
            var d = dateTime.Day;
            var h = dateTime.Hour;
            var q = dateTime.Minute;
            var s = dateTime.Second;
            var command = $"{{\"cmd\":10,\"year\":{y},\"month\":{m},\"day\":{d},\"hour\":{h},\"minute\":{q},\"second\":{s}}}";
            var response = this.SendCommand(command);
        }

        /// <summary>
        /// Creates a timer used to turn off the bulb.
        /// </summary>
        /// <param name="dateTime">The date and time when you want the bulb to turn off.</param>
        /// <param name="repeat">True, when this event will occur every day. False, when it only occurs once.</param>
        /// <returns>The unique identifier of the timer.</returns>
        /// <remarks>
        /// When using repeat = true, the date will be ignored; it will occur every day.
        /// </remarks>
        public uint SetShutdownTimer(DateTime dateTime, bool repeat) // 13 --> repeat = 0   {"res":0,"cmd":13}
        {
            var y = dateTime.Year;
            var m = dateTime.Month;
            var d = dateTime.Day;
            var h = dateTime.Hour;
            var q = dateTime.Minute;
            var s = dateTime.Second;
            var r = repeat ? 1 : 0;
            var ts = this.GenerateTs();
            var command = $"{{\"cmd\":13,\"ts\":{ts},\"year\":{y},\"month\":{m},\"day\":{d},\"hour\":{h},\"minute\":{q},\"second\":{s},\"repeat\":{r}}}";
            var response = this.SendCommand(command);
            return ts;
        }

        /// <summary>
        /// Creates a timer used to turn on the bulb.
        /// </summary>
        /// <param name="dateTime">The date and time when you want the bulb to turn on.</param>
        /// <param name="repeat">True, when this event will occur every day. False, when it only occurs once.</param>
        /// <returns>The unique identifier of the timer.</returns>
        public uint SetPowerupTimer(DateTime dateTime, bool repeat) // 14
        {
            var t = (int)DoHomeTimerType.TIMER_CONSTANT;
            var y = dateTime.Year;
            var m = dateTime.Month;
            var d = dateTime.Day;
            var h = dateTime.Hour;
            var q = dateTime.Minute;
            var s = dateTime.Second;
            var r = repeat ? 1 : 0;
            var ts = this.GenerateTs();
            var command = $"{{\"cmd\":14,\"ts\":{ts},\"year\":{y},\"month\":{m},\"day\":{d},\"hour\":{h},\"minute\":{q},\"second\":{s},\"type\":{t},\"repeat\":{r}}}";
            var response = this.SendCommand(command);
            return ts;
        }

        /// <summary>
        /// Sets the connected WiFi Router.
        /// </summary>
        /// <param name="ssid">The SSID of the WiFi Network.</param>
        /// <param name="password">The password to connect to the WiFi Network.</param>
        /// <param name="bssid">Optional; BSSID is simply the MAC address of a wireless access point or also known as WAP.</param>
        public void RouterConfig(string ssid, string password, string bssid = "") // 16 -- {"res":0,"cmd":16,"dev_id":"286dcd00fb6c_DT-WYRGB_W600","ssid":"2694BB42","pass":"banaanbanaanbanaan"}
        {
            var command = $"{{\"cmd\":16,\"ssid\":\"{ssid}\",\"pass\":\"{password}\",\"bssid\":\"{bssid}\"}}";
            var response = this.SendCommand(command);
        }

        /// <summary>
        /// Turns of the blub after specified minutes.
        /// </summary>
        /// <param name="minutes">Number of minutes after which to turn off the bulb.</param>
        /// <returns>The unique identifier of the timer.</returns>
        public uint DelayShutdown(int minutes) // 17 -- {"res":0,"cmd":17}
        {
            var ts = this.GenerateTs();
            var command = $"{{\"cmd\":17,\"time\":{minutes},\"ts\":{ts}}}";
            var response = this.SendCommand(command);
            return ts;
        }

        /// <summary>
        /// Get the IP Address of the bulb if connected to a router.
        /// </summary>
        /// <returns>The IP Address.</returns>
        /// <remarks>
        /// This will throw an exception when the bulb is not connected to a router.
        /// </remarks>
        public IPAddress IsConnectedToRouter() // 19
        {
            var command = "{\"cmd\":19}";
            var response = this.SendCommand(command);
            var result = IPAddress.Parse(response.RootElement.GetProperty("ip").GetString());
            return result;
        }

        /// <summary>
        /// Get the firmware version number of the bulb.
        /// </summary>
        /// <returns>Firmware version.</returns>
        public string GetVersion() // 20
        {
            var command = "{\"cmd\":20}";
            var response = this.SendCommand(command);
            return response.RootElement.GetProperty("ver").GetString();
        }

        /// <summary>
        /// Gets all the currently active timers on the bulb.
        /// </summary>
        /// <returns>A list of timers</returns>
        /// <remarks>
        /// The result will include all timers like power-up and shutdown timers as well as delayed shutdown timers.
        /// </remarks>
        public DoHomeTimer[] GetDevTimer() // 21 -- "{"res":0,"cmd":21,"timers":[]}" or ValueKind = Object : "{"res":0,"cmd":21,"timers":[{"index":0,"ts":260673,"type":0,"repeat":1,"year":2021,"mon":6,"day":21,"hour":23,"min":0,"sec":0}]}"
        {
            var command = "{\"cmd\":21}";
            var response = this.SendCommand(command);
            var timers = response.RootElement.GetProperty("timers").EnumerateArray().Select(t => new DoHomeTimer(t));
            return timers.ToArray();
        }

        /// <summary>
        /// Gets all the currently active delay shutdown timers on the bulb.
        /// </summary>
        /// <returns>A list of TIMER_DELAY_SHUTDOWN timers.</returns>
        public DoHomeTimer[] GetDelayInfo() // 22 -- "{"res":0,"cmd":22,"timers":[]}"
        {
            var command = "{\"cmd\":22}";
            var response = this.SendCommand(command);
            var timers = response.RootElement.GetProperty("timers").EnumerateArray().Select(t => new DoHomeTimer(t));
            return timers.ToArray();
        }

        /// <summary>
        /// Removes a timer from the bulb.
        /// </summary>
        /// <param name="ts">The unique identifier of the timer.</param>
        public void CancelTimer(uint ts) // 23 -- "{"res":0,"cmd":23}"
        {
            var command = $"{{\"cmd\":23,\"ts\":{ts}}}";
            var response = this.SendCommand(command);
        }

        /// <summary>
        /// Gets the current status of the light.
        /// </summary>
        /// <returns>The current <see cref="DoHomeColor"/> of the device.</returns>
        public DoHomeColor GetLedStatus() // 25 -- "{"res":0,"cmd":25,"r":0,"g":0,"b":0,"w":0,"m":0,"type":1}"
        {
            var command = "{\"cmd\":25}";
            var response = this.SendCommand(command);
            var result = JsonSerializer.Deserialize<DoHomeColor>(response.RootElement.ToString());
            return result;
        }

        /// <summary>
        /// Modify a timer on the bulb.
        /// </summary>
        /// <param name="index">The index of the timer in the array of timers obtained by <see cref="GetDevTimer"/>.</param>
        /// <param name="dateTime">The new Date Time.</param>
        /// <param name="repeat">True, when this event will occur every day. False, when it only occurs once.</param>
        /// <remarks>
        /// TIMER_DELAY_SHUTDOWN timers do not need to be modified, they can be set directly.
        /// </remarks>
        public void ModifyTimer(int index, DateTime dateTime, bool repeat) // 26
        {
            var y = dateTime.Year;
            var m = dateTime.Month;
            var d = dateTime.Day;
            var h = dateTime.Hour;
            var q = dateTime.Minute;
            var s = dateTime.Second;
            var r = repeat ? 1 : 0;
            var command = $"{{\"cmd\":26,\"index\":{index},\"year\":{y},\"month\":{m},\"day\":{d},\"hour\":{h},\"minute\":{q},\"second\":{s},\"repeat\":{r}}}";
            var response = this.SendCommand(command);
        }

        /// <summary>
        /// Resets the device's access point configuration.
        /// </summary>
        public void ResetAccessPoint() // 28
        {
            var command = "{\"cmd\":28}";
            var response = this.SendCommand(command);
        }

        /// <summary>
        /// Set the time zone offset, based on Beijing time.
        /// </summary>
        /// <param name="offset">The offset, in hours, from Beijing time.</param>
        /// <remarks>
        /// Beijing time is used by default, and it's offset is 19.
        /// UTC is 11.
        /// </remarks>
        public void SetTimeZoneOffset(int offset) // 29
        {
            if (offset < 0 || offset > 23)
            {
                return;
            }

            this.SendCommand($"{{\"cmd\":29,\"offset\":{offset}}}");
        }

        /// <summary>
        /// Clear all information and rest to factory defaults.
        /// </summary>
        public void FactoryReset() // 201
        {
            var command = "{\"cmd\":201,\"en\":1}";
            var response = this.SendCommand(command);
        }

        /// <summary>
        /// Updates the last known color of the device.
        /// </summary>
        /// <param name="color">The newly observed <see cref="DoHomeColor"/>.</param>
        internal void UpdateColor(DoHomeColor color)
        {
            this.color = color;
        }

        /// <summary>
        /// Generates a random <see cref="uint"/> to be used when creating new timers.
        /// </summary>
        /// <returns>A random <see cref="uint"/>.</returns>
        private uint GenerateTs()
        {
            var random = new Random(DateTime.Now.Millisecond);
            var thirtyBits = (uint)random.Next(1 << 30);
            var twoBits = (uint)random.Next(1 << 2);
            var result = (thirtyBits << 2) | twoBits;
            return result;
        }

        /// <summary>
        /// Sends a command over TCP.
        /// </summary>
        /// <param name="command">The command to send.</param>
        /// <returns>A response from the devices (potentially <see cref="null"/>).</returns>
        private JsonDocument SendCommand(string command)
        {
            if (!command.EndsWith("\r\n"))
            {
                command += "\r\n";
            }

            JsonDocument result = null;
            try
            {
                var stream = this.tcpClient.GetStream();
                var requestBytes = Encoding.ASCII.GetBytes(command);
                stream.Write(requestBytes, 0, requestBytes.Length);
                var responseBytes = new byte[1024];
                var responseLength = stream.Read(responseBytes, 0, responseBytes.Length);
                var response = Encoding.ASCII.GetString(responseBytes, 0, responseLength);
                
                if (response.Contains("\"cmd\":19\",ip\""))
                {
                    //// SUPER NASTY FIX FOR COMMAND 19 RESPONSE! FW 1.1.0 on W600
                    response = response.Replace("\"cmd\":19\",ip\"", "\"cmd\":19,\"ip\"");
                }

                result = JsonDocument.Parse(response);
                if (result.RootElement.TryGetProperty("res", out var res))
                {
                    if (res.GetInt32() != 0)
                    {
                        throw new Exception(((DoHomeErrorCode)res.GetInt32()).ToString());
                    }
                }
            }
            catch (InvalidOperationException)
            {
            }
            catch (IOException)
            {
            }
            catch (JsonException)
            {
            }

            return result;
        }
    }
}