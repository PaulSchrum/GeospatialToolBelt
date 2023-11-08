

namespace GeoTBelt
{
    public class Raster
    {
        public double cellSize { get; protected set; }
        public int numColumns { get; protected set; }
        public int numRows { get; protected set; }
        public double leftXCoordinate { get; protected set; }
        public double bottomYCoordinate { get; protected set; }
        public double topYCoordinate { get; protected set; }
        public GTBpoint anchorPoint { get; protected set; } // upper left point of the raster
        public string NoDataValue { get; protected set; }
        public List<Band> bands { get; protected set; } = new List<Band>();

        private Raster() { }

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

            if(fileType == "asc")
            {
                returnRaster.populateRasterFromAscFile(fullPath);
            }
            else
            {
                throw new ArgumentException
                    ("Could not determine file type from path or format parameter.");
            }

            return returnRaster;
        }
    }

}