//-----------------------------------------------------------------------
// <copyright file="DoHomeTimer.cs" company="Company">
//    Copyright (c) Tino Donderwinkel. All rights reserved.
// </copyright>
// <author>Tino Donderwinkel</author>
//-----------------------------------------------------------------------

namespace DoHome
{
    using System;
    using System.Text.Json;

    /// <summary>
    /// Represents a timer, created on the device.
    /// </summary>
    public class DoHomeTimer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoHomeTimer"/> class.
        /// </summary>
        /// <param name="element">The JSON element of the devices response in creating or getting a timer or timers.</param>
        public DoHomeTimer(JsonElement element)
        {
            var y = element.GetProperty("year").GetInt32();
            var m = element.GetProperty("mon").GetInt32();
            var d = element.GetProperty("day").GetInt32();
            var h = element.GetProperty("hour").GetInt32();
            var q = element.GetProperty("min").GetInt32();
            var s = element.GetProperty("sec").GetInt32();
            this.DateTime = new DateTime(y, m, d, h, q, s);
            this.Ts = element.GetProperty("ts").GetUInt32();
            this.TimerType = (DoHomeTimerType)element.GetProperty("type").GetInt32();
            this.Repeat = element.GetProperty("repeat").GetInt32() == 1 ? true : false;
            this.Index = element.GetProperty("index").GetInt32();
        }

        /// <summary>
        /// Gets or sets the date and time the timer will fire.
        /// </summary>
        /// <remarks>
        /// Setting the value will not be reflected on the device. It's only here for deserialization purposes.
        /// </remarks>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Gets or sets he unique identifier of the timer on the device.
        /// </summary>
        /// <remarks>
        /// Setting the value will not be reflected on the device. It's only here for deserialization purposes.
        /// </remarks>
        public uint Ts { get; set; }

        /// <summary>
        /// Gets or sets the type of timer.
        /// </summary>
        /// <remarks>
        /// Setting the value will not be reflected on the device. It's only here for deserialization purposes.
        /// </remarks>
        public DoHomeTimerType TimerType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the timer is a recurring timer (true) or a once-only timer (false).
        /// </summary>
        /// <remarks>
        /// Setting the value will not be reflected on the device. It's only here for deserialization purposes.
        /// </remarks>
        public bool Repeat { get; set; }

        /// <summary>
        /// Gets or sets the index of the timer in the list of timers on the device.
        /// </summary>
        /// <remarks>
        /// Setting the value will not be reflected on the device. It's only here for deserialization purposes.
        /// </remarks>
        public int Index { get; set; }
    }
}
