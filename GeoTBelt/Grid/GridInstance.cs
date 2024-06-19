using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTBelt.Grid
{
    public class GridInstance
    {
        private GridInstance() {}

        public int rasterColumns { get; private set; }
        public int rasterRows { get; private set; }
        public int tileColumns { get; private set; }
        public int tileRows { get; private set; }
        public int columnsPerTile { get; private set; }
        public int rowsPerTile { get; private set; }
        public int fullTilesColumnar { get; private set; }
        public int fullTilesRowise { get; private set; }
        public int lastTileColumnRemainder { get; private set; }
        public int lastTileRowRemainder { get; private set; }

        public GridInstance(
            int rasterColumns, int rasterRows, 
            int columnsPerTile=-1, int rowsPerTile=-1)
        {
            if (columnsPerTile == -1) columnsPerTile = rasterColumns;
            if (rowsPerTile == -1) rowsPerTile = rasterRows;
            this.rasterColumns = rasterColumns;
            this.rasterRows = rasterRows;
            this.columnsPerTile = columnsPerTile;
            this.rowsPerTile = rowsPerTile;

            this.tileColumns = rasterColumns / columnsPerTile;
            this.fullTilesColumnar = tileColumns;
            this.lastTileColumnRemainder = rasterColumns % columnsPerTile;
            if(this.lastTileColumnRemainder > 0) this.tileColumns++;

            this.tileRows = rasterRows / rowsPerTile;
            this.fullTilesRowise = tileRows;
            this.lastTileRowRemainder = rasterRows % rowsPerTile;
            if(this.lastTileRowRemainder > 0) this.tileRows++;
        }

        public RasterCoordinates AsRasterCoordinates(TiledCoordinates tileCoordinates)
        {
            throw new NotImplementedException();
        }

        public RasterCoordinates AsRasterCoordinates(
            int tileCol, int tileRow, int subCol, int subRow)
        {
            throw new NotImplementedException();
        }

        public RasterCoordinates AsRasterCoordinates(int arrayIndex)
        {
            int row = (int) (arrayIndex / rasterColumns);
            int column = arrayIndex % rasterColumns;

            return new RasterCoordinates(column, row);
        }

        public TiledCoordinates AsTiledCoordinates(int col, int row)
        {
            return AsTiledCoordinates(new RasterCoordinates(col, row));
        }

        public TiledCoordinates AsTiledCoordinates(RasterCoordinates rasterCoordinates)
        {
            int tileCol; int tileRow; int tileSubCol; int tileSubRow;
            int col = rasterCoordinates.column;  // Just an alias for shorter lines of code
            int row = rasterCoordinates.row;

            if (col >= this.rasterColumns || row >= this.rasterRows)
                throw new ArgumentException();

            tileCol = col / this.columnsPerTile;
            tileRow = row / this.rowsPerTile;

            tileSubCol = col % this.columnsPerTile;
            tileSubRow = row % this.rowsPerTile;

            return new TiledCoordinates(tileCol, tileRow, tileSubCol, tileSubRow); 
        }

        public TiledCoordinates AsTiledCoordinates(int arrayIndex)
        { throw new NotImplementedException(); }

        public int TileSequentialCountFrom(int tileCol, int tileRow)
        {
            int wholeRowsPart = (tileRow * this.tileColumns);
            int t = wholeRowsPart + tileCol;
            return wholeRowsPart + tileCol;
        }

        public int AsArrayIndex(TiledCoordinates tileCoordinates) 
        { 
            
            
            throw new NotImplementedException(); 
        }

        public int AsArrayIndex(int tileCol, int tileRow, int subCol, int subRow)
        {
            RasterCoordinates rstsCoord = 
                AsRasterCoordinates(tileCol, tileRow, subCol, subRow);
            return AsArrayIndex(rstsCoord);
        }

        public int AsArrayIndex(RasterCoordinates rasterCoordinates)
        {
            if (rasterCoordinates == null) return -1;
            var r = rasterCoordinates;  // aliasing for convenience
            if(r.column >= this.rasterColumns) throw new IndexOutOfRangeException();
            if(r.row >= this.rasterRows) throw new IndexOutOfRangeException();

            return r.row * this.rasterColumns + r.column;
        }

        public int AsArrayIndex(int row, int column)
        {
            return AsArrayIndex(new RasterCoordinates(column, row));
        }
    }
}
