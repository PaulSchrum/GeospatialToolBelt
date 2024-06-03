using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landis_Mimic
{
    internal class UshortPixel : Pixel
    {
        public Band<ushort> MapCode = "The numeric code for each raster cell";

        public UshortPixel()
        {
            SetBands(MapCode);
        }
    }
}
