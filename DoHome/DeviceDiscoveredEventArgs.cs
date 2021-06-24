using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoHome
{
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
