using GeoTBelt;
using GeoTBelt.GeoTiff;

namespace Landis_Mimic
{
    public static class RasterFactory
    {
        public static IInputRaster<TPixel>
            OpenRaster<TPixel>(string path) 
            where TPixel : Pixel, new()
        {
            System.Type theType = typeof(TPixel);
            dynamic theRaster = default;

            if (theType == typeof(BytePixel))
            {
                theRaster = (GeoTiffRaster<byte>)Raster<byte>.
                    Load(path);
                return (IInputRaster<TPixel>)
                    (IInputRaster<BytePixel>)(theRaster);
            }

            else if (theType == typeof(SbytePixel))
            {
                theRaster = (GeoTiffRaster<sbyte>)Raster<sbyte>.
                    Load(path);
                return (IInputRaster<TPixel>)
                    (IInputRaster<SbytePixel>)(theRaster);
            }

            else if (theType == typeof(ShortPixel))
            {
                theRaster = (GeoTiffRaster<short>)Raster<short>.
                    Load(path);
                return (IInputRaster<TPixel>)
                    (IInputRaster<ShortPixel>)(theRaster);
            }

            else if (theType == typeof(UshortPixel))
            {
                theRaster = (GeoTiffRaster<ushort>)Raster<ushort>.
                    Load(path);
                return (IInputRaster<TPixel>)
                    (IInputRaster<UshortPixel>)(theRaster);
            }

            else if (theType == typeof(IntPixel))
            {
                theRaster = (GeoTiffRaster<int>)Raster<int>.
                    Load(path);
                return (IInputRaster<TPixel>)
                    (IInputRaster<IntPixel>)(theRaster);
            }

            else if (theType == typeof(UintPixel))
            {
                theRaster = (GeoTiffRaster<uint>)Raster<uint>.
                    Load(path);
                return (IInputRaster<TPixel>)
                    (IInputRaster<UintPixel>)(theRaster);
            }

            else if (theType == typeof(LongPixel))
            {
                theRaster = (GeoTiffRaster<long>)Raster<long>.
                    Load(path);
                return (IInputRaster<TPixel>)
                    (IInputRaster<LongPixel>)(theRaster);
            }

            else if (theType == typeof(UlongPixel))
            {
                theRaster = (GeoTiffRaster<ulong>)Raster<ulong>.
                    Load(path);
                return (IInputRaster<TPixel>)
                    (IInputRaster<UlongPixel>)(theRaster);
            }

            else if (theType == typeof(FloatPixel))
            {
                theRaster = new RasterLandis<FloatPixel, float>(path);
                return (IInputRaster<TPixel>)
                    //(IInputRaster<FloatPixel>)
                    (theRaster);
            }

            // else if (theType == typeof(DoublePixel))

            // This uses the guessing approach. Switch to the
            // I know it's a geotiff approach (GeoTiff Helper).
            theRaster = (GeoTiffRaster<double>)Raster<double>.
                Load(path);

            //theRaster = GeoTiffRaster
            return (IInputRaster<TPixel>)
                (IInputRaster <DoublePixel>)(theRaster);
        }
    }
}
