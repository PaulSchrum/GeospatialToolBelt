using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landis_Mimic
{
    internal class LongPixel : Pixel
    {
        public Band<long> MapCode = "The numeric code for each raster cell";

        public LongPixel()
        {
            SetBands(MapCode);
        }
    }
}
