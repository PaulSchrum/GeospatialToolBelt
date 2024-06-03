using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landis_Mimic
{
    internal class DoublePixel : Pixel
    {
        public Band<double> MapCode = "The numeric code for each raster cell";

        public DoublePixel()
        {
            SetBands(MapCode);
        }
    }
}
