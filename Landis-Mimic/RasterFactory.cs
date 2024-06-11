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
        public static IInputRaster<T> OpenRaster<T>(string path) 
            where T : struct
        {

            var localRaster = (GeoTiffRaster<T>)Raster<T>.Load(path);

            LandisRaster<T> workingRaster = new LandisRaster<T>();
            workingRaster.theRaster = localRaster;

            return (IInputRaster<T>) workingRaster;
        }
    }
}
