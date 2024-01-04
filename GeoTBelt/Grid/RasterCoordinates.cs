using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTBelt.Grid
{
    public class RasterCoordinates
    {
        private RasterCoordinates() { }

        public int column { get; private set; }
        public int row { get; private set; }

        public RasterCoordinates(int column, int row)
        {
            this.column = column;
            this.row = row;
        }
    }
}
