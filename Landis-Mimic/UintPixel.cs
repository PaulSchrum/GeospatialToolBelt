using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landis_Mimic
{
    internal class UintPixel : Pixel
    {
        public Band<uint> MapCode = "The numeric code for each raster cell";

        public UintPixel()
        {
            SetBands(MapCode);
        }
    }
}
