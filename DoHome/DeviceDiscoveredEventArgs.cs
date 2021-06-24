//-----------------------------------------------------------------------
// <copyright file="DeviceDiscoveredEventArgs.cs" company="Company">
//    Copyright (c) Tino Donderwinkel. All rights reserved.
// </copyright>
// <author>Tino Donderwinkel</author>
//-----------------------------------------------------------------------

namespace DoHome
{
    using System;

    /// <summary>
    /// This class describes the arguments to <see cref="DoHome.DeviceDiscoveredEventHandler"/>/>
    /// </summary>
    public class DeviceDiscoveredEventArgs : EventArgs
    {
        /// <summary>
        /// The discovered device, see <see cref="DoHomeDevice"/>.
        /// </summary>
        private readonly DoHomeDevice device;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceDiscoveredEventArgs"/> class.
        /// </summary>
        /// <param name="device">The discovered device.</param>
        internal DeviceDiscoveredEventArgs(DoHomeDevice device)
        {
            this.device = device;
        }

        /// <summary>
        /// Gets the discovered device.
        /// </summary>
        public DoHomeDevice Device
        {
            get
            {
                return this.device;
            }
        }
    }
}
