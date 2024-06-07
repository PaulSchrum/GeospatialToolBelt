using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landis_Mimic
{
    public class FloatPixel : Pixel
    {
        public Band<float> MapCode = "The numeric code for each raster cell";

        public FloatPixel()
        {
            SetBands(MapCode);
        }
    }
}

