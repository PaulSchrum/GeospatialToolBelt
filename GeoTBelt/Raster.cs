using BitMiracle.LibTiff.Classic;
using static System.Net.Mime.MediaTypeNames;

namespace GeoTBelt
{
    public class Raster
    {
        public double cellSize { get; protected set; }  // Technical Debt: Need isomorphic cell sizes.
        public int numColumns { get; protected set; }
        public int numRows { get; protected set; }
        public double leftXCoordinate { get; protected set; }
        public double bottomYCoordinate { get; protected set; }
        public double topYCoordinate { get; protected set; }
        public GTBpoint anchorPoint { get; protected set; } // upper left point of the raster
        public string NoDataValue { get; protected set; }
        public List<Band> bands { get; protected set; } = new List<Band>();

        #region TIFF Tags
        private static readonly TiffTag TIFFTAG_GEOPIXELSCALE = (TiffTag)33550;
        private static readonly TiffTag TIFFTAG_GEOTIEPOINTS = (TiffTag)33922;
        private static readonly TiffTag TIFFTAG_GEOKEYDIRECTORY = (TiffTag)34735;
        private static readonly TiffTag TIFFTAG_GEODOUBLEPARAMS = (TiffTag)34736;
        private static readonly TiffTag TIFFTAG_GEOASCIIPARAMS = (TiffTag)34737;
        private static readonly TiffTag TIFFTAG_GDAL_METADATA = (TiffTag)42112;
        private static readonly TiffTag TIFFTAG_GDAL_NODATA = (TiffTag)42113;
        private static readonly TiffTag TIFFTAG_GDAL_NODATA1 = (TiffTag)42113;
        private static readonly TiffTag TIFFTAG_GDAL_NODATA2 = (TiffTag)42114;
        private static readonly TiffTag TIFFTAG_GDAL_NODATA3 = (TiffTag)42115;
        private static readonly TiffTag TIFFTAG_GDAL_NODATA4 = (TiffTag)42116;
        private static readonly TiffTag TIFFTAG_GDAL_METADATA1 = (TiffTag)42112;
        private static readonly TiffTag TIFFTAG_GDAL_METADATA24 = (TiffTag)42135;
        // Note: I don't know how many metadatas there are. - Paul S.
        #endregion TIFF Tags

        private Raster() { }

        /// <summary>
        /// Loads a raster of any supported format.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Raster Load(string fullPath, string format = null)
        {
            string fileType = format;
            if (string.IsNullOrEmpty(fileType))
                fileType = Path.GetExtension(fullPath).ToLower();
            if (string.IsNullOrEmpty(fileType))
                throw new ArgumentException
                    ("Could not determine file type from path or format parameter.");

            Raster returnRaster = new Raster();

            fileType = fileType.ToLower();
            if (fileType[0] == '.')
                fileType = fileType.Substring(1);

            if (fileType == "asc")
            {
                returnRaster.populateRasterFromAscFile(fullPath);
            }
            else if (considerGeoTiff(fileType))
            {
                returnRaster.populateRasterFromTiffFile(fullPath);
            }
            else
            {
                throw new ArgumentException
                    ("Could not determine file type from path or format parameter.");
            }

            return returnRaster;
        }

        #region Asc Raster Format
        private void populateRasterFromAscFile(string path)
        {
            this.bands.Add(new Band(this));
            var band = this.bands[0];

            using (StreamReader sr = new StreamReader(path))
            {
                int rowCount = 0;
                while (rowCount < 6)
                {
                    var lineArray = sr.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    switch (lineArray[0])
                    {
                        case "ncols":
                            {
                                numColumns = Convert.ToInt32(lineArray[1]);
                                rowCount++;
                                break;
                            }
                        case "nrows":
                            {
                                numRows = Convert.ToInt32(lineArray[1]);
                                rowCount++;
                                break;
                            }
                        case "xllcorner":
                            {
                                leftXCoordinate = Convert.ToDouble(lineArray[1]);
                                rowCount++;
                                break;
                            }
                        case "yllcorner":
                            {
                                bottomYCoordinate = Convert.ToDouble(lineArray[1]);
                                rowCount++;
                                break;
                            }
                        case "cellsize":
                            {
                                cellSize = Convert.ToDouble(lineArray[1]);
                                rowCount++;
                                break;
                            }
                        case "NODATA_value":
                            {
                                NoDataValue = lineArray[1];
                                rowCount++;
                                break;
                            }
                        default:
                            break;
                    }
                }

                topYCoordinate = bottomYCoordinate + cellSize * numRows;
                anchorPoint = new GTBpoint(leftXCoordinate, topYCoordinate);
                band.Type = typeof(double);

                string line;
                int rowCounter = -1;
                bool arrayCreated = false;
                while (true)
                {
                    line = sr.ReadLine();
                    if (line == null) break;
                    var lineList = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    rowCounter++;
                    int columnCounter = -1;

                    foreach (var entry in lineList)
                    {
                        columnCounter++;
                        if (entry == this.NoDataValue)
                            band.CellArray[columnCounter, rowCounter] = double.NaN;
                        else
                        {
                            if(!arrayCreated)
                            {
                                band.CreateCellArray(numColumns, numRows);
                                band.Type = numberParser(entry);
                                arrayCreated = true;
                            }
                            if (band.Type == typeof(double))
                                band.CellArray[columnCounter, rowCounter] = double.Parse(entry);
                            else if (band.Type == typeof(int))
                                band.CellArray[columnCounter, rowCounter] = int.Parse(entry);
                            else // it's string or char. Can't handle complex.
                                band.CellArray[columnCounter, rowCounter] = entry;
                        }
                    }
                }

            }
        }

