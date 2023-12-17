using BitMiracle.LibTiff.Classic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace GeoTBelt.GeoTiff
{
    internal class GeoTiffHelper
    {
        #region TIFF Tags
        private static readonly TiffTag TIFFTAG_GEOTIFFTAGS = (TiffTag)42112;
        private static readonly TiffTag TIFFTAG_GEOPIXELSCALE = (TiffTag)33550;
        private static readonly TiffTag TIFFTAG_GEOTIEPOINTS = (TiffTag)33922;
        private static readonly TiffTag TIFFTAG_GEOKEYDIRECTORY = (TiffTag)34735;
        private static readonly TiffTag TIFFTAG_GEODOUBLEPARAMS = (TiffTag)34736;
        private static readonly TiffTag TIFFTAG_GEOASCIIPARAMS = (TiffTag)34737;
        private static readonly TiffTag TIFFTAG_GDAL_METADATA = (TiffTag)42112;  // Need
        private static readonly TiffTag TIFFTAG_GDAL_NODATA = (TiffTag)42113;    // Need
        private static readonly TiffTag TIFFTAG_GDAL_NODATA1 = (TiffTag)42113;
        private static readonly TiffTag TIFFTAG_GDAL_NODATA2 = (TiffTag)42114;
        private static readonly TiffTag TIFFTAG_GDAL_NODATA3 = (TiffTag)42115;
        private static readonly TiffTag TIFFTAG_GDAL_NODATA4 = (TiffTag)42116;
        private static readonly TiffTag TIFFTAG_GDAL_METADATA1 = (TiffTag)42112;
        private static readonly TiffTag TIFFTAG_GDAL_METADATA24 = (TiffTag)42135;
        // Note: I don't know how many metadatas there are. - Paul S.
        private static readonly TiffTag TIFFTAG_RESOLUTIONUNIT = (TiffTag)296;
        private static readonly TiffTag TIFFTAG_XRESOLUTION = (TiffTag)282;
        private static readonly TiffTag TIFFTAG_YRESOLUTION = (TiffTag)283;
        private static readonly TiffTag TIFFTAG_GEOTRANSMATRIX = (TiffTag)34264;
        private static readonly TiffTag TIFFTAG_GEOKEYDIRECTORY1 = (TiffTag)34735;
        private static readonly TiffTag TIFFTAG_GEOKEYDIRECTORY2 = (TiffTag)34736;
        private static readonly TiffTag TIFFTAG_GEOKEYDIRECTORY3 = (TiffTag)34737;
        private static readonly TiffTag TIFFTAG_GEOTIFFPCSDATUM = (TiffTag)34741;
        private static readonly TiffTag TIFFTAG_GEOTIFFPCSNAME = (TiffTag)34742;
        private static readonly TiffTag TIFFTAG_GEOTIFFPROJECTION = (TiffTag)34737;
        private static readonly TiffTag TIFFTAG_GEOTIFFPROXY = (TiffTag)34738;
        private static readonly TiffTag TIFFTAG_GEOKEYS = (TiffTag)34736;
        private static readonly TiffTag TIFFTAG_MODELPIXELSCALETAG = (TiffTag)33550;

        // Either one or the other of the next two, but not both.
        private static readonly TiffTag TIFFTAG_MODELTIEPOINTTAG = (TiffTag)33922;
        private static readonly TiffTag TIFFTAG_MODELTRANSFORMATIONTAG = (TiffTag)34264;

        private static readonly TiffTag TIFFTAG_GEOASCII_PARAMS = (TiffTag)34737;
        private static readonly TiffTag TIFFTAG_GEOTIFFPHOTOMETRICINTERPRETATIONTAG =
            (TiffTag)33922;
        private static readonly TiffTag TIFFTAG_GEOTIFFGEOGRAPHICTYPEGEOKEY = (TiffTag)34737;
        #endregion TIFF Tags

        //TiffTag.DATATYPE;     // 33920
        //TiffTag.GEOTIEPOINTS;     // 33922
        //TiffTag.GEOTRANSMATRIX;   // 34264
        //TiffTag.GEOTIFFDIRECTORY; // 34735
        //TiffTag.GEOTIFFDOUBLEPARAMS; // 34736
        //TiffTag.GEOTIFFASCIIPARAMS;  // 34737
        //TiffTag.GEOTIFFVERSION;      // 34737
        //TiffTag.GEOTIFFPROXY;        // 34738
        //TiffTag.GEOTIFFIPCORE;       // 34739
        //TiffTag.GEOTIFFMETADATA;      // 42112
        //TiffTag.GEOTIFFPCSGEOKEYDIRECTORY;  // 34740
        //TiffTag.GEOTIFFPCSDATUM;      // 34741
        //TiffTag.GEOTIFFPCSNAME;      // 34742
        //TiffTag.GEOTIFFPCSCITATION;  // 34743
        //TiffTag.GEOTIFFGEOKEYDIRECTORY;  // 34735
        //TiffTag.GEOTIFFGEOKEYS;       // 34736
        //TiffTag.GEOTIFFMODELPIXELSCALETAG;   // 33550
        //TiffTag.GEOTIFFMODELTIEPOINTTAG;  // 33922  // either this or MODELTRANSFORMATION
        //TiffTag.GEOTIFFMODELPARAMSTAG;   // 34736
        //TiffTag.GEOTIFFMODELTRANSFORMATIONTAG;  // 34264 // either this or MODELTIEPOINT
        //TiffTag.GEOTIFFPHOTOMETRICINTERPRETATIONTAG;   // 33922
        //TiffTag.GEOTIFFGEOGRAPHICTYPEGEOKEY;  // 34737



        public static Raster ReadGeoTiff(string fileToOpen)
        {
            Raster returnRaster = null;

            using (Tiff image = Tiff.Open(fileToOpen, "r"))
            {
                if (image == null) throw new Exception("Could not open file.");

                var tags = GetAllTags(image);

                returnRaster = new Raster();

                FieldValue[] value = image.GetField(TiffTag.IMAGEWIDTH);
                int width = value[0].ToInt();
                returnRaster.numColumns = width;

                value = image.GetField(TiffTag.IMAGELENGTH);
                int height = value[0].ToInt();
                returnRaster.numRows = height;

                int imageSize = height * width;

                Console.WriteLine($"ht: {height}    wd: {width}.");
                int[] raster = new int[imageSize];

                value = image.GetField(TIFFTAG_GDAL_NODATA);
                string intermediateString = value[1].ToString();
                returnRaster.NoDataValue = intermediateString
                    .Substring(0, intermediateString.Length - 1);

            }

            return returnRaster;
        }

        private dynamic? imageGetField(Tiff img, TiffTag tag)
        {
            FieldValue[] value = img.GetField(tag);
            if (value is null)
                return null;
            return value[0];
        }

        /// <summary>
        /// Attempts to get field value for all tags. The first one it finds which
        /// has a non-null value it returns and does not try the others.
        /// </summary>
        /// <param name="img">The tiff to look in.</param>
        /// <param name="tags">Ordered list of tags to try.</param>
        /// <returns></returns>
        private dynamic? imageGetFieldFromMultiple(Tiff img, List<TiffTag> tags)
        {
            dynamic? returnValue = null;
            foreach (TiffTag aTag in tags)
            {
                FieldValue[] value = img.GetField(aTag);
                if (value is null) continue;
                returnValue = value[0];
                if (returnValue is not null)
                    return returnValue;
            }
            return null;
        }

        private static Dictionary<string, dynamic?> GetAllTags(Tiff tif)
        {
            Dictionary<string, dynamic?> returnDict = new Dictionary<string, dynamic?>();

            #region diagnostics
            //string newline = Environment.NewLine;
            //StringBuilder debugSB = new StringBuilder("| ");
            //var rList = new List<string>();
            //StringBuilder sb = new StringBuilder();
            //for (ushort t = ushort.MinValue; t < ushort.MaxValue; ++t)
            //{
            //    TiffTag tag = (TiffTag)t;
            //    FieldValue[] value = tif.GetField(tag);
            //    if (value != null)
            //    {
            //        for (int j = 0; j < value.Length; j++)
            //        {
            //            sb.Append($"{tag.ToString()}   Type: ");
            //            sb.AppendLine(
            //                $"{value[j].Value.GetType().ToString()},  {value[j].ToString()}");
            //        }

            //        string s = sb.ToString().Trim();
            //        rList.Add(s);
            //        debugSB.Append($"{s}{newline}| ");
            //        sb.Clear();
            //    }
            //}
            #endregion diagnostics

            List<string> tagsInExistance = new List<string>();  // for diagnostics

            #region WhatAllTagsAreInThisFile
            var tagsInTheFile = new Dictionary<int, string>();

            List<GTBTifTag> retTags = new List<GTBTifTag>();
            int noCount = 0; int yesCount = 0;
            for (ushort t = ushort.MinValue; t < ushort.MaxValue; ++t)
            {
                string localName = AllTags.Tag(t)?.IdString;
                if(localName is null)
                {
                    noCount++;
                    continue;
                }
                else
                {
                    yesCount++;
                }

                tagsInExistance.Add( localName );

                bool tagIsInTheFile = GTBTifTag.IsThere(tif, t);
                if ( tagIsInTheFile )
                {
                    tagsInTheFile[t] = localName;
                }

            }
            #endregion WhatAllTagsAreInThisFile

            returnDict["ImageWidth"] = tif.GetAsInt("ImageWidth"); // column count
            returnDict["ImageLength"] = tif.GetAsInt("ImageLength"); // row count

            //"BitsPerSample"  Bit depth, limiting the type. For example, 8 bit
            //     means the type is byte. 16 bit means the type is ushort. etc.
            returnDict["BitsPerSample"] = tif.GetAsInt("BitsPerSample");

            //"Compression"   // Do this one later.

            //"PhotometricInterpretation"
            //"StripOffsets"

            //"SamplesPerPixel" The number of bands. (Surprise, ain't it.)
            returnDict["SamplesPerPixel"] = tif.GetAsInt("SamplesPerPixel");


            //"StripByteCounts"
            //"PlanarConfiguration"
            //"TileWidth"
            //"TileLength"
            //"TileOffsets"
            //"TileByteCounts"
            //"ExtraSamples"
            //"SampleFormat"

            #region Geospaital tags
            //"ModelPixelScaleTag"  Pixel height and width (and depth) in
            //      the original units and crs.
            // Type: double[]
            // value[0, 1, 2] contain pixel height, width, and depth.
            returnDict["ModelPixelScaleTag"] = tif.GetAsDoubleArray("ModelPixelScaleTag");

            //"ModelTiepointTag" Anchor point in the original units and crs.
            // Type: double[]
            // value[3, 4, 5] contain anchor xyz or lat/long/elev.
            returnDict["ModelTiepointTag"] = tif.GetAsDoubleArray("ModelTiepointTag");

            //"GeoKeyDirectoryTag"
            // Foundational spec at The Open Geospatial Consortium,
            //     http://www.opengeospatial.org/standards/geotiff
            //      Section 7.1.1 and following. Also see the bottom of this file.
            //      Further,
            // See http://geotiff.maptools.org/spec/geotiff2.4.html to understand this.
            // For more on this mess of bear skins and stone knives, see
            //      2.7.3 Cookbook for Geocoding Data
            //      at http://geotiff.maptools.org/spec/geotiff2.7.html#2.7

            int numColumns = 4;
            var geoTagKeys = tif.GetAsShortArray("GeoKeyDirectoryTag");
            int totalRowCount = geoTagKeys.Length / numColumns;
            var GeoKey_directory_Version = geoTagKeys[0];
            var majorRevision = geoTagKeys[1];  // key major revision number
            var minorRevision = geoTagKeys[2]; // key minor revision number
            var numKeysToFollow = geoTagKeys[3];  // number of keys following (4-column rows)

            List<dynamic> GeoKeyThingies = new List<dynamic>();

            for(int i = numColumns; i< geoTagKeys.Length; i+=4)
            {
                var keyID = geoTagKeys[i + 0];
                var TIFFTagLocation = geoTagKeys[i + 1];
                var count = geoTagKeys[i + 2];
                var value_Offset = geoTagKeys[i + 3];
                GeoKeyThingies.Add(new { 
                    keyID = keyID,  field2=TIFFTagLocation, field3=count,
                    field4 = value_Offset });
            }


            ExploreTiff(tif, "GeoKeyDirectoryTag");
            //returnDict["GeoKeyDirectoryTag"]

            //"GeoDoubleParamsTag"
            //"GeoAsciiParamsTag"

            //"GDAL_METADATA"  string type.
            returnDict["GDAL_METADATA"] = tif.GetAsString("GDAL_METADATA");
            var vvvvv = tif.GetField(TIFFTAG_GEOKEYS).GetType();

            //"GDAL_NODATA"
            returnDict["GDAL_NODATA"] = tif.GetAsInt("GDAL_NODATA");
            #endregion Geospaital tags

            return returnDict;
        }

        private static void ExploreTiff(Tiff tif, string varName)
        {
            int id = AllTags.Tag(varName).IdInteger;
            FieldValue[] value = tif.GetField((TiffTag)id);

            var internalLength = value[0];
            var testString = value[1]; //.ToString();
            var ljl = value.GetType(); // .ToString();

            Byte[] bytes;
            if (value != null && value.Length > 1 && value[1].Value is byte[])
            {
                bytes = (byte[])value[1].Value;
            }
            else
            {
                // Handle the case where the expected data is not present
            }
        }


    }

    public static class TifExtensionMethods
    {
        public static int? GetAsInt(this Tiff tif, string varName)
        {
            int id = AllTags.Tag(varName).IdInteger;
            FieldValue[] value = tif.GetField((TiffTag)id);

            if (value is null) return null;
            return (value[0]).ToInt();
        }

        public static string? GetAsString(this Tiff tif, string varName)
        {
            int id = AllTags.Tag(varName).IdInteger;
            FieldValue[] value = tif.GetField((TiffTag)id);

            if (value is null) return null;
            // string diagnosticString = value[1].ToString();
            return (value[1]).ToString();
        }

        public static short[]? GetAsShortArray(this Tiff tif, string varName)
        {
            int id = AllTags.Tag(varName).IdInteger;
            FieldValue[] value = tif.GetField((TiffTag)id);

            if (value is null) return null;

            int arraySize = value[0].ToInt();

            Byte[] byteArray;
            if (value != null && value.Length > 1 && value[1].Value is byte[])
            {
                byteArray = (byte[])value[1].Value;

                List<short> accumulator = new List<short>();
                int itemSize = sizeof(short);
                for (int i = 0; i < arraySize; i++)
                {
                    accumulator.Add(
                        BitConverter.ToInt16(byteArray, i * itemSize));
                }
                return accumulator.ToArray();
            }

            return null;
        }

        public static double[]? GetAsDoubleArray(this Tiff tif, string varName)
        {
            int id = AllTags.Tag(varName).IdInteger;
            FieldValue[] value = tif.GetField((TiffTag)id);

            if (value is null) return null;

            int arraySize = value[0].ToInt();

            Byte[] byteArray;
            if (value != null && value.Length > 1 && value[1].Value is byte[])
            {
                byteArray = (byte[])value[1].Value;

                List<double> accumulator = new List<double>();
                int itemSize = sizeof(double);
                for (int i = 0; i < arraySize; i++)
                {
                    accumulator.Add(
                        BitConverter.ToDouble(byteArray, i * itemSize));
                }
                return accumulator.ToArray();
            }

            return null;
        }
    }
}


/// Important text from https://docs.ogc.org/is/19-008r4/19-008r4.html
/// Section 7.1.2 about the GeoKeyDirectoryTag
/// A GeoTIFF file stores projection parameters in a set of "Keys" which are 
/// virtually identical in function to a TIFF tag, but have one more level of 
/// abstraction above TIFF. Like a tag, a Key has an ID number ranging from 0 
/// to 65535, but unlike TIFF tags, all key ID’s are available for use in 
/// GeoTIFF parameter definitions.
/// 
/// The Keys in GeoTIFF (also called "GeoKeys") are all referenced from the 
/// GeoKeyDirectoryTag tag.
/// 





