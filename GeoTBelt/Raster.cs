﻿using BitMiracle.LibTiff.Classic;
using GeoTBelt.GeoTiff;
using System.Collections;
//using static System.Net.Mime.MediaTypeNames;

namespace GeoTBelt
{
    public class Raster
    {
        public double cellSize
        {
            get
            {
                if (_cellSizeX is null)
                {
                    _cellSizeX = _cellSizeY = 1.0;
                }
                return (double) _cellSizeX;
            }
            internal set
            {
                _cellSizeX = value;
                _cellSizeY = value;
            }
        }

        private double? _cellSizeX = null;
        public double cellSizeX
        {
            get
            {
                if (_cellSizeX is null) { return cellSize; }
                return (double)_cellSizeX;
            }
            internal set
            {
                _cellSizeX = value;
            }
        }
        
        private double? _cellSizeY = null;
        public double cellSizeY 
        {
            get
            {
                if (_cellSizeY is null) { return cellSize; }
                return (double) _cellSizeY;
            }
            internal set
            {
                _cellSizeY = value;
            }
        }

        public int numColumns { get; internal set; }
        public int numRows { get; internal set; }
        public double leftXCoordinate { get; internal set; }
        public double rightXCoordinate { get; internal set; }
        public double bottomYCoordinate { get; internal set; }
        public double topYCoordinate { get; internal set; }
        public GTBpoint anchorPoint { get; internal set; } // upper left point of the raster
        public string NoDataValue { get; internal set; }
        internal Type CellDataType { get; set; } = null;
        public List<Band> bands { get; internal set; } = new List<Band>();


        internal Raster() { }

        /// <summary>
        /// Loads a raster of any supported format.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Raster Load(string fullPath, string format = "")
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
                returnRaster = GeoTiffHelper.ReadGeoTiff(fullPath);
                    //.populateRasterFromTiffFile(fullPath);
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
                band.theType = typeof(double);

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
                            band.Set(double.NaN, rowCounter, columnCounter);
                        else
                        {
                            if(!arrayCreated)
                            {
                                band.CreateCellArray(numColumns, numRows);
                                band.theType = numberParser(entry);
                                arrayCreated = true;
                            }
                            if (band.theType == typeof(double))
                                band.Set(double.Parse(entry), rowCounter, columnCounter);
                            else if (band.theType == typeof(int))
                                band.Set(int.Parse(entry), rowCounter, columnCounter);
                            else // it's string or char. Can't handle complex.
                                band.Set(entry, rowCounter, columnCounter);
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


        //public Raster populateRasterFromTiffFile(string fileToOpen)
        //{ }

        public void AddBand(IEnumerable<dynamic> datasetIn)
        {
            dynamic[] dataset = datasetIn.ToArray();
            Type bandType = dataset.GetType().GetElementType();
            var newBand = new Band(this, dataset, bandType, this.numRows, this.numColumns);
            this.bands.Add(newBand);
        }

        public void AddBand(Byte[] rawDataAsBytes, Type aType)
        {
            Band newBand = 
                new Band(this, rawDataAsBytes, aType, this.numRows, this.numColumns);
            this.bands.Add((Band)newBand);
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