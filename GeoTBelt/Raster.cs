using BitMiracle.LibTiff.Classic;
using GeoTBelt.GeoTiff;
using GeoTBelt.Grid;
using System.Collections;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
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
        public dynamic NoDataValDynamic { get; internal set; }
        public int BandCount { get; internal set; } = 0;
        public int TotalCellCount { get; internal set; }
        public Type CellDataType { get; internal set; }
        public int SingleCellDataLength { get; internal set; }
        public int ByteArrayLength { get; protected set; }

        protected Byte[] DataFrame { get; set; }
        protected GridInstance grid { get; set; }


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
            this.BandCount++;

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

                CellDataType = typeof(float);  // valid only for ASCII
                NoDataValDynamic = convertStringToNumber(NoDataValue);
                TotalCellCount = numColumns * numRows;
                ByteArrayLength = 
                    TotalCellCount * SingleCellDataLength * BandCount;
                DataFrame = new Byte[ByteArrayLength];
                grid = new GridInstance(numColumns, numRows);

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

                        if (entry == this.NoDataValue)
                        {
                            setValue(this.NoDataValDynamic, columnCounter, rowCounter);
                        }
                        else
                        {
                            var value = convertStringToNumber(entry);
                            setValue(value, columnCounter, rowCounter);
                        }
                    }
                }

            }
        }

        private Type numberParser(string inputValue)
        {
            if (inputValue.Contains("."))
            {
                float _;
                bool isFloat = Single.TryParse(inputValue, out _);
                if (isFloat)
                    return typeof(Single);
            }
            else
            {
                int _;
                bool isInt = Int32.TryParse(inputValue, out _);
                if (isInt)
                    return typeof(int);
            }
            // technical debt: Need to make a guess for Byte (i.e., char)
            if(inputValue.Length > 1)
                return typeof(string);

            return typeof(char);
        }

        protected dynamic convertStringToNumber(string inputValue)
        {
            return CellDataType switch
            {
                { } type when type == typeof(byte) => Convert.ToByte(inputValue),
                { } type when type == typeof(float) => Convert.ToSingle(inputValue),
                { } type when type == typeof(uint) => Convert.ToUInt32(inputValue),
                { } type when type == typeof(short) => Convert.ToInt16(inputValue),
                { } type when type == typeof(ushort) => Convert.ToUInt16(inputValue),
                { } type when type == typeof(long) => Convert.ToInt64(inputValue),
                { } type when type == typeof(ulong) => Convert.ToUInt64(inputValue),
                { } type when type == typeof(sbyte) => Convert.ToSByte(inputValue),
                { } type when type == typeof(int) => Convert.ToInt32(inputValue),
                { } type when type == typeof(double) => Convert.ToDouble(inputValue),

                _ => throw 
                        new InvalidOperationException
                        ($"Unsupported conversion type: {CellDataType}.")
            };
        }

        protected Byte[] ConvertToByteArray(dynamic value, Type type)
        {
            return type switch
            {
                _ when type == typeof(byte) => new byte[] { (byte)value },
                _ when type == typeof(uint) => BitConverter.GetBytes((uint)value),
                _ when type == typeof(ushort) => BitConverter.GetBytes((ushort)value),
                _ when type == typeof(ulong) => BitConverter.GetBytes((ulong)value),
                _ when type == typeof(float) => BitConverter.GetBytes((float)value),
                _ when type == typeof(SByte) => BitConverter.GetBytes((SByte)value),
                _ when type == typeof(double) => BitConverter.GetBytes((double)value),
                _ when type == typeof(bool) => BitConverter.GetBytes((bool)value),
                _ when type == typeof(short) => BitConverter.GetBytes((short)value),
                _ when type == typeof(int) => BitConverter.GetBytes((int)value),
                _ when type == typeof(long) => BitConverter.GetBytes((long)value),
                _ => throw new InvalidOperationException("Unsupported type")
            };
        }

        ////////////////////////////////////////////////////////////////////////
        /// Big Technical Debt here: Multiple bands. At this time (March 2024),
        ///   I am putting a way to specifiy band in this part of the API, but
        ///   not implementing it yet as this is specified out by client for now.
        ///   If it comes back in later, it needs to be relocated to be in the
        ///   GridInstance class, which is supposed to encapsulate everything
        ///   related to interfacing with a linear array in a multi-dimensional
        ///   way.
        ////////////////////////////////////////////////////////////////////////

        public void setValue(dynamic value, int column, int row,
            int TileColumnIndex=-1, int TileRowIndex=-1, int band=0)
        {
            int arrayIndex = this.grid.AsArrayIndex(column, row);
            Byte[] thisValueAsByteArray = ConvertToByteArray(value, this.CellDataType);
            for(int i=0; i<this.ByteArrayLength; i++)
                this.DataFrame[arrayIndex+i] = thisValueAsByteArray[i];
        }

        public Byte GetAsByte(int columnIndex, int rowIndex,
            int TileColumnIndex = -1, int TileRowIndex = -1, int band = 0)
        {
            int arrayIndex = this.grid.AsArrayIndex(columnIndex, rowIndex);
            Byte returnByte = this.DataFrame[arrayIndex];
            return returnByte;
        }

        public SByte GetAsSByte(int columnIndex, int rowIndex,
            int TileColumnIndex = -1, int TileRowIndex = -1, int band = 0)
        {
            int arrayIndex = this.grid.AsArrayIndex(columnIndex, rowIndex);
            SByte returnSByte = (SByte) this.DataFrame[arrayIndex];
            return returnSByte;
        }

        public short GetAsShort(int columnIndex, int rowIndex,
            int TileColumnIndex = -1, int TileRowIndex = -1, int band = 0)
        {
            int arrayIndex = this.grid.AsArrayIndex(columnIndex, rowIndex);
            int shortLength = sizeof(short);
            arrayIndex *= shortLength;
            return BitConverter.ToInt16(DataFrame, arrayIndex);
        }

        public ushort GetAsUShort(int columnIndex, int rowIndex,
             int TileColumnIndex = -1, int TileRowIndex = -1, int band = 0)
        {
            int arrayIndex = this.grid.AsArrayIndex(columnIndex, rowIndex);
            int ushortLength = sizeof(ushort);
            arrayIndex *= ushortLength;
            return BitConverter.ToUInt16(DataFrame, arrayIndex);
        }

        public int GetAsInt(int columnIndex, int rowIndex,
             int TileColumnIndex = -1, int TileRowIndex = -1, int band = 0)
        {
            int arrayIndex = this.grid.AsArrayIndex(columnIndex, rowIndex);
            int intLength = sizeof(int);
            arrayIndex *= intLength;
            return BitConverter.ToInt32(DataFrame, arrayIndex);
        }

        public uint GetAsUInt(int columnIndex, int rowIndex,
             int TileColumnIndex = -1, int TileRowIndex = -1, int band = 0)
        {
            int arrayIndex = this.grid.AsArrayIndex(columnIndex, rowIndex);
            int uintLength = sizeof(uint);
            arrayIndex *= uintLength;
            return BitConverter.ToUInt32(DataFrame, arrayIndex);
        }

        public long GetAsLong(int columnIndex, int rowIndex,
             int TileColumnIndex = -1, int TileRowIndex = -1, int band = 0)
        {
            int arrayIndex = this.grid.AsArrayIndex(columnIndex, rowIndex);
            int longLength = sizeof(long);
            arrayIndex *= longLength;
            return BitConverter.ToInt64(DataFrame, arrayIndex);
        }

        public ulong GetAsULong(int columnIndex, int rowIndex,
             int TileColumnIndex = -1, int TileRowIndex = -1, int band = 0)
        {
            int arrayIndex = this.grid.AsArrayIndex(columnIndex, rowIndex);
            int ulongLength = sizeof(ulong);
            arrayIndex *= ulongLength;
            return BitConverter.ToUInt64(DataFrame, arrayIndex);
        }

        public float GetAsFloat(int columnIndex, int rowIndex,
             int TileColumnIndex = -1, int TileRowIndex = -1, int band = 0)
        {
            int arrayIndex = this.grid.AsArrayIndex(columnIndex, rowIndex);
            int floatLength = sizeof(float);
            arrayIndex *= floatLength;
            return BitConverter.ToSingle(DataFrame, arrayIndex);
        }

        public double GetAsDouble(int columnIndex, int rowIndex,
             int TileColumnIndex = -1, int TileRowIndex = -1, int band = 0)
        {
            int arrayIndex = this.grid.AsArrayIndex(columnIndex, rowIndex);
            int doubleLength = sizeof(double);
            arrayIndex *= doubleLength;
            return BitConverter.ToDouble(DataFrame, arrayIndex);
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

////////////////////////////////////////////
                //writer.Write(this.bands[0].ToString());
                
                
                
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
/////////////////////////////////////////////////
            //dynamic[] dataset = datasetIn.ToArray();
            //Type bandType = dataset.GetType().GetElementType();
            //var newBand = new Band(this, dataset, bandType, this.numRows, this.numColumns);
            //this.bands.Add(newBand);
        }

        public void AddBand(Byte[] rawDataAsBytes, Type aType)
        {
////////////////////////////////////////////////////////
            //Band newBand = 
            //    new Band(this, rawDataAsBytes, aType, this.numRows, this.numColumns);
            //this.bands.Add((Band)newBand);
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