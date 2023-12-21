using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTBelt.GeoTiff
{
    public class GeoTiffRaster : Raster
    {
        public dynamic Compression { get; internal set; }
        public short PhotometricInterpretation { get; internal set; }
        public dynamic StripOffsets { get; internal set; }
        public dynamic RowsPerStrip { get; internal set; }
        public dynamic StripByteCounts { get; internal set; }
        public dynamic PlanarConfiguration { get; internal set; }
        public dynamic TileOffsets { get; internal set; }
        public dynamic TileByteCounts { get; internal set; }
        public dynamic SampleFormat { get; internal set; }

        internal GeoTiffRaster() { }


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

    }
}
