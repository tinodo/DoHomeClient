namespace DoHomeClient
{
    using System;
    using System.Linq;
    using System.Threading;
    using DoHome;

    class Program
    {
        static void Main(string[] args)
        {
            var client = new DoHomeClient();
            client.DeviceDiscovered += Helper_DeviceDiscovered;
            client.StartListener();

            var first = client.Devices.FirstOrDefault();
            if (null == first)
            {
                Console.WriteLine("No devices found!");
                return;
            }

            foreach (DoHomeColorPattern colorPattern in Enum.GetValues(typeof(DoHomeColorPattern)))
            {
                first.SetPresetMode(colorPattern);
                Console.WriteLine(colorPattern.ToString());
                Thread.Sleep(500);
            }

            var color1 = new DoHomeColor(4000, 1000, 0, 0, 0);
            var color2 = new DoHomeColor(0, 1000, 4000, 0, 0);
            var colorOff = new DoHomeColor(0, 0, 0, 0, 0);
            var colorAllWhite = new DoHomeColor(0, 0, 0, 5000, 0);
            var colorAllWarm = new DoHomeColor(0, 0, 0, 0, 5000);

            for (int i = 0; i < 10; i++)
            {
                first.ChangeColor(color1, true, 5);
                Thread.Sleep(500);
                first.ChangeColor(color2, true, 5);
                Thread.Sleep(500);
            }

            for (int i = 0; i < 10; i++)
            {
                first.ChangeColor(color1, false);
                Thread.Sleep(500);
                first.ChangeColor(color2, false);
                Thread.Sleep(500);
            }

            for (int i = 0; i < 10; i++)
            {
                first.ChangeColor(colorOff, true, 5);
                Thread.Sleep(500);
                first.ChangeColor(colorAllWhite, true, 5);
                Thread.Sleep(500);
            }

            for (int i = 0; i < 10; i++)
            {
                first.ChangeColor(colorOff, false);
                Thread.Sleep(500);
                first.ChangeColor(colorAllWhite, false);
                Thread.Sleep(500);
            }

            for (int i = 0; i < 10; i++)
            {
                first.ChangeColor(colorAllWarm, true, 5);
                Thread.Sleep(500);
                first.ChangeColor(colorAllWhite, true, 5);
                Thread.Sleep(500);
            }

            for (int i = 0; i < 10; i++)
            {
                first.ChangeColor(colorAllWarm, false);
                Thread.Sleep(500);
                first.ChangeColor(colorAllWhite, false);
                Thread.Sleep(500);
            }

            var random = new Random();
            for (int c = 0; c <= 25; c++)
            {
                var color = new DoHomeColor(random.Next(5000), random.Next(5000), random.Next(5000), 0, 0);
                first.ChangeColor(color, true, 5);
                Thread.Sleep(500);
            }

            for (int c = 0; c <= 25; c++)
            {
                var color = new DoHomeColor(random.Next(5000), random.Next(5000), random.Next(5000), 0, 0);
                first.ChangeColor(color, false);
                Thread.Sleep(500);
            }
            
            first.Off();
            client.StopListener();
            return;
        }

        private static void Helper_DeviceDiscovered(object sender, DeviceDiscoveredEventArgs e)
        {
            Console.WriteLine($"DeviceDiscovered Event Received: {e.Device.UdpDeviceId}");
        }
    }
}
