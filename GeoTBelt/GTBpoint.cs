using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTBelt
{
    public class GTBpoint
    {
        public double X { get; protected set; }
        public double Y { get; protected set; }
        public double Z { get; protected set; } = 0;
        public GTBpoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public GTBpoint(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
