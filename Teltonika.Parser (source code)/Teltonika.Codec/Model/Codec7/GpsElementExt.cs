using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teltonika.Codec.Model
{
    public struct GpsElementExt
    {
        private readonly GpsElement _gps;
        private readonly IoElement _io;

        public GpsElementExt(GpsElement gps, IoElement io)
        {
            _gps = gps;
            _io = io;
        }

        public GpsElement GPS
        {
            get { return _gps; }
        }

        public IoElement IO
        {
            get { return _io; }
        }
    }
}
