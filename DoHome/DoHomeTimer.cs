using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DoHome
{
    public class DoHomeTimer
    {
        public DateTime DateTime { get; set; }
        public uint Ts { get; set; }

        public DoHomeTimerType TimerType { get; set; }

        public bool Repeat { get; set; }

        public int Index { get; set; }
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
            var index = element.GetProperty("index").GetInt32();
        }
    }
}
