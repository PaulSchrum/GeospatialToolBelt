using System.Drawing;

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
        public Point anchorPoint { get; protected set; } // upper left point of the raster
        public string NoDataValue { get; protected set; }
        public List<Band> bands { get; protected set; } = new List<Band>();

        private Raster() { }

        private void populateRasterFromAscFile(string path)
        {

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