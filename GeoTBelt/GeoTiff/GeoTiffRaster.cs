using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitMiracle.LibTiff.Classic;
using GeoTBelt;
using GeoTBelt.Grid;

namespace GeoTBelt.GeoTiff
{
    public class GeoTiffRaster<T> : Raster<T> where T : struct
    {
        public int? Compression { get; set; }
        public int? BitsPerSample { get; set; }
        public int? SamplesPerPixel { get; set; }
        public int? PhotometricInterpretation { get; internal set; }
        public long[]? StripOffsets { get; internal set; }
        public int? RowsPerStrip { get; internal set; }
        public long[]? StripByteCounts { get; internal set; }
        public int? PlanarConfiguration { get; internal set; }
        public long[]? TileOffsets { get; internal set; }
        public long[]? TileByteCounts { get; internal set; }
        public long? TileWidth { get; internal set; }
        public long? TileLength { get; internal set; }
        public long? TileSize { get; internal set; }
        public int? SampleFormat { get; internal set; }

        internal GeoTiffRaster() { }

        #region BitsPerSample Values
        public static short BPS_UnsignedInteger { get { return 1; } }
        public static short BPS_SignedInteger { get { return 2; } }
        public static short BPS_IEEEFP { get { return 3; } }
        public static short BPS_Undefined { get { return 4; }}
        #endregion BitsPerSample Values

        #region PhotometricInterpretation Values
        public static short PI_WhiteIsZero { get { return 0; } }
        public static short PI_BlackIsZero { get { return 1; } }
        public static short PI_RGB { get { return 2; } }
        public static short PI_PalleteColor { get { return 3; } }
        public static short PI_TransparencyMask { get { return 4; } }
        public static short PI_CMYK { get { return 5; } }
        public static short PI_YCbCr { get { return 6; } }
        public static short PI_ICCLab { get { return 7; } }
        public static short PI_CIELab { get { return 8; } }
        #endregion PhotometricInterpretation Values

        #region SampleFormat
        public static short SF_UnsignedInteger { get { return 1; } }
        public static short SF_SignedInteger { get { return 2; } }
        public static short SF_IEEEFP { get { return 3; } }
        public static short SF_Undefined { get { return 4; } }
        public static short SF_ComplexInteger { get { return 5; } }
        #endregion SampleFormat

        #region PlanarConfiguration
        public static short PC_Contiguous { get { return 1; } }
        public static short PC_Separate { get { return 2; }   }
        #endregion PlanarConfiguration

        public void SaveAs(string outFileName)
        {
            GeoTiffHelper.WriteGeoTiff(this, outFileName);
        }

