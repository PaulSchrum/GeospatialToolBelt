﻿using GeoTBelt.GeoTiff;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
//using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace GeoTBelt.GeoTiff
{
    internal class TiffTagInfo
    {
        public int IdInteger { get; internal set; }
        public string IdString { get; internal set; }
        public string ShortDescription { get; internal set; }
        public string SourceOfTag { get; internal set; }
        public string Note { get; internal set; }
        //public Type BaseType { get; internal set; }
        //public bool IsArray { get; internal set; }
        //public bool IsMatrix { get; internal set; }
        //public int? columnCount { get; internal set; }
        //public int? rowCount { get; internal set; }

        public override string ToString()
        {
            return $"{IdInteger} {IdString}";
        }
    }

    /// <summary>
    /// See https://www.itu.int/itudoc/itu-t/com16/tiff-fx/docs/tiff6.pdf
    /// and the section Image File Directory for authority
    /// </summary>
    internal enum TiffTypes
    {
        BYTE = 1, // 8-bit unsigned integger, Byte in C#
        ASCII = 2, // 8-bit byte that contains a 7-bit ASCII code; the last byte must be NUL (binary zero).
        SHORT = 3, // 16-bit (2-byte) unsigned integer
        LONG = 4, // 32-bit (4-byte) unsigned integer, int in C#
        RATIONAL = 5, // Two LONGs: the first represents the numerator of a fraction; the second, the denominator.
        SBYTE = 6, // An 8-bit signed (twos-complement) integer.
        UNDEFINED = 7, // An 8-bit byte that may contain anything, depending on the definition of the field.
        SSHORT = 8, // A 16-bit (2-byte) signed (twos-complement) integer.
        SLONG = 9, // A 32-bit (4-byte) signed (twos-complement) integer.
        SRATIONAL = 10, // Two SLONG’s: the first represents the numerator of a fraction, the second the denominator.
        FLOAT = 11, // Single precision (4-byte) IEEE format.
        DOUBLE = 12, // Double precision (8-byte) IEEE format.
    }

    internal static class AllTags
    {
        private static Dictionary<int, TiffTagInfo> _tagsByInt = null;
        private static Dictionary<string, TiffTagInfo> _tagsByString = null;

        /// <summary>
        /// Get the Tiff Tag Information when all you know is the integer id.
        /// </summary>
        internal static TiffTagInfo Tag(int integerRef)
        {
            if (_tagsByInt == null)
            {
                _tagsByInt = new Dictionary<int, TiffTagInfo>();
                foreach (var entry in tempTagList)
                {
                    _tagsByInt[entry.IdInteger] = entry;
                }
                if (_tagsByString is not null)
                {
                    tempTagList.Clear();
                    tempTagList = null;
                }
            }
            if (_tagsByInt.ContainsKey(integerRef))
                return _tagsByInt[integerRef];

            return null;
        }

        /// <summary>
        /// Get the Tiff Tag Information when all you know is the text (string) id.
        /// </summary>
        internal static TiffTagInfo Tag(string stringRef)
        {
            if (_tagsByString == null)
            {
                _tagsByString = new Dictionary<string, TiffTagInfo>();
                foreach (var entry in tempTagList)
                {
                    if(!_tagsByString.ContainsKey(entry.IdString))
                        _tagsByString[entry.IdString] = entry;
                }
                if (_tagsByInt is not null)
                {
                    tempTagList.Clear();
                    tempTagList = null;
                }
            }
            if (_tagsByString.ContainsKey(stringRef))
                return _tagsByString[stringRef];

            return null;
        }

        private static List<TiffTagInfo> tempTagList = new List<TiffTagInfo>
        {
            { new TiffTagInfo {IdInteger = 254, IdString = "NewSubfileType", ShortDescription = "A general indication of the kind of data contained in this subfile." , SourceOfTag = " Baseline" , Note = "Usage rule in JHOVE TIFF module." } },
            { new TiffTagInfo {IdInteger = 255, IdString = "SubfileType", ShortDescription = "A general indication of the kind of data contained in this subfile." , SourceOfTag = " Baseline" , Note = " " } },
            { new TiffTagInfo {IdInteger = 256, IdString = "ImageWidth", ShortDescription = "The number of columns in the image, i.e., the number of pixels per row." , SourceOfTag = " Baseline" , Note = "Mandatory for TIFF 6.0 classes B, G, P, R, and Y.1" } },
            { new TiffTagInfo {IdInteger = 257, IdString = "ImageLength", ShortDescription = "The number of rows of pixels in the image." , SourceOfTag = " Baseline" , Note = "Mandatory for TIFF 6.0 classes B, G, P, R, and Y.1" } },
            { new TiffTagInfo {IdInteger = 258, IdString = "BitsPerSample", ShortDescription = "Number of bits per component." , SourceOfTag = " Baseline" , Note = "" } },
            { new TiffTagInfo {IdInteger = 259, IdString = "Compression", ShortDescription = "Compression scheme used on the image data." , SourceOfTag = " Baseline" , Note = "Sample values: 1=uncompressed and 4=CCITT Group 4." } },
            { new TiffTagInfo {IdInteger = 262, IdString = "PhotometricInterpretation", ShortDescription = "The color space of the image data." , SourceOfTag = " Baseline" , Note = "Sample values: 1=black is zero and 2=RGB. Document also states \"RGB is assumed to be sRGB; if RGB, an ICC profile should be present in the 34675 tag.\"" } },
            { new TiffTagInfo { IdInteger = 263, IdString = "Threshholding", ShortDescription = "For black and white TIFF files that represent shades of gray, the technique used to convert from gray to black and white pixels.", SourceOfTag = " Baseline", Note = "Usage rule in JHOVE TIFF module." } },
            { new TiffTagInfo { IdInteger = 264, IdString = "CellWidth", ShortDescription = "The width of the dithering or halftoning matrix used to create a dithered or halftoned bilevel file.", SourceOfTag = " Baseline", Note = " " } },
            { new TiffTagInfo { IdInteger = 265, IdString = "CellLength", ShortDescription = "The length of the dithering or halftoning matrix used to create a dithered or halftoned bilevel file.", SourceOfTag = " Baseline", Note = "Usage rule in JHOVE TIFF module." } },
            { new TiffTagInfo { IdInteger = 266, IdString = "FillOrder", ShortDescription = "The logical order of bits within a byte.", SourceOfTag = " Baseline", Note = " " } },
            { new TiffTagInfo { IdInteger = 269, IdString = "DocumentName", ShortDescription = "The name of the document from which this image was scanned.", SourceOfTag = " Extended", Note = "Also used by HD Photo." } },
            { new TiffTagInfo { IdInteger = 270, IdString = "ImageDescription", ShortDescription = "A string that describes the subject of the image.", SourceOfTag = " Baseline", Note = "Also used by HD Photo" } },
            { new TiffTagInfo { IdInteger = 271, IdString = "Make", ShortDescription = "The scanner manufacturer.", SourceOfTag = " Baseline", Note = "Also used by HD Photo." } },
            { new TiffTagInfo { IdInteger = 272, IdString = "Model", ShortDescription = "The scanner model name or number.", SourceOfTag = " Baseline", Note = "Also used by HD Photo." } },
            { new TiffTagInfo { IdInteger = 273, IdString = "StripOffsets", ShortDescription = "For each strip, the byte offset of that strip.", SourceOfTag = " Baseline", Note = "Mandatory for TIFF 6.0 classes B, G, P, R, and Y.1 (Files outside of these classes may use tiles and tags 322, 323, 324, and 325; Comments welcome.)" } },
            { new TiffTagInfo { IdInteger = 274, IdString = "Orientation", ShortDescription = "The orientation of the image with respect to the rows and columns.", SourceOfTag = " Baseline", Note = "Mandatory for TIFF/EP." } },
            { new TiffTagInfo { IdInteger = 277, IdString = "SamplesPerPixel", ShortDescription = "The number of components per pixel.", SourceOfTag = " Baseline", Note = "Mandatory for TIFF 6.0 classes R and Y.1" } },
            { new TiffTagInfo { IdInteger = 278, IdString = "RowsPerStrip", ShortDescription = "The number of rows per strip.", SourceOfTag = " Baseline", Note = "Mandatory for TIFF 6.0 classes B, G, P, R, and Y.1 (Files outside of these classes may use tiles and tags 322, 323, 324, and 325; Comments welcome.)" } },
            { new TiffTagInfo { IdInteger = 279, IdString = "StripByteCounts", ShortDescription = "For each strip, the number of bytes in the strip after compression.", SourceOfTag = " Baseline", Note = "Mandatory for TIFF 6.0 classes B, G, P, R, and Y.1 (Files outside of these classes may use tiles and tags 322, 323, 324, and 325; Comments welcome.)" } },
            { new TiffTagInfo { IdInteger = 280, IdString = "MinSampleValue", ShortDescription = "The minimum component value used.", SourceOfTag = " Baseline", Note = " " } },
            { new TiffTagInfo { IdInteger = 281, IdString = "MaxSampleValue", ShortDescription = "The maximum component value used.", SourceOfTag = " Baseline", Note = " " } },
            { new TiffTagInfo { IdInteger = 282, IdString = "XResolution", ShortDescription = "The number of pixels per ResolutionUnit in the ImageWidth direction.", SourceOfTag = " Baseline", Note = "Xresolution is a Rational; ImageWidth (Tag 256) is the numerator and the length of the source (measured in the units specified in ResolutionUnit (Tag 296)) is the denominator." } },
            { new TiffTagInfo { IdInteger = 283, IdString = "YResolution", ShortDescription = "The number of pixels per ResolutionUnit in the ImageLength direction.", SourceOfTag = " Baseline", Note = "Mandatory for TIFF 6.0 classes B, G, P, R, and Y.1" } },
            { new TiffTagInfo { IdInteger = 284, IdString = "PlanarConfiguration", ShortDescription = "How the components of each pixel are stored.", SourceOfTag = " Baseline", Note = "Mandatory for TIFF/EP." } },
            { new TiffTagInfo { IdInteger = 285, IdString = "PageName", ShortDescription = "The name of the page from which this image was scanned.", SourceOfTag = " Extended", Note = "Also used by HD Photo" } },
            { new TiffTagInfo { IdInteger = 286, IdString = "XPosition", ShortDescription = "X position of the image.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 287, IdString = "YPosition", ShortDescription = "Y position of the image.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 288, IdString = "FreeOffsets", ShortDescription = "For each string of contiguous unused bytes in a TIFF file, the byte offset of the string.", SourceOfTag = " Baseline", Note = " " } },
            { new TiffTagInfo { IdInteger = 289, IdString = "FreeByteCounts", ShortDescription = "For each string of contiguous unused bytes in a TIFF file, the number of bytes in the string.", SourceOfTag = " Baseline", Note = " " } },
            { new TiffTagInfo { IdInteger = 290, IdString = "GrayResponseUnit", ShortDescription = "The precision of the information contained in the GrayResponseCurve.", SourceOfTag = " Baseline", Note = " " } },
            { new TiffTagInfo { IdInteger = 291, IdString = "GrayResponseCurve", ShortDescription = "For grayscale data, the optical density of each possible pixel value.", SourceOfTag = " Baseline", Note = " " } },
            { new TiffTagInfo { IdInteger = 292, IdString = "T4Options", ShortDescription = "Options for Group 3 Fax compression", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 293, IdString = "T6Options", ShortDescription = "Options for Group 4 Fax compression", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 296, IdString = "ResolutionUnit", ShortDescription = "The unit of measurement for XResolution and YResolution.", SourceOfTag = " Baseline", Note = "Mandatory for TIFF 6.0 classes B, G, P, R, and Y.1" } },
            { new TiffTagInfo { IdInteger = 297, IdString = "PageNumber", ShortDescription = "The page number of the page from which this image was scanned.", SourceOfTag = " Extended", Note = "Also used by HD Photo" } },
            { new TiffTagInfo { IdInteger = 301, IdString = "TransferFunction", ShortDescription = "Describes a transfer function for the image in tabular style.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 305, IdString = "Software", ShortDescription = "Name and version number of the software package(s) used to create the image.", SourceOfTag = " Baseline", Note = "Also used by HD Photo." } },
            { new TiffTagInfo { IdInteger = 306, IdString = "DateTime", ShortDescription = "Date and time of image creation.", SourceOfTag = " Baseline", Note = "Also used by HD Photo." } },
            { new TiffTagInfo { IdInteger = 315, IdString = "Artist", ShortDescription = "Person who created the image.", SourceOfTag = " Baseline", Note = "Also used by HD Photo" } },
            { new TiffTagInfo { IdInteger = 316, IdString = "HostComputer", ShortDescription = "The computer and/or operating system in use at the time of image creation.", SourceOfTag = " Baseline", Note = "Also used by HD Photo" } },
            { new TiffTagInfo { IdInteger = 317, IdString = "Predictor", ShortDescription = "A mathematical operator that is applied to the image data before an encoding scheme is applied.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 318, IdString = "WhitePoint", ShortDescription = "The chromaticity of the white point of the image.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 319, IdString = "PrimaryChromaticities", ShortDescription = "The chromaticities of the primaries of the image.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 320, IdString = "ColorMap", ShortDescription = "A color map for palette color images.", SourceOfTag = " Baseline", Note = "Mandatory for TIFF 6.0 class P.1" } },
            { new TiffTagInfo { IdInteger = 321, IdString = "HalftoneHints", ShortDescription = "Conveys to the halftone function the range of gray levels within a colorimetrically-specified image that should retain tonal detail.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 322, IdString = "TileWidth", ShortDescription = "The tile width in pixels. This is the number of columns in each tile.", SourceOfTag = " Extended", Note = "Mandatory for TIFF 6.0 files that use tiles. (Files that use strips employ tags 273, 278, and 279.)" } },
            { new TiffTagInfo { IdInteger = 323, IdString = "TileLength", ShortDescription = "The tile length (height) in pixels. This is the number of rows in each tile.", SourceOfTag = " Extended", Note = "Referenced in JHOVE TIFF module for files that use tiles. (Files that use strips employ tags 273, 278, and 279.)" } },
            { new TiffTagInfo { IdInteger = 324, IdString = "TileOffsets", ShortDescription = "For each tile, the byte offset of that tile, as compressed and stored on disk.", SourceOfTag = " Extended", Note = "Mandatory for TIFF 6.0 files that use tiles. (Files that use strips employ tags 273, 278, and 279.)" } },
            { new TiffTagInfo { IdInteger = 325, IdString = "TileByteCounts", ShortDescription = "For each tile, the number of (compressed) bytes in that tile.", SourceOfTag = " Extended", Note = "Mandatory for TIFF 6.0 files that use tiles. (Files that use strips employ tags 273, 278, and 279.)" } },
            { new TiffTagInfo { IdInteger = 326, IdString = "BadFaxLines", ShortDescription = "Used in the TIFF-F standard, denotes the number of 'bad' scan lines encountered by the facsimile device.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 327, IdString = "CleanFaxData", ShortDescription = "Used in the TIFF-F standard, indicates if 'bad' lines encountered during reception are stored in the data, or if 'bad' lines have been replaced by the receiver.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 328, IdString = "ConsecutiveBadFaxLines", ShortDescription = "Used in the TIFF-F standard, denotes the maximum number of consecutive 'bad' scanlines received.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 330, IdString = "SubIFDs", ShortDescription = "Offset to child IFDs.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 332, IdString = "InkSet", ShortDescription = "The set of inks used in a separated (PhotometricInterpretation=5) image.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 333, IdString = "InkNames", ShortDescription = "The name of each ink used in a separated image.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 334, IdString = "NumberOfInks", ShortDescription = "The number of inks.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 336, IdString = "DotRange", ShortDescription = "The component values that correspond to a 0% dot and 100% dot.", SourceOfTag = " Extended", Note = "Usage rule in JHOVE TIFF module." } },
            { new TiffTagInfo { IdInteger = 337, IdString = "TargetPrinter", ShortDescription = "A description of the printing environment for which this separation is intended.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 338, IdString = "ExtraSamples", ShortDescription = "Description of extra components.", SourceOfTag = " Baseline", Note = " " } },
            { new TiffTagInfo { IdInteger = 339, IdString = "SampleFormat", ShortDescription = "Specifies how to interpret each data sample in a pixel.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 340, IdString = "SMinSampleValue", ShortDescription = "Specifies the minimum sample value.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 341, IdString = "SMaxSampleValue", ShortDescription = "Specifies the maximum sample value.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 342, IdString = "TransferRange", ShortDescription = "Expands the range of the TransferFunction.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 343, IdString = "ClipPath", ShortDescription = "Mirrors the essentials of PostScript's path creation functionality.", SourceOfTag = " Extended", Note = "Usage rule in JHOVE TIFF module." } },
            { new TiffTagInfo { IdInteger = 344, IdString = "XClipPathUnits", ShortDescription = "The number of units that span the width of the image, in terms of integer ClipPath coordinates.", SourceOfTag = " Extended", Note = "Usage rule in JHOVE TIFF module." } },
            { new TiffTagInfo { IdInteger = 345, IdString = "YClipPathUnits", ShortDescription = "The number of units that span the height of the image, in terms of integer ClipPath coordinates.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 346, IdString = "Indexed", ShortDescription = "Aims to broaden the support for indexed images to include support for any color space.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 347, IdString = "JPEGTables", ShortDescription = "JPEG quantization and/or Huffman tables.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 351, IdString = "OPIProxy", ShortDescription = "OPI-related.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 400, IdString = "GlobalParametersIFD", ShortDescription = "Used in the TIFF-FX standard to point to an IFD containing tags that are globally applicable to the complete TIFF file.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 401, IdString = "ProfileType", ShortDescription = "Used in the TIFF-FX standard, denotes the type of data stored in this file or IFD.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 402, IdString = "FaxProfile", ShortDescription = "Used in the TIFF-FX standard, denotes the 'profile' that applies to this file.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 403, IdString = "CodingMethods", ShortDescription = "Used in the TIFF-FX standard, indicates which coding methods are used in the file.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 404, IdString = "VersionYear", ShortDescription = "Used in the TIFF-FX standard, denotes the year of the standard specified by the FaxProfile field.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 405, IdString = "ModeNumber", ShortDescription = "Used in the TIFF-FX standard, denotes the mode of the standard specified by the FaxProfile field.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 433, IdString = "Decode", ShortDescription = "Used in the TIFF-F and TIFF-FX standards, holds information about the ITULAB (PhotometricInterpretation = 10) encoding.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 434, IdString = "DefaultImageColor", ShortDescription = "Defined in the Mixed Raster Content part of RFC 2301, is the default color needed in areas where no image is available.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 512, IdString = "JPEGProc", ShortDescription = "Old-style JPEG compression field. TechNote2 invalidates this part of the specification.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 513, IdString = "JPEGInterchangeFormat", ShortDescription = "Old-style JPEG compression field. TechNote2 invalidates this part of the specification.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 514, IdString = "JPEGInterchangeFormatLength", ShortDescription = "Old-style JPEG compression field. TechNote2 invalidates this part of the specification.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 515, IdString = "JPEGRestartInterval", ShortDescription = "Old-style JPEG compression field. TechNote2 invalidates this part of the specification.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 517, IdString = "JPEGLosslessPredictors", ShortDescription = "Old-style JPEG compression field. TechNote2 invalidates this part of the specification.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 518, IdString = "JPEGPointTransforms", ShortDescription = "Old-style JPEG compression field. TechNote2 invalidates this part of the specification.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 519, IdString = "JPEGQTables", ShortDescription = "Old-style JPEG compression field. TechNote2 invalidates this part of the specification.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 520, IdString = "JPEGDCTables", ShortDescription = "Old-style JPEG compression field. TechNote2 invalidates this part of the specification.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 521, IdString = "JPEGACTables", ShortDescription = "Old-style JPEG compression field. TechNote2 invalidates this part of the specification.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 529, IdString = "YCbCrCoefficients", ShortDescription = "The transformation from RGB to YCbCr image data.", SourceOfTag = " Extended", Note = "Mandatory for TIFF/EP YCbCr images." } },
            { new TiffTagInfo { IdInteger = 530, IdString = "YCbCrSubSampling", ShortDescription = "Specifies the subsampling factors used for the chrominance components of a YCbCr image.", SourceOfTag = " Extended", Note = "Mandatory for TIFF/EP YCbCr images." } },
            { new TiffTagInfo { IdInteger = 531, IdString = "YCbCrPositioning", ShortDescription = "Specifies the positioning of subsampled chrominance components relative to luminance samples.", SourceOfTag = " Extended", Note = "Mandatory for TIFF/EP YCbCr images." } },
            { new TiffTagInfo { IdInteger = 532, IdString = "ReferenceBlackWhite", ShortDescription = "Specifies a pair of headroom and footroom image data values (codes) for each pixel component.", SourceOfTag = " Extended", Note = "Mandatory for TIFF 6.0 class Y.1" } },
            { new TiffTagInfo { IdInteger = 559, IdString = "StripRowCounts", ShortDescription = "Defined in the Mixed Raster Content part of RFC 2301, used to replace RowsPerStrip for IFDs with variable-sized strips.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 700, IdString = "XMP", ShortDescription = "XML packet containing XMP metadata", SourceOfTag = " Extended", Note = "Also used by HD Photo" } },
            { new TiffTagInfo { IdInteger = 18246, IdString = "Image.Rating", ShortDescription = "Ratings tag used by Windows", SourceOfTag = " Exif private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 18249, IdString = "Image.RatingPercent", ShortDescription = "Ratings tag used by Windows, value as percent", SourceOfTag = " Exif private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 32781, IdString = "ImageID", ShortDescription = "OPI-related.", SourceOfTag = " Extended", Note = " " } },
            { new TiffTagInfo { IdInteger = 32932, IdString = "Wang Annotation", ShortDescription = "Annotation data, as used in 'Imaging for Windows'.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 33421, IdString = "CFARepeatPatternDim", ShortDescription = "For camera raw files from sensors with CFA overlay.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 33422, IdString = "CFAPattern", ShortDescription = "For camera raw files from sensors with CFA overlay.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 33423, IdString = "BatteryLevel", ShortDescription = "Encodes camera battery level at time of image capture.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 33432, IdString = "Copyright", ShortDescription = "Copyright notice.", SourceOfTag = " Baseline", Note = "Also used by HD Photo." } },
            { new TiffTagInfo { IdInteger = 33434, IdString = "ExposureTime", ShortDescription = "Exposure time, given in seconds.", SourceOfTag = " Exif Private IFD", Note = "" } },
            { new TiffTagInfo { IdInteger = 33437, IdString = "FNumber", ShortDescription = "The F number.", SourceOfTag = " Exif Private IFD", Note = "" } },
            { new TiffTagInfo { IdInteger = 33445, IdString = "MD FileTag", ShortDescription = "Specifies the pixel data format encoding in the Molecular Dynamics GEL file format.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 33446, IdString = "MD ScalePixel", ShortDescription = "Specifies a scale factor in the Molecular Dynamics GEL file format.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 33447, IdString = "MD ColorTable", ShortDescription = "Used to specify the conversion from 16bit to 8bit in the Molecular Dynamics GEL file format.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 33448, IdString = "MD LabName", ShortDescription = "Name of the lab that scanned this file, as used in the Molecular Dynamics GEL file format.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 33449, IdString = "MD SampleInfo", ShortDescription = "Information about the sample, as used in the Molecular Dynamics GEL file format.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 33450, IdString = "MD PrepDate", ShortDescription = "Date the sample was prepared, as used in the Molecular Dynamics GEL file format.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 33451, IdString = "MD PrepTime", ShortDescription = "Time the sample was prepared, as used in the Molecular Dynamics GEL file format.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 33452, IdString = "MD FileUnits", ShortDescription = "Units for data in this file, as used in the Molecular Dynamics GEL file format.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 33550, IdString = "ModelPixelScaleTag", ShortDescription = "Used in interchangeable GeoTIFF_1_0 files.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 33723, IdString = "IPTC/NAA", ShortDescription = "IPTC-NAA (International Press Telecommunications Council-Newspaper Association of America) metadata.", SourceOfTag = " TIFF/EP spec, p. 33", Note = "Tag name and values defined by IPTC-NAA Info Interchange Model & Digital Newsphoto Parameter Record." } },
            { new TiffTagInfo { IdInteger = 33918, IdString = "INGR Packet Data Tag", ShortDescription = "Intergraph Application specific storage.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 33919, IdString = "INGR Flag Registers", ShortDescription = "Intergraph Application specific flags.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 33920, IdString = "IrasB Transformation Matrix", ShortDescription = "Originally part of Intergraph's GeoTIFF tags, but likely understood by IrasB only.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 33922, IdString = "ModelTiepointTag", ShortDescription = "Originally part of Intergraph's GeoTIFF tags, but now used in interchangeable GeoTIFF_1_0 files.", SourceOfTag = " Private", Note = "In GeoTIFF_1_0, either this tag or 34264 must be defined, but not both" } },
            { new TiffTagInfo { IdInteger = 34016, IdString = "Site", ShortDescription = "Site where image created.", SourceOfTag = " TIFF/IT spec, 7.2.3", Note = " " } },
            { new TiffTagInfo { IdInteger = 34017, IdString = "ColorSequence", ShortDescription = "Sequence of colors if other than CMYK.", SourceOfTag = " TIFF/IT spec, 7.2.8.3.2", Note = "For BP and BL only2" } },
            { new TiffTagInfo { IdInteger = 34018, IdString = "IT8Header", ShortDescription = "Certain inherited headers.", SourceOfTag = " TIFF/IT spec, 7.2.3", Note = "Obsolete" } },
            { new TiffTagInfo { IdInteger = 34019, IdString = "RasterPadding", ShortDescription = "Type of raster padding used, if any.", SourceOfTag = " TIFF/IT spec, 7.2.6", Note = " " } },
            { new TiffTagInfo { IdInteger = 34020, IdString = "BitsPerRunLength", ShortDescription = "Number of bits for short run length encoding.", SourceOfTag = " TIFF/IT spec, 7.2.6", Note = "For LW only2" } },
            { new TiffTagInfo { IdInteger = 34021, IdString = "BitsPerExtendedRunLength", ShortDescription = "Number of bits for long run length encoding.", SourceOfTag = " TIFF/IT spec, 7.2.6", Note = "For LW only2" } },
            { new TiffTagInfo { IdInteger = 34022, IdString = "ColorTable", ShortDescription = "Color value in a color pallette.", SourceOfTag = " TIFF/IT spec, 7.2.8.4", Note = "For BP and BL only2" } },
            { new TiffTagInfo { IdInteger = 34023, IdString = "ImageColorIndicator", ShortDescription = "Indicates if image (foreground) color or transparency is specified.", SourceOfTag = " TIFF/IT spec, 7.2.9", Note = "For MP, BP, and BL only2" } },
            { new TiffTagInfo { IdInteger = 34024, IdString = "BackgroundColorIndicator", ShortDescription = "Background color specification.", SourceOfTag = " TIFF/IT spec, 7.2.9", Note = "For BP and BL only2" } },
            { new TiffTagInfo { IdInteger = 34025, IdString = "ImageColorValue", ShortDescription = "Specifies image (foreground) color.", SourceOfTag = " TIFF/IT spec, 7.2.8.4", Note = "For MP, BP, and BL only2" } },
            { new TiffTagInfo { IdInteger = 34026, IdString = "BackgroundColorValue", ShortDescription = "Specifies background color.", SourceOfTag = " TIFF/IT spec, 7.2.8.4", Note = "For BP and BL only2" } },
            { new TiffTagInfo { IdInteger = 34027, IdString = "PixelIntensityRange", ShortDescription = "Specifies data values for 0 percent and 100 percent pixel intensity.", SourceOfTag = " TIFF/IT spec, 7.2.8.4", Note = "For MP only2" } },
            { new TiffTagInfo { IdInteger = 34028, IdString = "TransparencyIndicator", ShortDescription = "Specifies if transparency is used in HC file.", SourceOfTag = " TIFF/IT spec, 7.2.8.4", Note = "For HC only2" } },
            { new TiffTagInfo { IdInteger = 34029, IdString = "ColorCharacterization", ShortDescription = "Specifies ASCII table or other reference per ISO 12641 and ISO 12642.", SourceOfTag = " TIFF/IT spec, 7.2.8.4", Note = " " } },
            { new TiffTagInfo { IdInteger = 34030, IdString = "HCUsage", ShortDescription = "Indicates the type of information in an HC file.", SourceOfTag = " TIFF/IT spec, 7.2.6", Note = "For HC only2" } },
            { new TiffTagInfo { IdInteger = 34031, IdString = "TrapIndicator", ShortDescription = "Indicates whether or not trapping has been applied to the file.", SourceOfTag = " TIFF/IT spec, 7.2.6", Note = " " } },
            { new TiffTagInfo { IdInteger = 34032, IdString = "CMYKEquivalent", ShortDescription = "Specifies CMYK equivalent for specific separations.", SourceOfTag = " TIFF/IT spec, 7.2.8.3.4", Note = " " } },
            { new TiffTagInfo { IdInteger = 34033, IdString = "Reserved", ShortDescription = "For future TIFF/IT use", SourceOfTag = " TIFF/IT spec", Note = " " } },
            { new TiffTagInfo { IdInteger = 34034, IdString = "Reserved", ShortDescription = "For future TIFF/IT use", SourceOfTag = " TIFF/IT spec", Note = " " } },
            { new TiffTagInfo { IdInteger = 34035, IdString = "Reserved", ShortDescription = "For future TIFF/IT use", SourceOfTag = " TIFF/IT spec", Note = " " } },
            { new TiffTagInfo { IdInteger = 34264, IdString = "ModelTransformationTag", ShortDescription = "Used in interchangeable GeoTIFF_1_0 files.", SourceOfTag = " Private", Note = "In GeoTIFF_1_0, either this tag or 33922 must be defined, but not both" } },
            { new TiffTagInfo { IdInteger = 34377, IdString = "Photoshop", ShortDescription = "Collection of Photoshop 'Image Resource Blocks'.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 34665, IdString = "Exif IFD", ShortDescription = "A pointer to the Exif IFD.", SourceOfTag = " Private", Note = "Also used by HD Photo." } },
            { new TiffTagInfo { IdInteger = 34675, IdString = "InterColorProfile", ShortDescription = "ICC profile data.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 34732, IdString = "ImageLayer", ShortDescription = "Defined in the Mixed Raster Content part of RFC 2301, used to denote the particular function of this Image in the mixed raster scheme.", SourceOfTag = " Extended", Note = " " } },

            // GeoTiff-specific tags
            { new TiffTagInfo { IdInteger = 34735, IdString = "GeoKeyDirectoryTag", ShortDescription = "Used in interchangeable GeoTIFF_1_0 files.", SourceOfTag = " Private", Note = "Mandatory in GeoTIFF_1_0" } },
            { new TiffTagInfo { IdInteger = 34736, IdString = "GeoDoubleParamsTag", ShortDescription = "Used in interchangeable GeoTIFF_1_0 files.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 34737, IdString = "GeoAsciiParamsTag", ShortDescription = "Used in interchangeable GeoTIFF_1_0 files.", SourceOfTag = " Private", Note = " " } },



            { new TiffTagInfo { IdInteger = 34850, IdString = "ExposureProgram", ShortDescription = "The class of the program used by the camera to set exposure when the picture is taken.", SourceOfTag = " Exif Private IFD", Note = "" } },
            { new TiffTagInfo { IdInteger = 34852, IdString = "SpectralSensitivity", ShortDescription = "Indicates the spectral sensitivity of each channel of the camera used.", SourceOfTag = " Exif Private IFD", Note = "" } },
            { new TiffTagInfo { IdInteger = 34853, IdString = "GPSInfo", ShortDescription = "A pointer to the Exif-related GPS Info IFD.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 34855, IdString = "ISOSpeedRatings", ShortDescription = "Indicates the ISO Speed and ISO Latitude of the camera or input device as specified in ISO 12232.", SourceOfTag = " Exif Private IFD", Note = "" } },
            { new TiffTagInfo { IdInteger = 34856, IdString = "OECF", ShortDescription = "Indicates the Opto-Electric Conversion Function (OECF) specified in ISO 14524.", SourceOfTag = " Exif Private IFD", Note = "" } },
            { new TiffTagInfo { IdInteger = 34857, IdString = "Interlace", ShortDescription = "Indicates the field number of multifield images.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 34858, IdString = "TimeZoneOffset", ShortDescription = "Encodes time zone of camera clock relative to GMT.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 34859, IdString = "SelfTimeMode", ShortDescription = "Number of seconds image capture was delayed from button press.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 34864, IdString = "SensitivityType", ShortDescription = "The SensitivityType tag indicates PhotographicSensitivity tag, which one of the parameters of ISO 12232. Although it is an optional tag, it should be recorded when a PhotographicSensitivity tag is recorded. Value = 4, 5, 6, or 7 may be used in case that the values of plural parameters are the same.", SourceOfTag = " Exif private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 34865, IdString = "StandardOutputSensitivity", ShortDescription = "This tag indicates the standard output sensitivity value of a camera or input device defined in ISO 12232. When recording this tag, the PhotographicSensitivity and SensitivityType tags shall also be recorded.", SourceOfTag = " Exif private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 34866, IdString = "RecommendedExposureIndex", ShortDescription = "This tag indicates the recommended exposure index value of a camera or input device defined in ISO 12232. When recording this tag, the PhotographicSensitivity and SensitivityType tags shall also be recorded.", SourceOfTag = " Exif private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 34867, IdString = "ISOSpeed", ShortDescription = "This tag indicates the ISO speed value of a camera or input device that is defined in ISO 12232. When recording this tag, the PhotographicSensitivity and SensitivityType tags shall also be recorded.", SourceOfTag = " Exif private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 34868, IdString = "ISOSpeedLatitudeyyy", ShortDescription = "This tag indicates the ISO speed latitude yyy value of a camera or input device that is defined in ISO 12232. However, this tag shall not be recorded without ISOSpeed and ISOSpeedLatitudezzz.", SourceOfTag = " Exif private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 34869, IdString = "ISOSpeedLatitudezzz", ShortDescription = "This tag indicates the ISO speed latitude zzz value of a camera or input device that is defined in ISO 12232. However, this tag shall not be recorded without ISOSpeed and ISOSpeedLatitudeyyy.", SourceOfTag = " Exif private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 34908, IdString = "HylaFAX FaxRecvParams", ShortDescription = "Used by HylaFAX.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 34909, IdString = "HylaFAX FaxSubAddress", ShortDescription = "Used by HylaFAX.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 34910, IdString = "HylaFAX FaxRecvTime", ShortDescription = "Used by HylaFAX.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 36864, IdString = "ExifVersion", ShortDescription = "The version of the supported Exif standard.", SourceOfTag = " Exif Private IFD", Note = "Mandatory in the Exif IFD." } },
            { new TiffTagInfo { IdInteger = 36867, IdString = "DateTimeOriginal", ShortDescription = "The date and time when the original image data was generated.", SourceOfTag = " Exif Private IFD", Note = "" } },
            { new TiffTagInfo { IdInteger = 36868, IdString = "DateTimeDigitized", ShortDescription = "The date and time when the image was stored as digital data.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 37121, IdString = "ComponentsConfiguration", ShortDescription = "Specific to compressed data; specifies the channels and complements PhotometricInterpretation", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 37122, IdString = "CompressedBitsPerPixel", ShortDescription = "Specific to compressed data; states the compressed bits per pixel.", SourceOfTag = " Exif Private IFD", Note = "" } },
            { new TiffTagInfo { IdInteger = 37377, IdString = "ShutterSpeedValue", ShortDescription = "Shutter speed.", SourceOfTag = " Exif Private IFD", Note = "" } },
            { new TiffTagInfo { IdInteger = 37378, IdString = "ApertureValue", ShortDescription = "The lens aperture.", SourceOfTag = " Exif Private IFD", Note = "" } },
            { new TiffTagInfo { IdInteger = 37379, IdString = "BrightnessValue", ShortDescription = "The value of brightness.", SourceOfTag = " Exif Private IFD", Note = "" } },
            { new TiffTagInfo { IdInteger = 37380, IdString = "ExposureBiasValue", ShortDescription = "The exposure bias.", SourceOfTag = " Exif Private IFD", Note = "" } },
            { new TiffTagInfo { IdInteger = 37381, IdString = "MaxApertureValue", ShortDescription = "The smallest F number of the lens.", SourceOfTag = " Exif Private IFD", Note = "" } },
            { new TiffTagInfo { IdInteger = 37382, IdString = "SubjectDistance", ShortDescription = "The distance to the subject, given in meters.", SourceOfTag = " Exif Private IFD", Note = "" } },
            { new TiffTagInfo { IdInteger = 37383, IdString = "MeteringMode", ShortDescription = "The metering mode.", SourceOfTag = " Exif Private IFD", Note = "" } },
            { new TiffTagInfo { IdInteger = 37384, IdString = "LightSource", ShortDescription = "The kind of light source.", SourceOfTag = " Exif Private IFD", Note = "" } },
            { new TiffTagInfo { IdInteger = 37385, IdString = "Flash", ShortDescription = "Indicates the status of flash when the image was shot.", SourceOfTag = " Exif Private IFD", Note = "" } },
            { new TiffTagInfo { IdInteger = 37386, IdString = "FocalLength", ShortDescription = "The actual focal length of the lens, in mm.", SourceOfTag = " Exif Private IFD", Note = "" } },
            { new TiffTagInfo { IdInteger = 37387, IdString = "FlashEnergy", ShortDescription = "Amount of flash energy (BCPS).", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 37388, IdString = "SpatialFrequencyResponse", ShortDescription = "SFR of the camera.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 37389, IdString = "Noise", ShortDescription = "Noise measurement values.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 37390, IdString = "FocalPlaneXResolution", ShortDescription = "Number of pixels per FocalPlaneResolutionUnit (37392) in ImageWidth direction for main image.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 37391, IdString = "FocalPlaneYResolution", ShortDescription = "Number of pixels per FocalPlaneResolutionUnit (37392) in ImageLength direction for main image.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 37392, IdString = "FocalPlaneResolutionUnit", ShortDescription = "Unit of measurement for FocalPlaneXResolution(37390) and FocalPlaneYResolution(37391).", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 37393, IdString = "ImageNumber", ShortDescription = "Number assigned to an image, e.g., in a chained image burst.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 37394, IdString = "SecurityClassification", ShortDescription = "Security classification assigned to the image.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 37395, IdString = "ImageHistory", ShortDescription = "Record of what has been done to the image.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 37396, IdString = "SubjectLocation", ShortDescription = "Indicates the location and area of the main subject in the overall scene.", SourceOfTag = " Exif Private IFD", Note = "" } },
            { new TiffTagInfo { IdInteger = 37397, IdString = "ExposureIndex", ShortDescription = "Encodes the camera exposure index setting when image was captured.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 37398, IdString = "TIFF/EPStandardID", ShortDescription = "For current spec, tag value equals 1 0 0 0.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 37399, IdString = "SensingMethod", ShortDescription = "Type of image sensor.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 37500, IdString = "MakerNote", ShortDescription = "Manufacturer specific information.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 37510, IdString = "UserComment", ShortDescription = "Keywords or comments on the image; complements ImageDescription.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 37520, IdString = "SubsecTime", ShortDescription = "A tag used to record fractions of seconds for the DateTime tag.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 37521, IdString = "SubsecTimeOriginal", ShortDescription = "A tag used to record fractions of seconds for the DateTimeOriginal tag.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 37522, IdString = "SubsecTimeDigitized", ShortDescription = "A tag used to record fractions of seconds for the DateTimeDigitized tag.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 37724, IdString = "ImageSourceData", ShortDescription = "Used by Adobe Photoshop.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 40091, IdString = "XPTitle", ShortDescription = "Title tag used by Windows, encoded in UCS2", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 40092, IdString = "XPComment", ShortDescription = "Comment tag used by Windows, encoded in UCS2", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 40093, IdString = "XPAuthor", ShortDescription = "Author tag used by Windows, encoded in UCS2", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 40094, IdString = "XPKeywords", ShortDescription = "Keywords tag used by Windows, encoded in UCS2", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 40095, IdString = "XPSubject", ShortDescription = "Subject tag used by Windows, encoded in UCS2", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 40960, IdString = "FlashpixVersion", ShortDescription = "The Flashpix format version supported by a FPXR file.", SourceOfTag = " Exif Private IFD", Note = "Mandatory in the Exif IFD" } },
            { new TiffTagInfo { IdInteger = 40961, IdString = "ColorSpace", ShortDescription = "The color space information tag is always recorded as the color space specifier.", SourceOfTag = " Exif Private IFD", Note = "Mandatory in the Exif IFD" } },
            { new TiffTagInfo { IdInteger = 40962, IdString = "PixelXDimension", ShortDescription = "Specific to compressed data; the valid width of the meaningful image.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 40963, IdString = "PixelYDimension", ShortDescription = "Specific to compressed data; the valid height of the meaningful image.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 40964, IdString = "RelatedSoundFile", ShortDescription = "Used to record the name of an audio file related to the image data.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 40965, IdString = "Interoperability IFD", ShortDescription = "A pointer to the Exif-related Interoperability IFD.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 41483, IdString = "FlashEnergy", ShortDescription = "Indicates the strobe energy at the time the image is captured, as measured in Beam Candle Power Seconds", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 41484, IdString = "SpatialFrequencyResponse", ShortDescription = "Records the camera or input device spatial frequency table and SFR values in the direction of image width, image height, and diagonal direction, as specified in ISO 12233.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 41486, IdString = "FocalPlaneXResolution", ShortDescription = "Indicates the number of pixels in the image width (X) direction per FocalPlaneResolutionUnit on the camera focal plane.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 41487, IdString = "FocalPlaneYResolution", ShortDescription = "Indicates the number of pixels in the image height (Y) direction per FocalPlaneResolutionUnit on the camera focal plane.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 41488, IdString = "FocalPlaneResolutionUnit", ShortDescription = "Indicates the unit for measuring FocalPlaneXResolution and FocalPlaneYResolution.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 41492, IdString = "SubjectLocation", ShortDescription = "Indicates the location of the main subject in the scene.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 41493, IdString = "ExposureIndex", ShortDescription = "Indicates the exposure index selected on the camera or input device at the time the image is captured.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 41495, IdString = "SensingMethod", ShortDescription = "Indicates the image sensor type on the camera or input device.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 41728, IdString = "FileSource", ShortDescription = "Indicates the image source.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 41729, IdString = "SceneType", ShortDescription = "Indicates the type of scene.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 41730, IdString = "CFAPattern", ShortDescription = "Indicates the color filter array (CFA) geometric pattern of the image sensor when a one-chip color area sensor is used.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 41985, IdString = "CustomRendered", ShortDescription = "Indicates the use of special processing on image data, such as rendering geared to output.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 41986, IdString = "ExposureMode", ShortDescription = "Indicates the exposure mode set when the image was shot.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 41987, IdString = "WhiteBalance", ShortDescription = "Indicates the white balance mode set when the image was shot.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 41988, IdString = "DigitalZoomRatio", ShortDescription = "Indicates the digital zoom ratio when the image was shot.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 41989, IdString = "FocalLengthIn35mmFilm", ShortDescription = "Indicates the equivalent focal length assuming a 35mm film camera, in mm.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 41990, IdString = "SceneCaptureType", ShortDescription = "Indicates the type of scene that was shot.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 41991, IdString = "GainControl", ShortDescription = "Indicates the degree of overall image gain adjustment.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 41992, IdString = "Contrast", ShortDescription = "Indicates the direction of contrast processing applied by the camera when the image was shot.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 41993, IdString = "Saturation", ShortDescription = "Indicates the direction of saturation processing applied by the camera when the image was shot.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 41994, IdString = "Sharpness", ShortDescription = "Indicates the direction of sharpness processing applied by the camera when the image was shot.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 41995, IdString = "DeviceSettingDescription", ShortDescription = "This tag indicates information on the picture-taking conditions of a particular camera model.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 41996, IdString = "SubjectDistanceRange", ShortDescription = "Indicates the distance to the subject.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 42016, IdString = "ImageUniqueID", ShortDescription = "Indicates an identifier assigned uniquely to each image.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 42032, IdString = "CameraOwnerName", ShortDescription = "Camera owner name as ASCII string.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 42033, IdString = "BodySerialNumber", ShortDescription = "Camera body serial number as ASCII string.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 42034, IdString = "LensSpecification", ShortDescription = "This tag notes minimum focal length, maximum focal length, minimum F number in the minimum focal length, and minimum F number in the maximum focal length, which are specification information for the lens that was used in photography. When the minimum F number is unknown, the notation is 0/0.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 42035, IdString = "LensMake", ShortDescription = "Lens manufacturer name as ASCII string.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 42036, IdString = "LensModel", ShortDescription = "Lens model name and number as ASCII string.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 42037, IdString = "LensSerialNumber", ShortDescription = "Lens serial number as ASCII string.", SourceOfTag = " Exif Private IFD", Note = " " } },
            
            
            // GeoTiff-specific, added by GDAL
            { new TiffTagInfo { IdInteger = 42112, IdString = "GDAL_METADATA", ShortDescription = "Used by the GDAL library, holds an XML list of name=value 'metadata' values about the image as a whole, and about specific samples.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 42113, IdString = "GDAL_NODATA", ShortDescription = "Used by the GDAL library, contains an ASCII encoded nodata or background pixel value.", SourceOfTag = " Private", Note = " " } },


            { new TiffTagInfo { IdInteger = 48129, IdString = "PixelFormat", ShortDescription = "A 128-bit Globally Unique Identifier (GUID) that identifies the image pixel format.", SourceOfTag = " HD Photo Feature Spec, p. 17", Note = " " } },
            { new TiffTagInfo { IdInteger = 48130, IdString = "Transformation", ShortDescription = "Specifies the transformation to be applied when decoding the image to present the desired representation.", SourceOfTag = " HD Photo Feature Spec, p. 23", Note = " " } },
            { new TiffTagInfo { IdInteger = 48131, IdString = "Uncompressed", ShortDescription = "Specifies that image data is uncompressed.", SourceOfTag = " HD Photo Feature Spec, p. 23", Note = " " } },
            { new TiffTagInfo { IdInteger = 48132, IdString = "ImageType", ShortDescription = "Specifies the image type of each individual frame in a multi-frame file.", SourceOfTag = " HD Photo Feature Spec, p. 27", Note = " " } },
            { new TiffTagInfo { IdInteger = 48256, IdString = "ImageWidth", ShortDescription = "Specifies the number of columns in the transformed photo, or the number of pixels per scan line.", SourceOfTag = " HD Photo Feature Spec, p. 21", Note = " " } },
            { new TiffTagInfo { IdInteger = 48257, IdString = "ImageHeight", ShortDescription = "Specifies the number of pixels or scan lines in the transformed photo.", SourceOfTag = " HD Photo Feature Spec, p. 21", Note = " " } },
            { new TiffTagInfo { IdInteger = 48258, IdString = "WidthResolution", ShortDescription = "Specifies the horizontal resolution of a transformed image expressed in pixels per inch.", SourceOfTag = " HD Photo Feature Spec, p. 21", Note = " " } },
            { new TiffTagInfo { IdInteger = 48259, IdString = "HeightResolution", ShortDescription = "Specifies the vertical resolution of a transformed image expressed in pixels per inch.", SourceOfTag = " HD Photo Feature Spec, p. 21", Note = " " } },
            { new TiffTagInfo { IdInteger = 48320, IdString = "ImageOffset", ShortDescription = "Specifies the byte offset pointer to the beginning of the photo data, relative to the beginning of the file.", SourceOfTag = " HD Photo Feature Spec, p. 22", Note = " " } },
            { new TiffTagInfo { IdInteger = 48321, IdString = "ImageByteCount", ShortDescription = "Specifies the size of the photo in bytes.", SourceOfTag = " HD Photo Feature Spec, p. 22", Note = " " } },
            { new TiffTagInfo { IdInteger = 48322, IdString = "AlphaOffset", ShortDescription = "Specifies the byte offset pointer the beginning of the planar alpha channel data, relative to the beginning of the file.", SourceOfTag = " HD Photo Feature Spec, p. 22", Note = " " } },
            { new TiffTagInfo { IdInteger = 48323, IdString = "AlphaByteCount", ShortDescription = "Specifies the size of the alpha channel data in bytes.", SourceOfTag = " HD Photo Feature Spec, p. 23", Note = " " } },
            { new TiffTagInfo { IdInteger = 48324, IdString = "ImageDataDiscard", ShortDescription = "Signifies the level of data that has been discarded from the image as a result of a compressed domain transcode to reduce the file size.", SourceOfTag = " HD Photo Feature Spec, p. 25", Note = " " } },
            { new TiffTagInfo { IdInteger = 48325, IdString = "AlphaDataDiscard", ShortDescription = "Signifies the level of data that has been discarded from the planar alpha channel as a result of a compressed domain transcode to reduce the file size.", SourceOfTag = " HD Photo Feature Spec, p. 26", Note = " " } },
            { new TiffTagInfo { IdInteger = 50215, IdString = "Oce Scanjob Description", ShortDescription = "Used in the Oce scanning process.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 50216, IdString = "Oce Application Selector", ShortDescription = "Used in the Oce scanning process.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 50217, IdString = "Oce Identification Number", ShortDescription = "Used in the Oce scanning process.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 50218, IdString = "Oce ImageLogic Characteristics", ShortDescription = "Used in the Oce scanning process.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 50341, IdString = "PrintImageMatching", ShortDescription = "Description needed.", SourceOfTag = " Exif Private IFD", Note = " " } },
            { new TiffTagInfo { IdInteger = 50706, IdString = "DNGVersion", ShortDescription = "Encodes DNG four-tier version number; for version 1.1.0.0, the tag contains the bytes 1, 1, 0, 0. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50707, IdString = "DNGBackwardVersion", ShortDescription = "Defines oldest version of spec with which file is compatible. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50708, IdString = "UniqueCameraModel", ShortDescription = "Unique, non-localized nbame for camera model. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50709, IdString = "LocalizedCameraModel", ShortDescription = "Similar to 50708, with localized camera name. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50710, IdString = "CFAPlaneColor", ShortDescription = "Mapping between values in the CFAPattern tag and the plane numbers in LinearRaw space. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50711, IdString = "CFALayout", ShortDescription = "Spatial layout of the CFA. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50712, IdString = "LinearizationTable", ShortDescription = "Lookup table that maps stored values to linear values. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50713, IdString = "BlackLevelRepeatDim", ShortDescription = "Repeat pattern size for BlackLevel tag. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50714, IdString = "BlackLevel", ShortDescription = "Specifies the zero light encoding level.Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50715, IdString = "BlackLevelDeltaH", ShortDescription = "Specifies the difference between zero light encoding level for each column and the baseline zero light encoding level. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50716, IdString = "BlackLevelDeltaV", ShortDescription = "Specifies the difference between zero light encoding level for each row and the baseline zero light encoding level. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50717, IdString = "WhiteLevel", ShortDescription = "Specifies the fully saturated encoding level for the raw sample values. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50718, IdString = "DefaultScale", ShortDescription = "For cameras with non-square pixels, specifies the default scale factors for each direction to convert the image to square pixels. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50719, IdString = "DefaultCropOrigin", ShortDescription = "Specifies the origin of the final image area, ignoring the extra pixels at edges used to prevent interpolation artifacts. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50720, IdString = "DefaultCropSize", ShortDescription = "Specifies size of final image area in raw image coordinates. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50721, IdString = "ColorMatrix1", ShortDescription = "Defines a transformation matrix that converts XYZ values to reference camera native color space values, under the first calibration illuminant. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50722, IdString = "ColorMatrix2", ShortDescription = "Defines a transformation matrix that converts XYZ values to reference camera native color space values, under the second calibration illuminant. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50723, IdString = "CameraCalibration1", ShortDescription = "Defines a calibration matrix that transforms reference camera native space values to individual camera native space values under the first calibration illuminant. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50724, IdString = "CameraCalibration2", ShortDescription = "Defines a calibration matrix that transforms reference camera native space values to individual camera native space values under the second calibration illuminant. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50725, IdString = "ReductionMatrix1", ShortDescription = "Defines a dimensionality reduction matrix for use as the first stage in converting color camera native space values to XYZ values, under the first calibration illuminant. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50726, IdString = "ReductionMatrix2", ShortDescription = "Defines a dimensionality reduction matrix for use as the first stage in converting color camera native space values to XYZ values, under the second calibration illuminant. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50727, IdString = "AnalogBalance", ShortDescription = "Pertaining to white balance, defines the gain, either analog or digital, that has been applied to the stored raw values. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50728, IdString = "AsShotNeutral", ShortDescription = "Specifies the selected white balance at the time of capture, encoded as the coordinates of a perfectly neutral color in linear reference space values. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50729, IdString = "AsShotWhiteXY", ShortDescription = "Specifies the selected white balance at the time of capture, encoded as x-y chromaticity coordinates. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50730, IdString = "BaselineExposure", ShortDescription = "Specifies in EV units how much to move the zero point for exposure compensation. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50731, IdString = "BaselineNoise", ShortDescription = "Specifies the relative noise of the camera model at a baseline ISO value of 100, compared to reference camera model. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50732, IdString = "BaselineSharpness", ShortDescription = "Specifies the relative amount of sharpening required for this camera model, compared to reference camera model. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50733, IdString = "BayerGreenSplit", ShortDescription = "For CFA images, specifies, in arbitrary units, how closely the values of the green pixels in the blue/green rows track the values of the green pixels in the red/green rows. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50734, IdString = "LinearResponseLimit", ShortDescription = "Specifies the fraction of the encoding range above which the response may become significantly non-linear. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50735, IdString = "CameraSerialNumber", ShortDescription = "Serial number of camera. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50736, IdString = "LensInfo", ShortDescription = "Information about the lens. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50737, IdString = "ChromaBlurRadius", ShortDescription = "Normally for non-CFA images, provides a hint about how much chroma blur ought to be applied. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50738, IdString = "AntiAliasStrength", ShortDescription = "Provides a hint about the strength of the camera's anti-aliasing filter. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50739, IdString = "ShadowScale", ShortDescription = "Used by Adobe Camera Raw to control sensitivity of its shadows slider. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50740, IdString = "DNGPrivateData", ShortDescription = "Provides a way for camera manufacturers to store private data in DNG files for use by their own raw convertors. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50741, IdString = "MakerNoteSafety", ShortDescription = "Lets the DNG reader know whether the Exif MakerNote tag is safe to preserve. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50778, IdString = "CalibrationIlluminant1", ShortDescription = "Illuminant used for first set of calibration tags. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50779, IdString = "CalibrationIlluminant2", ShortDescription = "Illuminant used for second set of calibration tags. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50780, IdString = "BestQualityScale", ShortDescription = "Specifies the amount by which the values of the DefaultScale tag need to be multiplied to achieve best quality image size. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50781, IdString = "RawDataUniqueID", ShortDescription = "Contains a 16-byte unique identifier for the raw image file in the DNG file. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50784, IdString = "Alias Layer Metadata", ShortDescription = "Alias Sketchbook Pro layer usage description.", SourceOfTag = " Private", Note = " " } },
            { new TiffTagInfo { IdInteger = 50827, IdString = "OriginalRawFileName", ShortDescription = "Name of original file if the DNG file results from conversion from a non-DNG raw file. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50828, IdString = "OriginalRawFileData", ShortDescription = "If the DNG file was converted from a non-DNG raw file, then this tag contains the original raw data. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50829, IdString = "ActiveArea", ShortDescription = "Defines the active (non-masked) pixels of the sensor. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50830, IdString = "MaskedAreas", ShortDescription = "List of non-overlapping rectangle coordinates of fully masked pixels, which can optimally be used by DNG readers to measure the black encoding level. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50831, IdString = "AsShotICCProfile", ShortDescription = "Contains ICC profile that, in conjunction with the AsShotPreProfileMatrix tag, specifies a default color rendering from camera color space coordinates (linear reference values) into the ICC profile connection space. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50832, IdString = "AsShotPreProfileMatrix", ShortDescription = "Specifies a matrix that should be applied to the camera color space coordinates before processing the values through the ICC profile specified in the AsShotICCProfile tag. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50833, IdString = "CurrentICCProfile", ShortDescription = "The CurrentICCProfile and CurrentPreProfileMatrix tags have the same purpose and usage as", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50834, IdString = "CurrentPreProfileMatrix", ShortDescription = "The CurrentICCProfile and CurrentPreProfileMatrix tags have the same purpose and usage as", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50879, IdString = "ColorimetricReference", ShortDescription = "The DNG color model documents a transform between camera colors and CIE XYZ values. This tag describes the colorimetric reference for the CIE XYZ values. 0 = The XYZ values are scene-referred. 1 = The XYZ values are output-referred, using the ICC profile perceptual dynamic range. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50931, IdString = "CameraCalibrationSignature", ShortDescription = "A UTF-8 encoded string associated with the CameraCalibration1 and CameraCalibration2 tags. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50932, IdString = "ProfileCalibrationSignature", ShortDescription = "A UTF-8 encoded string associated with the camera profile tags. Used in IFD 0 or CameraProfile IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50933, IdString = "ExtraCameraProfiles", ShortDescription = "A list of file offsets to extra Camera Profile IFDs. The format of a camera profile begins with a 16-bit byte order mark (MM or II) followed by a 16-bit \"magic\" number equal to 0x4352 (CR), a 32-bit IFD offset, and then a standard TIFF format IFD. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50935, IdString = "NoiseReductionApplied", ShortDescription = "Indicates how much noise reduction has been applied to the raw data on a scale of 0.0 to 1.0. A 0.0 value indicates that no noise reduction has been applied. A 1.0 value indicates that the \"ideal\" amount of noise reduction has been applied, i.e. that the DNG reader should not apply additional noise reduction by default. A value of 0/0 indicates that this parameter is unknown. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50936, IdString = "ProfileName", ShortDescription = "A UTF-8 encoded string containing the name of the camera profile. Used in IFD 0 or Camera Profile IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50937, IdString = "ProfileHueSatMapDims", ShortDescription = "Specifies the number of input samples in each dimension of the hue/saturation/value mapping tables. The data for these tables are stored in ProfileHueSatMapData1 and ProfileHueSatMapData2 tags. Allowed values include the following: HueDivisions >= 1; SaturationDivisions >= 2; ValueDivisions >=1. Used in IFD 0 or Camera Profile IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50938, IdString = "ProfileHueSatMapData1", ShortDescription = "Contains the data for the first hue/saturation/value mapping table. Each entry of the table contains three 32-bit IEEE floating-point values. The first entry is hue shift in degrees; the second entry is saturation scale factor; and the third entry is a value scale factor. Used in IFD 0 or Camera Profile IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50939, IdString = "ProfileHueSatMapData2", ShortDescription = "Contains the data for the second hue/saturation/value mapping table. Each entry of the table contains three 32-bit IEEE floating-point values. The first entry is hue shift in degrees; the second entry is saturation scale factor; and the third entry is a value scale factor. Used in IFD 0 or Camera Profile IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50940, IdString = "ProfileToneCurve", ShortDescription = "Contains a default tone curve that can be applied while processing the image as a starting point for user adjustments. The curve is specified as a list of 32-bit IEEE floating-point value pairs in linear gamma. Each sample has an input value in the range of 0.0 to 1.0, and an output value in the range of 0.0 to 1.0. The first sample is required to be (0.0, 0.0), and the last sample is required to be (1.0, 1.0). Interpolated the curve using a cubic spline. Used in IFD 0 or Camera Profile IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50941, IdString = "ProfileEmbedPolicy", ShortDescription = "Contains information about the usage rules for the associated camera profile. The valid values and meanings are: 0 = ΓÇ£allow copyingΓÇ¥; 1 = ΓÇ£embed if usedΓÇ¥; 2 = ΓÇ£embed neverΓÇ¥; and 3 = ΓÇ£no restrictionsΓÇ¥. Used in IFD 0 or Camera Profile IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50942, IdString = "ProfileCopyright", ShortDescription = "Contains information about the usage rules for the associated camera profile. The valid values and meanings are: 0 = ΓÇ£allow copyingΓÇ¥; 1 = ΓÇ£embed if usedΓÇ¥; 2 = ΓÇ£embed neverΓÇ¥; and 3 = ΓÇ£no restrictionsΓÇ¥. Used in IFD 0 or Camera Profile IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50964, IdString = "ForwardMatrix1", ShortDescription = "Defines a matrix that maps white balanced camera colors to XYZ D50 colors. Used in IFD 0 or Camera Profile IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50965, IdString = "ForwardMatrix2", ShortDescription = "Defines a matrix that maps white balanced camera colors to XYZ D50 colors. Used in IFD 0 or Camera Profile IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50966, IdString = "PreviewApplicationName", ShortDescription = "A UTF-8 encoded string containing the name of the application that created the preview stored in the IFD. Used in Preview IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50967, IdString = "PreviewApplicationVersion", ShortDescription = "A UTF-8 encoded string containing the version number of the application that created the preview stored in the IFD. Used in Preview IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50968, IdString = "PreviewSettingsName", ShortDescription = "A UTF-8 encoded string containing the name of the conversion settings (for example, snapshot name) used for the preview stored in the IFD. Used in Preview IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50969, IdString = "PreviewSettingsDigest", ShortDescription = "A unique ID of the conversion settings (for example, MD5 digest) used to render the preview stored in the IFD. Used in Preview IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50970, IdString = "PreviewColorSpace", ShortDescription = "This tag specifies the color space in which the rendered preview in this IFD is stored. The valid values include: 0 = Unknown; 1 = Gray Gamma 2.2; 2 = sRGB; 3 = Adobe RGB; and 4 = ProPhoto RGB. Used in Preview IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50971, IdString = "PreviewDateTime", ShortDescription = "This tag is an ASCII string containing the name of the date/time at which the preview stored in the IFD was rendered, encoded using ISO 8601 format. Used in Preview IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50972, IdString = "RawImageDigest", ShortDescription = "MD5 digest of the raw image data. All pixels in the image are processed in row-scan order. Each pixel is zero padded to 16 or 32 bits deep (16-bit for data less than or equal to 16 bits deep, 32-bit otherwise). The data is processed in little-endian byte order. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50973, IdString = "OriginalRawFileDigest", ShortDescription = "MD5 digest of the data stored in the OriginalRawFileData tag. Used in IFD 0 of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50974, IdString = "SubTileBlockSize", ShortDescription = "Normally, pixels within a tile are stored in simple row-scan order. This tag specifies that the pixels within a tile should be grouped first into rectangular blocks of the specified size. These blocks are stored in row-scan order. Within each block, the pixels are stored in row-scan order. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50975, IdString = "RowInterleaveFactor", ShortDescription = "Specifies that rows of the image are stored in interleaved order. The value of the tag specifies the number of interleaved fields. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50981, IdString = "ProfileLookTableDims", ShortDescription = "Specifies the number of input samples in each dimension of a default \"look\" table. The data for this table is stored in the ProfileLookTableData tag. Allowed values include: HueDivisions >= 1; SaturationDivisions >= 2; and ValueDivisions >= 1. Used in IFD 0 or Camera Profile IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 50982, IdString = "ProfileLookTableData", ShortDescription = "Default \"look\" table that can be applied while processing the image as a starting point for user adjustment. This table uses the same format as the tables stored in the ProfileHueSatMapData1 and ProfileHueSatMapData2 tags, and is applied in the same color space. However, it should be applied later in the processing pipe, after any exposure compensation and/or fill light stages, but before any tone curve stage. Each entry of the table contains three 32-bit IEEE floating-point values. The first entry is hue shift in degrees, the second entry is a saturation scale factor, and the third entry is a value scale factor. Used in IFD 0 or Camera Profile IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 51008, IdString = "OpcodeList1", ShortDescription = "Specifies the list of opcodes (image processing operation codes) that should be applied to the raw image, as read directly from the file. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 51009, IdString = "OpcodeList2", ShortDescription = "Specifies the list of opcodes (image processing operation codes) that should be applied to the raw image, just after it has been mapped to linear reference values. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 51022, IdString = "OpcodeList3", ShortDescription = "Specifies the list of opcodes (image processing operation codes) that should be applied to the raw image, just after it has been demosaiced. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 51041, IdString = "NoiseProfile", ShortDescription = "Describes the amount of noise in a raw image; models the amount of signal-dependent photon (shot) noise and signal-independent sensor readout noise, two common sources of noise in raw images. Used in Raw IFD of DNG files.", SourceOfTag = " ", Note = "" } },
            { new TiffTagInfo { IdInteger = 51089, IdString = "OriginalDefaultFinalSize", ShortDescription = "If this file is a proxy for a larger original DNG file, this tag specifics the default final size of the larger original file from which this proxy was generated. The default value for this tag is default final size of the current DNG file, which is DefaultCropSize * DefaultScale.", SourceOfTag = " DNG spec (1.4, 2012), p. 74", Note = " " } },
            { new TiffTagInfo { IdInteger = 51090, IdString = "OriginalBestQualityFinalSize", ShortDescription = "If this file is a proxy for a larger original DNG file, this tag specifics the best quality final size of the larger original file from which this proxy was generated. The default value for this tag is the OriginalDefaultFinalSize, if specified. Otherwise the default value for this tag is the best quality size of the current DNG file, which is DefaultCropSize * DefaultScale * BestQualityScale.", SourceOfTag = " DNG spec (1.4, 2012), p. 75", Note = " " } },
            { new TiffTagInfo { IdInteger = 51091, IdString = "OriginalDefaultCropSize", ShortDescription = "If this file is a proxy for a larger original DNG file, this tag specifics the DefaultCropSize of the larger original file from which this proxy was generated. The default value for this tag is the OriginalDefaultFinalSize, if specified. Otherwise, the default value for this tag is the DefaultCropSize of the current DNG file.", SourceOfTag = " DNG spec (1.4, 2012), p. 75", Note = " " } },
            { new TiffTagInfo { IdInteger = 51107, IdString = "ProfileHueSatMapEncoding", ShortDescription = "Provides a way for color profiles to specify how indexing into a 3D HueSatMap is performed during raw conversion. This tag is not applicable to 2.5D HueSatMap tables (i.e., where the Value dimension is 1). The currently defined values are: 0 = Linear encoding (method described in DNG spec); 1 = sRGB encoding (method described in DNG spec).", SourceOfTag = " DNG spec (1.4, 2012), p. 73", Note = " " } },
            { new TiffTagInfo { IdInteger = 51108, IdString = "ProfileLookTableEncoding", ShortDescription = "Provides a way for color profiles to specify how indexing into a 3D LookTable is performed during raw conversion. This tag is not applicable to a 2.5D LookTable (i.e., where the Value dimension is 1). The currently defined values are: 0 = Linear encoding (method described in DNG spec); 1 = sRGB encoding (method described in DNG spec).", SourceOfTag = " DNG spec (1.4, 2012), p. 72-3", Note = " " } },
            { new TiffTagInfo { IdInteger = 51109, IdString = "BaselineExposureOffset", ShortDescription = "Provides a way for color profiles to increase or decrease exposure during raw conversion. BaselineExposureOffset specifies the amount (in EV units) to add to th e BaselineExposure tag during image rendering. For example, if the BaselineExposure value fo r a given camera model is +0.3, and the BaselineExposureOffset value for a given camera profile used to render an image for that camera model is -0.7, then th e actual default exposure value used during rendering will be +0.3 - 0.7 = -0.4.", SourceOfTag = " DNG spec (1.4, 2012), p. 71", Note = " " } },
            { new TiffTagInfo { IdInteger = 51110, IdString = "DefaultBlackRender", ShortDescription = "This optional tag in a color profile provides a hint to the raw converter regarding how to handle the black point (e.g., flare subtraction) during rendering. The currently defined values are: 0 = Auto; 1 = None. If set to Auto, the raw converter should perform black subtraction during rendering. The amount and method of black subtraction may be automatically determined and may be image-dependent. If set to None, the raw converter should not perform any black subtraction during rendering. This may be desirable when using color lookup tables (e.g., LookTable) or tone curves in camera profiles to perform a fixed, consistent level of black subtraction.", SourceOfTag = " DNG spec (1.4, 2012), p. 71", Note = " " } },
            { new TiffTagInfo { IdInteger = 51111, IdString = "NewRawImageDigest", ShortDescription = "This tag is a modified MD5 digest of the raw image data. It has been updated from the algorithm used to compute the RawImageDigest tag be more multi-processor friendly, and to support lossy compression algorithms. The details of the algorithm used to compute this tag are documented in the Adobe DNG SDK source code.", SourceOfTag = " DNG spec (1.4, 2012), p. 76", Note = " " } },
            { new TiffTagInfo { IdInteger = 51112, IdString = "RawToPreviewGain", ShortDescription = "The gain (what number the sample values are multiplied by) between the main raw IFD and the preview IFD containing this tag.", SourceOfTag = " DNG spec (1.4, 2012), p. 76", Note = " " } },
            { new TiffTagInfo { IdInteger = 51125, IdString = "DefaultUserCrop", ShortDescription = "Specifies a default user crop rectangle in relative coordinates. The values must satisfy: 0.0 <= top < bottom <= 1.0; 0.0 <= left < right <= 1.0. The default values of (top = 0, left = 0, bottom = 1, right = 1) correspond exactly to the default crop rectangle (as specified by the DefaultCropOrigin and DefaultCropSize tags).", SourceOfTag = " DNG spec (1.4, 2012), p. 70", Note = " " } },




        };
    }
}
