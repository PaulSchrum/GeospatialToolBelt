﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitMiracle.LibTiff.Classic;

namespace GeoTBelt.GeoTiff
{
    public class GeoTiffRaster : Raster
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

    }
}
