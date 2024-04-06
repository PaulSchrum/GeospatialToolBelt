using BitMiracle.LibTiff.Classic;
using GeoTBelt.GeoTiff;
using GeoTBelt.Grid;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
//using static System.Net.Mime.MediaTypeNames;

namespace GeoTBelt
{
    public class Raster<T> where T : struct
    {
        public double cellSize
        {
            get
            {
                if (_cellSizeX is null)
                {
                    _cellSizeX = _cellSizeY = 1.0;
                }
                return (double)_cellSizeX;
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
                return (double)_cellSizeY;
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

        private string noDataValueString_;
        public string NoDataValueString 
        { 
            get { return noDataValueString_; }
            internal set
            {
                noDataValueString_ = value;
                NoDataValue = ParseStringToNumber(NoDataValueString);
            }
        }
        public T NoDataValue { get; protected set; }
        internal Type CellDataType { get; set; } = null;
        public int CellCount { get; internal set; }
        public int BandCount { get; internal set; }
        public GridInstance Grid { get; protected set; }
        public List<T> DataFrame { get; private set; }
        private T[] dataFrameAsArray { get; set; }
        //public List<Band> bands { get; internal set; } = new List<Band>();


        internal Raster() { }

        /// <summary>
        /// Loads a raster of any supported format.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Raster<T> Load(string fullPath, string format = "")
        {
            string fileType = format;
            if (string.IsNullOrEmpty(fileType))
                fileType = Path.GetExtension(fullPath).ToLower();
            if (string.IsNullOrEmpty(fileType))
                throw new ArgumentException
                    ("Could not determine file type from path or format parameter.");

            Raster<T> returnRaster = new Raster<T>();

            fileType = fileType.ToLower();
            if (fileType[0] == '.')
                fileType = fileType.Substring(1);

            if (fileType == "asc")
            {
                returnRaster.populateRasterFromAscFile(fullPath);
            }
            else if (considerGeoTiff(fileType))
            {
                //throw new NotImplementedException("GeoTiff");
                returnRaster = GeoTiffHelper.ReadGeoTiff<T>(fullPath);
                //.populateRasterFromTiffFile(fullPath);
            }
            else
            {
                throw new ArgumentException
                    ("Could not determine file type from path or format parameter.");
            }

            return returnRaster;
        }

        public T GetValueAt(int row, int column, int band=0)
        { 
            int linearIndex = this.Grid.AsArrayIndex(row, column);
            return DataFrame[linearIndex];
        }

        public void SetValueAt(T value, 
            int row, int column, int band=0)
        {
            int linearIndex = this.Grid.AsArrayIndex(row, column);
            DataFrame[linearIndex] = value;
        }

        #region Asc Raster Format
        private void populateRasterFromAscFile(string path)
        {
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
                                NoDataValueString = lineArray[1];
                                NoDataValue = ParseStringToNumber(NoDataValueString);
                                rowCount++;
                                break;
                            }
                        default:
                            break;
                    }
                }

                topYCoordinate = bottomYCoordinate + cellSize * numRows;
                anchorPoint = new GTBpoint(leftXCoordinate, topYCoordinate);

                Grid = new GridInstance(numColumns, numRows);

                BandCount = 1;
                CellCount = numColumns * numRows * BandCount;
                CreateDataEmptyFrame();

                string line;
                int rowCounter = -1;
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
                        dataFrameAsArray[Grid.AsArrayIndex(columnCounter, rowCounter)] =
                            ParseStringToNumber(entry);
                    }
                }
                this.DataFrame = dataFrameAsArray.ToList();
            }
        }

        private T ParseStringToNumber(string strVal)
        {
            return 
                (T)Convert.ChangeType(strVal, typeof(T), CultureInfo.InvariantCulture);
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
                writer.WriteLine("NODATA_value  " + NoDataValueString);

                for (int currentRow = 0; currentRow < numRows; currentRow++)
                {
                    for (int currentColumn = 0; currentColumn < numColumns; currentColumn++)
                    {
                        T value = this.GetValueAt(currentRow, currentColumn);
                        if (EqualityComparer<T>.Default.Equals(value, NoDataValue))
                        {
                            writer.Write(NoDataValue + " ");
                        }
                        else
                        {
                            string outValue = $"{value:0.###} ";
                            writer.Write(outValue);
                        }
                    }
                    writer.WriteLine("");
                }
                writer.Flush();
            }
        }
        #endregion Asc Raster Format


        //public Raster populateRasterFromTiffFile(string fileToOpen)
        //{ }

        //public void AddBand(IEnumerable<dynamic> datasetIn)
        //{
        //    dynamic[] dataset = datasetIn.ToArray();
        //    Type bandType = dataset.GetType().GetElementType();
        //    var newBand = new Band<T>(this, dataset, bandType, this.numRows, this.numColumns);
        //    this.bands.Add(newBand);
        //}

        //public void AddBand(Byte[] rawDataAsBytes, Type aType)
        //{
        //    Band<T> newBand = 
        //        new Band<T>(this, rawDataAsBytes, aType, this.numRows, this.numColumns);
        //    this.bands.Add((Band<T>)newBand);
        //}

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

        internal void CreateDataEmptyFrame()
        {
            CellCount = numColumns * numRows * BandCount;
            dataFrameAsArray = new T[CellCount];
        }
    }

}