using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTBelt
{
    public class Band
    {
        public dynamic?[,] CellArray { get; protected set; }
        public Raster MyParent { get; protected set; }

        public Band(Raster parent)
        { 
            MyParent = parent;
        }
    }
}
