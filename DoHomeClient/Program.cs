namespace DoHomeClient
{
    using System;
    using System.Drawing;
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
            //helper.StopListener();
            //first.SetPresetMode(DoHomeLedEffect.STRIP_MODE_RB_GRADIENT);
            var color1 = new DoHomeColor(4000, 1000, 0, 0, 0);
            var color2 = new DoHomeColor(0, 1000, 4000, 0, 0);
            var colorOff = new DoHomeColor(0, 0, 0, 0, 0);
            var colorAllWhite = new DoHomeColor(0, 0, 0, 5000, 0);
            var colorAllWarm = new DoHomeColor(0, 0, 0, 0, 5000);

            //Console.ReadLine();
            foreach (KnownColor k in Enum.GetValues(typeof(KnownColor)))
            {
                var color = Color.FromKnownColor(k);
                var dhc = new DoHomeColor(color);
                Console.WriteLine(k.ToString());
                //first.ChangeColor(dhc, false);
                client.ChangeColor(dhc, false, client.Devices);
                Thread.Sleep(250);
            }

            //for (int a = 5; a < 256; a+=10)
            //{
            //    for (int r = 5; r < 256; r+=10)
            //    {
            //        for (int g = 5; g < 256; g+=10)
            //        {
            //            for (int b = 5; b < 256; b+=10)
            //            {
            //                var color = Color.FromArgb(a, r, g, b);
            //                var dhv = new DoHomeColor(color);
            //                first.ChangeColor(dhv, true);
            //                //Thread.Sleep(50);
            //            }
            //        }
            //    }
            //}

            Console.ReadLine();
            //first.DelayShutdown(1, 260673);
            //Thread.Sleep(5000);
            //first.SetPowerupTimer(DateTime.Now.AddSeconds(10), 668, DoHomeTimerType.TIMER_CONSTANT, false);
            //var version = first.GetVersion();
            //var timers = first.GetDevTimer();
            //first.GetDelayInfo();
            //first.SetShutdownTimer(DateTime.Now.AddDays(10), false, 260673);
            //Console.ReadLine();
            //var timers = first.GetDevTimer();
            //first.ModifyTimer(timers.First().Index, timers.First().DateTime, true);
            //Console.ReadLine();
            //timers = first.GetDevTimer();
            //Console.ReadLine();
            //first.CancelTimer(260673);
            //Console.ReadLine();
            //first.SetShutdownTimer(DateTime.Now.AddSeconds(10), false, 260673);
            //first.SetPresetMode(DoHomeLedEffect.STRIP_MODE_INIT_MODE);
            //foreach (DoHomeColorPattern colorPattern in Enum.GetValues(typeof(DoHomeColorPattern)))
            //{
            //    first.SetPresetMode(colorPattern);
            //    Console.WriteLine(colorPattern.ToString());
            //    Console.ReadLine();
            //}
            for (int i = 0; i < 5000; i+= 100)
            {
                var color = new DoHomeColor(0, 5000, 0, i, i);
                first.ChangeColor(color, true, 5);
                Thread.Sleep(500);
            }
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
            
            //for (int w = 0; w <= 5000; w += 500)
            //{
            //    first.ChangeColor(w, w, 25, 0, 0, true);
            //    Thread.Sleep(500);
            //}
            //first.ChangeColor(2000, 1000, 1000, 0, 0);
            //Console.ReadLine();
            //var c1 = first.GetLedStatus();
            //Console.ReadLine();
            //first.ChangeColor(2000, 0, 0, 4000, 0);
            //Console.ReadLine();
            //var c2 = first.GetLedStatus();
            //Console.ReadLine();
            //first.ChangeColor(2000, 0, 0, 4000, 4000);
            //Console.ReadLine();
            //first.SetPresetMode(DoHomeLedEffect.STRIP_MODE_RGB_STROBE);
            //Console.ReadLine();
            //var c3 = first.GetLedStatus();
            //Console.ReadLine();
            //first.Off();
            //Console.ReadLine();
            //var c4 = first.GetLedStatus();
            //Console.ReadLine();
            //first.ChangeColor(2000, 4000, 1000, 0, 0, false);
            //first.SetTimezone(13);
            //Console.ReadLine();
            //var dt = first.GetDeviceTime();
            //first.SetDeviceTime(DateTime.Now);
            //Thread.Sleep(5000);
            //Console.ReadLine();
            //first.SetCustomMode(DoHomeLightMode.TIMER_CUSTOM_MODE, 5, color1, color2);
            //first.SetShutdownTimer(DateTime.Now.AddMinutes(2), false, 12321);
            //var timers = first.GetDevTimer();
            //first.ModifyTimer(timers.First().Index, DateTime.Now.AddSeconds(10), false);
            //timers = first.GetDevTimer();
            first.Off();
            client.StopListener();
            return;
        }

        private static void Helper_DeviceDiscovered(object sender, DeviceDiscoveredEventArgs e)
        {
            //Console.WriteLine($"DeviceDiscovered event received: {e.Device.UdpDeviceId}");
        }
    }
}
