using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTBelt
{
    public class Band
    {
        public dynamic[,] CellArray { get; protected set; }
        public Raster MyParent { get; protected set; }
        public Type Type { get; set; } = typeof(double);

        public Band(Raster parent)
        { 
            MyParent = parent;
        }

        public void CreateCellArray(int rowCount, int columnCount)
        {
            CellArray = new dynamic[rowCount, columnCount];
        }
    }
}