        private Type numberParser(string inputValue)
        {
            if (inputValue.Contains("."))
            {
                double _;
                bool isDouble = Double.TryParse(inputValue, out _);
                if (isDouble)
                    return typeof(double);
            }
            else
            {
                int _;
                bool isInt = Int32.TryParse(inputValue, out _);
                if (isInt)
                    return typeof(int);
            }
            if(inputValue.Length > 1)
                return typeof(string);

            return typeof(char);
        }

        public void WriteASCRaster(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("ncols         " + numColumns);
                writer.WriteLine("nrows         " + numRows);
                writer.WriteLine("xllcorner     " + leftXCoordinate);
                writer.WriteLine("yllcorner     " + bottomYCoordinate);
                writer.WriteLine("cellsize      " + cellSize);
                writer.WriteLine("NODATA_value  " + NoDataValue);

                writer.Write(this.bands[0].ToString());
                //for (int currentRow = 0; currentRow < numRows; currentRow++)
                //{
                //    for (int currentColumn = 0; currentColumn < numColumns; currentColumn++)
                //    {
                //        if (this.bands[0].cellArray[currentRow, currentColumn] == double.NaN)
                //        {
                //            writer.Write(NoDataValue + " ");
                //        }
                //        else
                //        {
                //            string outValue = $"{cellArray[currentRow, currentColumn]:0.###} ";
                //            writer.Write(outValue);
                //        }
                //    }
                //    writer.WriteLine("");
                //}
                writer.Flush();
            }
        }
        #endregion Asc Raster Format


        public Raster populateRasterFromTiffFile(string fileToOpen)
        {
            Raster returnRaster = null;

            using (Tiff image = Tiff.Open(fileToOpen, "r"))
            {
                if (image == null) throw new Exception("Could not open file.");

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

                // Read the image into the memory buffer
                if (!image.ReadRGBAImage(width, height, raster))
                    throw new Exception("Could not read image.");

                value = image.GetField(TiffTag.CELLLENGTH);
                if (value != null)
                    returnRaster.cellSize = value[0].ToDouble();
                else if ((value = image.GetField(TiffTag.CELLWIDTH)) != null)
                    returnRaster.cellSize = value[0].ToDouble();
                else if ((value = image.GetField(TiffTag.XRESOLUTION)) != null)
                    returnRaster.cellSize = value[0].ToDouble();
                else if ((value = image.GetField(TiffTag.YRESOLUTION)) != null)
                    returnRaster.cellSize = value[0].ToDouble();

                //Disable possible null invocation warning as this potential
                //   is handled by the try/catch blocks.
#pragma warning disable 8602
                try
                {
                    returnRaster.NoDataValue = imageGetFieldFromMultiple(image,
                        new List<TiffTag>
                        {   (TiffTag) TIFFTAG_GDAL_NODATA,
                            (TiffTag) TIFFTAG_GDAL_NODATA1,
                            (TiffTag) TIFFTAG_GDAL_NODATA2,
                            (TiffTag) TIFFTAG_GDAL_NODATA3,
                        }).ToString();
                }
                catch (Exception e)
                { returnRaster.NoDataValue = String.Empty; }

                try
                {
                    returnRaster.cellSize = imageGetFieldFromMultiple(image,
                        new List<TiffTag>
                        { TiffTag.XRESOLUTION, TiffTag.YRESOLUTION,
                          TiffTag.CELLLENGTH, TiffTag.CELLWIDTH,
                         }).ToDouble();
                }
                catch(Exception e) 
                { throw new Exception("File contains no cell resolution information."); }

                // Technical Debt: Allow for anisotropic cell sizes.
                //returnRaster.cellSizeY = (double)imageGetField(image, TiffTag.YRESOLUTION);

#pragma warning restore 8602

                //TiffTag.DATATYPE;
                //TiffTag.GEOTIEPOINTS;
                //TiffTag.GEOTRANSMATRIX;
                //TiffTag.GEOTIFFDIRECTORY;
                //TiffTag.GEOTIFFDOUBLEPARAMS;
                //TiffTag.GEOTIFFASCIIPARAMS;
                //TiffTag.GEOTIFFTAGS;
                //TiffTag.GEOTIFFVERSION;
                //TiffTag.GEOTIFFPROXY;
                //TiffTag.GEOTIFFIPCORE;
                //TiffTag.GEOTIFFMETADATA;
                //TiffTag.GEOTIFFPCSGEOKEYDIRECTORY;
                //TiffTag.GEOTIFFPCSDATUM;
                //TiffTag.GEOTIFFPCSNAME;
                //TiffTag.GEOTIFFPCSCITATION;
                //TiffTag.GEOTIFFGEOKEYDIRECTORY;
                //TiffTag.GEOTIFFGEOKEYS;

            }

            return returnRaster;
        }

        private dynamic? imageGetField(Tiff img, TiffTag tag)
        {
            FieldValue[] value = img.GetField(tag);
            if (value is null)
                return null;
            return (dynamic?)value[0];
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
            foreach(TiffTag aTag in tags)
            {
                FieldValue[] value = img.GetField((TiffTag) aTag);
                if(value is null)  continue;
                returnValue = (dynamic?)value[0];
                if (returnValue is not null)
                    return returnValue;
            }
            return null;
        }

        private static bool considerGeoTiff(string filename)
        {
            return
                filename.EndsWith("tif") ||
                filename.EndsWith("tiff") ||
                filename.EndsWith("geotiff") ||
                filename.EndsWith("geotif") ||
                filename.EndsWith("gtif") ||
                filename.EndsWith("gtiff");
        }
    }

}