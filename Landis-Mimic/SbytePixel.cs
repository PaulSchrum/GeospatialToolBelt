using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landis_Mimic
{
    internal class SbytePixel : Pixel
    {
        public Band<sbyte> MapCode = "The numeric code for each raster cell";

        public SbytePixel()
        {
            SetBands(MapCode);
        }
    }
}
