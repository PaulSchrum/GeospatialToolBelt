using GeoTBelt.GeoTiff;
using GeoTBelt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landis_Mimic
{
    public static class RasterFactory
    {
        /// <summary>
        /// Creates an InputRaster<T> by loading it from a geotiff file.
        /// </summary>
        /// <typeparam name="T">Type of dataframe to return</typeparam>
        /// <param name="path">Path to the file to open.</param>
        /// <param name="SuppressTypeMismatchExceptions">If true, no exception 
        /// thrown when there is a type mismatch between the raster cell type
        /// and the paramter T.</param>
        /// <returns></returns>
        public static IInputRaster<T> OpenRaster<T>(
            string path, bool SuppressTypeMismatchExceptions=true) 
            where T : struct
        {

            var localRaster = (GeoTiffRaster<T>)Raster<T>
                .Load(path, SuppressTypeMismatchExceptions);

            LandisRaster<T> workingRaster = new LandisRaster<T>();
            workingRaster.theRaster = localRaster;

            return (IInputRaster<T>) workingRaster;
        }

        public static IOutputRaster<T> CreateRaster<T>
            (string path, Dimensions dimensions)
            where T : struct
        {
            LandisRaster<T> returnRaster = new LandisRaster<T>();
            returnRaster.theRaster =
                GeoTiffRaster<T>.MakeNew(path,
                dimensions.Rows, dimensions.Columns);

            var v = returnRaster.theRaster;

            returnRaster.Dimensions = dimensions;
            return returnRaster;
        }
    }
}
