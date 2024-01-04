using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTBelt.Grid
{
    public class TiledCoordinates
    {
        private TiledCoordinates() { }

        public int TileColumn { get; set; }
        public int TileRow { get; set; }
        public int subTileColumn { get; set; }
        public int subTileRow { get; set; }

        public TiledCoordinates(
            int tileColumn, 
            int tileRow, 
            int subTileColumn, 
            int subTileRow)
        {
            this.TileColumn = tileColumn;
            this.TileRow = tileRow;
            this.subTileColumn = subTileColumn;
            this.subTileRow = subTileRow;
        }

    }
}
