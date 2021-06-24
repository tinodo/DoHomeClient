namespace DoHome
{
    using System;

    public class DeviceDiscoveredEventArgs : EventArgs
    {
        private readonly DoHomeDevice device;
        public DoHomeDevice Device
        {
            get
            {
                return this.device;
            }
        }

        internal DeviceDiscoveredEventArgs(DoHomeDevice device)
        {
            this.device = device;
        }
    }
}
