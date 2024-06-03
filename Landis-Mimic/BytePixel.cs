using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landis_Mimic
{
    internal class BytePixel : Pixel
    {
        public Band<byte> MapCode = "The numeric code for each raster cell";

        public BytePixel()
        {
            SetBands(MapCode);
        }
    }
}
