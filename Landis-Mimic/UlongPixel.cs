using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landis_Mimic
{
    internal class UlongPixel : Pixel
    {
        public Band<ulong> MapCode = "The numeric code for each raster cell";

        public UlongPixel()
        {
            SetBands(MapCode);
        }
    }
}
