using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTBelt
{
    internal class Pixel<T>
    {
        public List<T?> Values { get; private set; }

        public Pixel(IEnumerable<T?> values)
        {
            Values = values.ToList();
        }
    }
}
