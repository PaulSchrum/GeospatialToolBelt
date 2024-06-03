using BitMiracle.LibTiff.Classic;
using GeoTBelt.Grid;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

        //TiffTag.DATATYPE;         // 33920
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

        public static GeoTiffRaster<T> ReadGeoTiff<T>(string fileToOpen)
            where T : struct
        {
            GeoTiffRaster<T> returnRaster = null;

            using (Tiff tifData = Tiff.Open(fileToOpen, "r"))
            {
                if (tifData == null) throw new Exception("Could not open file.");

                var tags = GetAllTags(tifData);

                returnRaster = new GeoTiffRaster<T>();

                #region raster base class items
                int width = tags["ImageWidth"];
                returnRaster.numColumns = width;

                int height = tags["ImageLength"];
                returnRaster.numRows = height;

                int imageSize = height * width;

                //Console.WriteLine($"ht: {height}    wd: {width}.");
                int[] raster = new int[imageSize];

                string NoDataString = String.Empty;
                if(tags.ContainsKey("GDAL_NODATA"))
                {
                    NoDataString = tags["GDAL_NODATA"];
                    if (NoDataString is not null)
                        returnRaster.NoDataValueString = NoDataString.Trim();
                }

                var t = tags["ModelPixelScaleTag"];
                returnRaster.cellSizeX = tags["ModelPixelScaleTag"][0];
                returnRaster.cellSizeY = tags["ModelPixelScaleTag"][1];

                var tiePoint = tags["ModelTiepointTag"];
                double tpImageSpaceX = tiePoint[0];
                double tpImageSpaceY = tiePoint[1];
                double tpImageSpaceZ = tiePoint[2];
                double tpWorldX = tiePoint[3];  // i.e., World Coordinates
                double tpWorldY = tiePoint[4];
                double tpWorldZ = tiePoint[5];
                returnRaster.anchorPoint = new GTBpoint(tpWorldX, tpWorldY);

                // Tech Debt: The anchor point may come from either ModelTiepointTag,
                // ModelTransformationTag, or the sidecar file. If the sidecar file
                // is present, use it. If not, then if ModelTiepointTag is defined,
                // use that. Only look for and interpret ModelTransformationTag if
                // ModelTiepointTag is not defined. See GeoTiff spec B.6 for
                // authority and explanation.
                // The tech debt here is that we have not implemented sidecar or
                // ModelTransformationTag yet.

                returnRaster.anchorPoint = new GTBpoint(tpWorldX, tpWorldY);
                returnRaster.leftXCoordinate = tpWorldX;
                returnRaster.topYCoordinate = tpWorldY;

                returnRaster.rightXCoordinate = tpWorldX + 
                    returnRaster.cellSizeX * returnRaster.numColumns;

                returnRaster.bottomYCoordinate = tpWorldY -
                    returnRaster.cellSizeY * returnRaster.numRows;
                #endregion raster base class items

                #region GeoTiffRaster tags
                returnRaster.SampleFormat =
                    (short?)tags.GetNullable("SampleFormat");
                //returnRaster.CellDataType = (CellDataTypeEnum)tags["DataType"];

                returnRaster.SamplesPerPixel = 
                    (short?)tags.GetNullable("SamplesPerPixel");

                returnRaster.BitsPerSample =
                    (short?)tags.GetNullable("BitsPerSample");

                returnRaster.CellDataType = determineType
                    (returnRaster.SampleFormat, returnRaster.BitsPerSample);

                returnRaster.Compression =
                    (short?)tags.GetNullable("Compression");

                returnRaster.PhotometricInterpretation =
                    (short?)tags.GetNullable("PhotometricInterpretation");

                returnRaster.StripOffsets =
                    (long[])tags.GetNullable("StripOffsets");

                returnRaster.RowsPerStrip =
                    (short?)tags.GetNullable("RowsPerStrip");

                returnRaster.StripByteCounts =
                    (long[])tags.GetNullable("StripByteCounts");

                returnRaster.PlanarConfiguration =
                    (short?)tags.GetNullable("PlanarConfiguration");


                returnRaster.TileOffsets =
                    (long[])tags.GetNullable("TileOffsets");

                returnRaster.TileByteCounts =
                    (long[])tags.GetNullable("TileByteCounts");

                returnRaster.TileWidth = (long?)tags.GetNullable("TileWidth");
                returnRaster.TileLength = (long?)tags.GetNullable("TileLength");
                returnRaster.TileSize = tifData.TileSize();

                returnRaster.SampleFormat =
                    (short?)tags.GetNullable("SampleFormat");

                #endregion GeoTiffRaster tags

                #region Read data bands

                int bytesPerValue = (int) returnRaster.BitsPerSample / 8;
                int valuesPerPixel = (int)returnRaster.SamplesPerPixel;
                int pixelSpan = bytesPerValue * valuesPerPixel;
                returnRaster.BandCount = valuesPerPixel;
                returnRaster.CreateDataEmptyFrame();

                returnRaster.CellDataType = 
                    determineType(returnRaster.SampleFormat, 
                    returnRaster.BitsPerSample);

                returnRaster.SetupGrid();

                List<List<Byte>> byteLists = new List<List<byte>>();
                for(int i=0; i < valuesPerPixel; i++)
                {
                    byteLists.Add(new List<byte>());
                }

                if (returnRaster.StripOffsets != null &&
                    returnRaster.StripOffsets.Length > 0)
                {   // readByStripMethod
                    int bytesToRead = tifData.ScanlineSize();
                    byte[] buf = new byte[bytesToRead];
                    byte[] cellBuffer = new byte[bytesPerValue];
                    if ((int) PlanarConfig.CONTIG == (int) returnRaster.PlanarConfiguration)
                    {   // CONTIG is chunky -- all bands in one strip
                        int dataFramePositionIdx = 0;
                        for (int row = 0; row < returnRaster.numRows; row++)
                        {   // Parse and rearrange bytes into each byteList
                            tifData.ReadScanline(buf, row);

                            for(int position = dataFramePositionIdx; 
                                (position - dataFramePositionIdx) < bytesToRead; 
                                position+=bytesPerValue)
                            {
                                T receiver = ConvertFromByteArrayTo<T>(buf, position-dataFramePositionIdx);
                                returnRaster.DataFrame[position/bytesPerValue] = receiver;
                            }
                            dataFramePositionIdx += bytesToRead;
                        }

                        bool ScanLineApproachSucceeded = true;
                        // Technical Debt: find a good way to verify that the read operation succeeded.
                        // This test will set ScanLineApproachSucceeded to false. But for now it's true.

                        if(!ScanLineApproachSucceeded)
                        {
                            //tryReadingAsTiles(tifData, returnRaster);
                            throw new Exception("ScanLineApproachSucceeded failed. Not able to read as tiles.");
                            // return tryReadAsBlocks();
                        }
                    }
                    else if (PlanarConfig.CONTIG == PlanarConfig.SEPARATE)
                    {  // SEPARATE is planar -- one strip per band -- rarely used
                        //value = image.GetField(TiffTag.SAMPLESPERPIXEL);
                        short spp = (short)returnRaster.SamplesPerPixel;

                        for (short s = 0; s < spp; s++)
                        {
                            for (int row = 0; row < returnRaster.numRows; row++)
                                tifData.ReadScanline(buf, row, s);
                        }
                    }
                    else if (tifData.IsTiled() == true)
                    {
                        throw new DataMisalignedException(
                            "Unable to read tiled Tiff files. Read operation failed.");
                    }
                    else
                    {
                        throw new DataMisalignedException(
                        "Unknown data configuration prevents reading this Tiff.");
                    }
                }

                #endregion Read data bands

                }

            return returnRaster;
        }

        public static T ConvertFromByteArrayTo<T>(byte[] buffer, int startIndex) 
            where T : struct
        {
            var theSize = Marshal.SizeOf(typeof(T));
            ReadOnlySpan<byte> span = 
                new ReadOnlySpan<byte>(buffer, startIndex, Marshal.SizeOf<T>());

            return MemoryMarshal.Read<T>(span);
        }



        private static Type determineType(int? sampleFormat, int? bitsPerSample)
        {
            if(sampleFormat is null || bitsPerSample is null)
                return null;

            if(bitsPerSample == 4)
            {
                return null;
            }
            if(bitsPerSample == 8)
            {
                return Type.GetType("System.Byte"); ;
            }
            if(bitsPerSample == 16)
            {
                if (sampleFormat == GeoTiffRaster<ushort>.BPS_UnsignedInteger)
                    return Type.GetType("System.UInt16"); // CellDataTypeEnum.UInt16;
                if (sampleFormat == GeoTiffRaster<short>.BPS_SignedInteger)
                    return Type.GetType("System.UInt16"); //CellDataTypeEnum.Int16;
                return null; // CellDataTypeEnum.Unknown;
            }
            if(bitsPerSample == 32)
            {
                if(sampleFormat == GeoTiffRaster<uint>.BPS_UnsignedInteger)
                    return Type.GetType("System.UInt32"); // CellDataTypeEnum.UInt32;
                if(sampleFormat == GeoTiffRaster<int>.BPS_SignedInteger)
                    return Type.GetType("System.Int32"); // CellDataTypeEnum.Int32;
                if (sampleFormat == GeoTiffRaster<float>.BPS_IEEEFP)
                    return Type.GetType("System.Single"); //CellDataTypeEnum.Float;
                return null; // CellDataTypeEnum.Unknown;
            }
            if(bitsPerSample == 64)
            {
                return Type.GetType("System.Double"); //CellDataTypeEnum.Double;
                // There is no combination which would return Long or ULong.
            }
            return null; // CellDataTypeEnum.Unknown;
        }

        ///// Uncomment this when doing GeoTiffs
        //protected static void tryReadingAsTiles(Tiff tifData, GeoTiffRaster<T> rstr)
        //{
        //    int bytesPerValue = (int)rstr.BitsPerSample / 8;
        //    int bandCount = (int)rstr.SamplesPerPixel;

        //    int rasterColumns = rstr.numColumns; // Pixel columns in the raster
        //    int rasterRows = rstr.numRows;     // Pixel rows in the raster

        //    int columnsPerTile = (int)rstr.TileWidth; // Pixel columns in a tile
        //    int rowsPerTile = (int)rstr.TileLength; // Pixel rows in a tile

        //    int tileColumnCount = 1 + (int) (rasterColumns / columnsPerTile); // Columns of tiles in a raster
        //    int tileRowCount = 1 + (int) (rasterRows / rowsPerTile);  // Rows of tiles in a raster

        //    int lastTilePixelColumns = rasterColumns % columnsPerTile; 
        //    int lastTilePixelRows = rasterRows % rowsPerTile;

        //    int lastTileColumnOverhang = columnsPerTile - lastTilePixelColumns;
        //    int lastTileBandColumnsOverhang = lastTileColumnOverhang * bandCount;
        //    int lastTileRowOverhang = rowsPerTile - lastTilePixelRows;
        //    int pixelCount = rasterRows * rasterColumns;
        //    int byteCount = pixelCount * bytesPerValue;
        //    //for (int bandId = 0; bandId < bandCount; bandId++)
        //    //{
        //    //    Byte[] byteBlock = new Byte[byteCount];
        //    //    rstr.AddBand(byteBlock, rstr.CellDataType);
        //    //}

        //    GridInstance grd =
        //        new GridInstance(rasterColumns: rasterColumns,
        //        rasterRows: rasterRows, columnsPerTile: columnsPerTile,
        //        rowsPerTile: rowsPerTile);

        //    //tifData.ReadTile(new byte[4], 0, 0, 0, 0, 0);
        //    byte[] buffer = new byte[columnsPerTile * rowsPerTile * bandCount];
        //    int bufferLength = buffer.Length;

        //    int destinationIndex = 0;
        //    int subPixelCounter = 0;
        //    int subPixelStepSize = (int) rstr.BitsPerSample / 8;
        //    byte[] buf = new byte[columnsPerTile * rowsPerTile * bandCount];
        //    int tileCounter = -1;
        //    for (int tileRow = 0; tileRow < tileRowCount; tileRow++)
        //    {
        //        for (int tileColumn = 0; tileColumn < tileColumnCount; tileColumn++)
        //        {
        //            tileCounter++;
        //            tifData.ReadTile(buf, 0, tileColumn, tileRow, 0, 0);
        //            for(int bdx=0; bdx < bandCount; bdx++)
        //            { // Note: This code builds but does not work. 
        //                //var byteArrayDestination =
        //                //    (rstr.bands[bdx].CellArray) as byte[];
        //                //Buffer.BlockCopy(buf, subPixelCounter,
        //                //    byteArrayDestination, destinationIndex, subPixelCounter);
        //            }
        //        }
        //    }

        //    ////
        //    //for(int tileColumn=0; tileColumn < tileColumnCount; tileColumn++)
        //    //{
        //    //    for(int tileRow=0; tileRow < tileRowCount; tileRow++)
        //    //    {
        //    //        for(int subCol=0; subCol< columnsPerTile; subCol++)
        //    //        {
        //    //            for(int subRow=0; subRow < rowsPerTile; subRow++)
        //    //            {
        //    //                byte[] buf = new byte[4];
        //    //                tifData.ReadTile(buf, 0, rasterColumn, rasterRow, 0, 0);
        //    //                int i = 0;
        //    //            }
        //    //        }
        //    //    }
        //    //}

        //    ////

        //    //int rasterColumn = tileColumn * columnsPerTile + subCol;
        //    //int rasterRow = tileRow * rowsPerTile + subRow;
        //    //int rasterIndex = rasterRow * rasterColumns + rasterColumn;
        //    //int tileIndex = subRow * columnsPerTile + subCol;
        //    //int tileOffset = tileIndex * bandCount;
        //    //int tileByteOffset = tileOffset * 4;
        //    //int rasterByteOffset = rasterIndex * 4;


        //    // 
        //    // Rework logic to use the new Tiled coordinate system framework I developed.
        //    List<byte[]> byteBlocks = new List<byte[]>();

        //    for (int y = 0; y < rstr.numRows; y+=(int)rstr.TileLength)
        //    {
        //        for(int x = 0; x < rstr.numColumns; x += (int)rstr.TileWidth)
        //        {
        //            buf = new byte[(int)rstr.TileSize];
        //            tifData.ReadTile(buf, 0, x, y, 0, 0);
        //            byteBlocks.Add(buf);
        //        }
        //    }
        //    var tileSize = rstr.TileSize;
        //    var tileWidth = rstr.TileWidth;
        //    var columnCnt = rstr.numColumns;
        //    var flattenedArray = byteBlocks.SelectMany(tile => tile).ToArray();
        //    int flattenedArrayLength = flattenedArray.Length;

        //    int cellCount = rstr.numRows * rstr.numColumns;
        //    int cellSizeInBytes = (int) rstr.BitsPerSample / 8;
        //    int numBands = (int) rstr.SamplesPerPixel;
        //    int cellBlockSize = cellSizeInBytes * numBands;
        //    int numCells = cellCount * cellSizeInBytes;
        //    numCells = rstr.numRows * rstr.numColumns;
        //    List<dynamic[]> bandArrays = new List<dynamic[]>();
        //    for(int idx = 0; idx < numBands; idx++)
        //    {
        //        bandArrays.Add(new dynamic[numCells]);
        //    }

        //    //int rasterSizeInBytes = cellCount * numBands;
        //    //int stepSize = cellSizeInBytes;
        //    //for(int row = 0; row < rstr.numRows; row++)
        //    //{
        //    //    int rowStart = row * rstr.numColumns;
        //    //    for(int col = 0; col < rstr.numColumns; col++)
        //    //    {
        //    //        int linearIndex = rowStart + col;
        //    //        int flattenedIndex = linearIndex * cellBlockSize;
        //    //        for(int bandIdx = 0; bandIdx < numBands; bandIdx++)
        //    //        {
        //    //            dynamic[] aBandArray = bandArrays[bandIdx];
        //    //            aBandArray[linearIndex] = flattenedArray[flattenedIndex + bandIdx];
        //    //        }
        //    //    }
        //    //}
        //    //int stopHere = 1;

        //    #region ForDevDiagnostics
        //    // Not needed to run in production; only when figuring out how
        //    // to understand this data structure.
        //    //
        //    List<Range> zerosRanges = new List<Range>();
        //    int aStart = 0; int anEnd = 0;
        //    for (int idx = 0; idx < flattenedArray.Length; idx++)
        //    {
        //        if (flattenedArray[idx] == 0)
        //        {
        //            anEnd = idx;
        //        }
        //        else
        //        {
        //            if (anEnd == idx - 1)
        //            {
        //                zerosRanges.Add(new Range(aStart, anEnd));
        //            }
        //            aStart = idx;
        //        }
        //    }

        //    Range r1 = zerosRanges[1];
        //    var v = r1.End.Value - r1.Start.Value;
        //    var v2 = zerosRanges[14].End.Value - zerosRanges[14].Start.Value;

        //    int before = zerosRanges.Count;

        //    zerosRanges = zerosRanges
        //        .Where(r => (r.End.Value - r.Start.Value) > 1)
        //        .ToList();
        //    int after = zerosRanges.Count;
        //    var zeroRangeSizes = zerosRanges
        //        .Select(r => r.End.Value - r.Start.Value).ToList();

        //    bool allAre210 = zeroRangeSizes.All(s => s == 210);
        //    List<int> not210 = zeroRangeSizes.Where(s => s != 210).ToList();
        //    bool allTheseAreSame = not210.All(s => s == not210[0]);
        //    int countOf210Span = zeroRangeSizes.Count(s => s == 210);
        //    int countOf81280 = zeroRangeSizes.Count(s => s == 81280);
        //    int sumCount = countOf210Span + countOf81280;
        //    #endregion ForDevDiagnostics

        //    int i = 09;
        //    throw new NotImplementedException();
        //}

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
            returnDict["Compression"] = tif.GetAsShort("Compression");

            //"PhotometricInterpretation"
            // ExploreTiff(tif, "Compression");
            returnDict["PhotometricInterpretation"] = 
                tif.GetAsShort("PhotometricInterpretation");

            //"StripOffsets"
            returnDict["StripOffsets"] = 
                tif.GetAsLongArray("StripOffsets");

            returnDict["StripByteCounts"] = tif.GetAsLongArray("StripByteCounts");

            returnDict["RowsPerStrip"] = tif.GetAsShort("RowsPerStrip");

            //"SamplesPerPixel" The number of bands. (Surprise, ain't it.)
            returnDict["SamplesPerPixel"] = tif.GetAsInt("SamplesPerPixel");

            //"PlanarConfiguration"
            returnDict["PlanarConfiguration"] = tif
                .GetAsShort("PlanarConfiguration");

            //returnDict["TileWidth"] = tif.GetAsShort("TileWidth");
            //returnDict["TileLength"] = tif.GetAsShort("TileLength");
            returnDict["TileOffsets"] = tif.GetAsLongArray("TileOffsets");
            returnDict["TileByteCounts"] = tif.GetAsLongArray("TileByteCounts");
            returnDict["TileWidth"] = tif.GetAsInt("TileWidth");
            returnDict["TileLength"] = tif.GetAsInt("TileLength");
            returnDict["ExtraSamples"] = tif.GetAsShort("ExtraSamples");
            returnDict["SampleFormat"] = tif.GetAsShort("SampleFormat");

            #region Geospatial tags
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
            int numberOfKeys = 0;
            var geoTagKeys = tif.GetAsShortArray("GeoKeyDirectoryTag");
            if(geoTagKeys is not null)
            {
                numberOfKeys = geoTagKeys.Length;
                int totalRowCount = numberOfKeys / numColumns;
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
            }

            //ExploreTiff(tif, "GeoKeyDirectoryTag");
            //returnDict["GeoKeyDirectoryTag"]

            //"GeoDoubleParamsTag"
            //"GeoAsciiParamsTag"

            //"GDAL_METADATA"  string type.
            returnDict["GDAL_METADATA"] = tif.GetAsString("GDAL_METADATA");

            //"GDAL_NODATA"
            string noDataString = tif.GetAsString("GDAL_NODATA");
            if(noDataString is not null)
            {
                noDataString = noDataString.Trim();
                if (noDataString.Contains("\0"))
                {
                    noDataString = noDataString.Remove(noDataString.Length - 1, 1);
                }
                returnDict["GDAL_NODATA"] = noDataString;

            }

            #endregion Geospatial tags

            #region More Diagnostics
            //HashSet<string> tagsInFileHS = new HashSet<string>(tagsInTheFile.Values);
            //HashSet<string> tagIHaveReadSoFar = new HashSet<string>(returnDict.Keys);
            //var InFileButNotReadYet = tagsInFileHS.Except(tagIHaveReadSoFar);
            #endregion More Diagnostics

            return returnDict;
        }

        public static void WriteGeoTiff<T>(GeoTiffRaster<T> tiff,
            string fileToCreate) where T : struct
        {
            /// Necessary callback. Found at
            /// https://stackoverflow.com/a/52492819/1339950
            static void TagExtender(Tiff tiff)
            {
                TiffFieldInfo[] tiffFieldInfo =
                {
                    new TiffFieldInfo(
                        TiffTag.GEOTIFF_MODELTIEPOINTTAG, 
                        6, 6, TiffType.DOUBLE, FieldBit.Custom, 
                        false, true, "MODELTIEPOINTTAG"),

                    new TiffFieldInfo(
                        TiffTag.GEOTIFF_MODELPIXELSCALETAG,
                        3, 3, TiffType.DOUBLE, FieldBit.Custom, 
                        false, true, "MODELPIXELSCALETAG")
                };

                tiff.MergeFieldInfo(tiffFieldInfo, tiffFieldInfo.Length);
            }

            Tiff.SetTagExtender(TagExtender);

            // aliases
            int width = tiff.numColumns;
            int height = tiff.numRows;
            int typeSize = (int)tiff.BitsPerSample;
            int typeSizeInBytes = typeSize / 8;

            using (Tiff outFile = Tiff.Open(fileToCreate, "w"))
            {
                outFile.CreateDirectory();

                #region writeHeaderData
                bool success = true;

                success |= outFile.SetFromInt("ImageWidth", width);
                success = outFile.SetFromInt("ImageLength", height);
                var grid = new Grid.GridInstance(width, height);

                if (tiff.BitsPerSample is not null)
                    success = outFile.SetFromInt("BitsPerSample", typeSize);

                success = outFile.SetFromShort("Compression", (short)Compression.NONE);
                success = outFile.SetFromShort("PhotometricInterpretation",
                    (short)Photometric.MINISBLACK);

                // strip offsets -- this needs a study
                // strip byte counts -- same

                success = outFile.SetField(TiffTag.ROWSPERSTRIP, (int)tiff.RowsPerStrip);
                success = outFile.SetFromInt("SamplesPerPixel", (int)tiff.BandCount);
                success = outFile.SetFromShort("PlanarConfiguration", (short)1);

                success = outFile.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                success = outFile.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);

                // TileOffsets same as strip offsets for non-tiled tiff
                // TileByteCounts same as strip by counts for non-tiled

                // TileWidth and TileLength not written for non-tiled
                // ExtraSamples not written -- not sure what it is for

                short sampleFormat = GetBPSintFromType<T>();
                outFile.SetFromShort("SampleFormat", sampleFormat);

                double[] scaleTag = new double[] {
                    (double)3.125, (double)3.125, (double)0.0 };
                success = outFile.SetField(
                    TiffTag.GEOTIFF_MODELPIXELSCALETAG,
                    3, scaleTag);

                double[] anchorPt = new double[]
                {
                    0d, 0d, 0d, tiff.anchorPoint.X,
                    tiff.anchorPoint.Y, tiff.anchorPoint.Z
                };
                success = outFile.SetField(
                    TiffTag.GEOTIFF_MODELTIEPOINTTAG,
                    6, anchorPt);

                #endregion writeHeaderData

                #region writeRasterData

                byte[] cellBuffer = new byte[typeSizeInBytes];
                byte[] stripBuffer = new byte[width * typeSizeInBytes];

                for(int row=0; row < height; row++)
                {
                    for(int column=0; column < width; column++)
                    {
                        T[] cellValue = new T[] { tiff.GetValueAt(column, row) };

                        Buffer.BlockCopy(cellValue, 0, 
                            cellBuffer, 0, typeSizeInBytes);
                        int rowArrayPosition = column * typeSizeInBytes;
                        Array.Copy(cellBuffer, 0, 
                            stripBuffer, rowArrayPosition, typeSizeInBytes);
                    }
                    outFile.WriteScanline(stripBuffer, row);
                }
                #endregion // writeRasterData

                outFile.WriteDirectory();
                outFile.Flush();
            }
        }

        private static short GetBPSintFromType<T>()
        {
            Type dataType = typeof(T);
            if (dataType == typeof(System.UInt32)) 
                return GeoTiffRaster<uint>.BPS_UnsignedInteger;

            if (dataType == typeof(System.Int32))
                return GeoTiffRaster<uint>.BPS_SignedInteger;

            return GeoTiffRaster<uint>.BPS_IEEEFP;

        }

        // Non-production code. For understanding what is going on.
        private static void ExploreTiff(Tiff tif, string varName)
        {
            int id = AllTags.Tag(varName).IdInteger;
            FieldValue[] value = tif.GetField((TiffTag)id);
            if(value is null) return;

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
        public static bool SetFromShort(this Tiff tif,
            string varName, short value)
        {
            int id = AllTags.Tag(varName).IdInteger;
            var res = tif.SetField((TiffTag)id, value);
            return res;
        }


        public static short? GetAsShort(this Tiff tif, string varName)
        {
            int id = AllTags.Tag(varName).IdInteger;
            FieldValue[] value = tif.GetField((TiffTag)id);

            if (value is null) return null;
            return (value[0]).ToShort();
        }

        public static bool SetFromInt(this Tiff tif, 
            string varName, int value)
        {
            int id = AllTags.Tag(varName).IdInteger;
            return tif.SetField((TiffTag)id, value);
        }

        public static int? GetAsInt(this Tiff tif, string varName)
        {
            int id = AllTags.Tag(varName).IdInteger;
            FieldValue[] value = tif.GetField((TiffTag)id);

            if (value is null) return null;
            return (value[0]).ToInt();
        }

        public static bool SetFromString(this Tiff tif,
            string varName, string value)
        {
            int id = AllTags.Tag(varName).IdInteger;
            return tif.SetField((TiffTag)id, value);
        }

        public static string? GetAsString(this Tiff tif, string varName)
        {
            int id = AllTags.Tag(varName).IdInteger;
            FieldValue[] value = tif.GetField((TiffTag)id);

            if (value is null) return null;
            // string diagnosticString = value[1].ToString();
            return (value[1]).ToString();
        }

        public static bool SetFromDouble(this Tiff tif, 
            string varName, double value)
        {
            int id = AllTags.Tag(varName).IdInteger;
            return tif.SetField((TiffTag)id, value);
        }

        public static double? GetAsDouble(this Tiff tif, string varName)
        {
            int id = AllTags.Tag(varName).IdInteger;
            FieldValue[] value = tif.GetField((TiffTag)id);

            if (value is null) return null;
            return (value[0]).ToDouble();
        }

        public static bool SetFromShortArray(this Tiff tif,
            string varName, short[] value)
        {
            int id = AllTags.Tag(varName).IdInteger;
            return tif.SetField((TiffTag)id, value);
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

        public static bool SetFromDoubleArray(this Tiff tif,
            TiffTag ttag, double[] value)
        {
            var res = tif.SetField(ttag, value.Length, value);
            return res;
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

        public static bool SetFromLongArray(this Tiff tif,
            string varName, long[] value)
        {
            int id = AllTags.Tag(varName).IdInteger;
            return tif.SetField((TiffTag)id, value);
        }


        public static long[]? GetAsLongArray(this Tiff tif, string varName)
        {
            int id = AllTags.Tag(varName).IdInteger;
            FieldValue[] value = tif.GetField((TiffTag)id);

            if (value is null || value.Length == 0) return null;

            if(value.Length == 1)
                return value[0].Value as long[];


            // Untested code below here.
            int arraySize = value[0].ToInt();

            Byte[] byteArray;
            if (value != null && value.Length > 1 && value[1].Value is byte[])
            {
                byteArray = (byte[])value[1].Value;

                List<long> accumulator = new List<long>();
                int itemSize = sizeof(long);
                for (int i = 0; i < arraySize; i++)
                {
                    accumulator.Add(
                        BitConverter.ToInt64(byteArray, i * itemSize));
                }
                return accumulator.ToArray();
            }

            return null;
        }

        public static dynamic? GetNullable
            (this Dictionary<string, dynamic?> thisDict, string key)
        {
            if (!thisDict.ContainsKey(key)) return null;
            return thisDict[key];
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





