// Code copied from Landis-spatial for the purpose of developing
// code which replaces GDAL. This is similar to mocking, but it
// is not mocking. It is more brute-force.
// - Paul Schrum

namespace Landis_Mimic
{
    public class IntPixel : Pixel
    {
        public Band<int> MapCode = "The numeric code for each raster cell";

        public IntPixel()
        {
            SetBands(MapCode);
        }
    }
}