        /// <summary>
        /// Makes a new GeoTiff raster from nothing.
        /// Units of "cellSize" and the "upperLeft" variables are
        /// not proscribed. Thus the units, whether meters, feet, or
        /// degrees, will be determned in other software for now.
        /// </summary>
        /// <param name="path">Path and filename of the </param>
        /// <param name="numberOfRows">Row count for the raster</param>
        /// <param name="numberOfColumns">column count for raster</param>
        /// <param name="cellSize">Cell size in x and y</param>
        /// <param name="upperLeftX">X-coordinate of anchor point</param>
        /// <param name="upperLeftY">Y-coordinates of anchor point</param>
        /// <param name="bandCount">Number of bands to create. Currently 
        /// forced to be 1.</param>
        /// <returns>GeoTiffRaster of type T, where T is the pixel type
        /// and may be any basic type of the following: byte, sbyte, short,
        /// ushort, int, uint, long, ulong, float, or double.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static GeoTiffRaster<T> MakeNew
            (string path, int numberOfRows, int numberOfColumns,
            double cellSize = 1d, 
            double upperLeftX = 0d, double upperLeftY = 0d,
            int bandCount = 1)
        {
            var returnRaster = new GeoTiffRaster<T>();

            returnRaster.fileName = path;
            returnRaster.numColumns = numberOfColumns;
            returnRaster.numRows = numberOfRows;
            returnRaster.NoDataValueString = "-9999";
            returnRaster.cellSizeX = returnRaster.cellSizeY = cellSize;
            returnRaster.BandCount = bandCount;

            returnRaster.anchorPoint = new GTBpoint(upperLeftX, upperLeftY);
            returnRaster.leftXCoordinate = upperLeftX;
            returnRaster.topYCoordinate = upperLeftY;

            returnRaster.rightXCoordinate = upperLeftX +
                returnRaster.cellSizeX * returnRaster.numColumns;
            returnRaster.bottomYCoordinate = upperLeftY -
                returnRaster.cellSizeY * returnRaster.numRows;

            Type myPixelType = typeof (T);
            returnRaster.SampleFormat = myPixelType switch
            {
                Type _ when typeof(T) == typeof(byte) => SF_UnsignedInteger,
                Type _ when typeof(T) == typeof(sbyte) => SF_SignedInteger,
                Type _ when typeof(T) == typeof(short) => SF_SignedInteger,
                Type _ when typeof(T) == typeof(ushort) => SF_UnsignedInteger,
                Type _ when typeof(T) == typeof(int) => SF_SignedInteger,
                Type _ when typeof(T) == typeof(uint) => SF_UnsignedInteger,
                Type _ when typeof(T) == typeof(long) => SF_SignedInteger,
                Type _ when typeof(T) == typeof(ulong) => SF_UnsignedInteger,
                Type _ when typeof(T) == typeof(float) => SF_IEEEFP,
                Type _ when typeof(T) == typeof(double) => SF_IEEEFP,
                _ => SF_Undefined
            };

            returnRaster.SamplesPerPixel = 1;

            returnRaster.BitsPerSample = myPixelType switch
            {
                Type _ when typeof(T) == typeof(byte) => sizeof(byte),
                Type _ when typeof(T) == typeof(sbyte) => sizeof(sbyte),
                Type _ when typeof(T) == typeof(short) => sizeof(short),
                Type _ when typeof(T) == typeof(ushort) => sizeof(ushort),
                Type _ when typeof(T) == typeof(int) => sizeof(int),
                Type _ when typeof(T) == typeof(uint) => sizeof(uint),
                Type _ when typeof(T) == typeof(long) => sizeof(long),
                Type _ when typeof(T) == typeof(ulong) => sizeof(ulong),
                Type _ when typeof(T) == typeof(float) => sizeof(float),
                Type _ when typeof(T) == typeof(double) => sizeof(double),
                _ => throw new InvalidOperationException("Unsupported pixel type")
            } * 8;

            returnRaster.CellDataType = typeof(T);

            returnRaster.Compression = 1; // uncompressed

            returnRaster.PhotometricInterpretation = 1; // always

            returnRaster.RowsPerStrip = 1;
            var bitsPerCell = (long) (returnRaster.BitsPerSample *
                returnRaster.SamplesPerPixel);
            var bytesPerCell = bitsPerCell / 8;
            var bytesPerRow = bytesPerCell * numberOfColumns;
            var bytesPerStrip = bytesPerRow * returnRaster.RowsPerStrip;

            var numberOfStrips = 
                (int) (numberOfRows / returnRaster.RowsPerStrip);

            returnRaster.StripByteCounts = new long[numberOfStrips];
            returnRaster.StripOffsets = new long[numberOfStrips];
            long offsetAccumulator = 0;
            for ( int i = 0; i < numberOfStrips; i++ )
            {
                returnRaster.StripByteCounts[i] = (long) bytesPerStrip;
                returnRaster.StripOffsets[i] = offsetAccumulator;
                offsetAccumulator += (long)bytesPerStrip;
            }

            returnRaster.PlanarConfiguration = 1;

            returnRaster.Grid = 
                new GridInstance(numberOfColumns, numberOfRows);

            returnRaster.DataFrame = 
                new T[numberOfRows * numberOfColumns * bandCount];

            return returnRaster;
        }
    }
}
