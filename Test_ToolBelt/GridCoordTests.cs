using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeoTBelt.Grid;

namespace Test_ToolBelt
{
    [TestClass]
    public class GridCoordTests
    {
        private GridInstance gridInstance1 = default;
        private GridInstance gridInstance2 = default;

        [TestInitialize]
        public void Setup()
        {
            gridInstance1 = new GridInstance(
                rasterColumns:28, 
                rasterRows: 23,
                columnsPerTile: 10,
                rowsPerTile: 9
                );

            gridInstance2 = new GridInstance(
                rasterColumns: 10*4,
                rasterRows: 10*4,
                columnsPerTile: 10,
                rowsPerTile: 10 );
        }

        [TestMethod]
        public void GridInstance_exists()
        {
            Assert.IsNotNull( gridInstance1 );
        }

        [TestMethod]
        public void GridInstance_ParametersAreCorrect()
        {
            int eRasterCols = 28;
            int eRasterRows = 23;
            int eTileCols = 3;
            int eTileRows = 3;
            int eColsPerTile = 10;
            int eRowsPerTile = 9;
            int eFullTilesColumnar = 2;
            int eFullTilesRowise = 2;
            int eLastTileColumnRemainder = 8;
            int eLastTileRowRemainder = 5;

            Assert.AreEqual(expected: eRasterCols, 
                            actual: gridInstance1.rasterColumns);

            Assert.AreEqual(expected: eRasterRows,
                            actual: gridInstance1.rasterRows);

            Assert.AreEqual(expected: eTileCols,
                            actual: gridInstance1.tileColumns);

            Assert.AreEqual(expected: eTileRows,
                            actual: gridInstance1.tileRows);

            Assert.AreEqual(expected: eColsPerTile,
                            actual: gridInstance1.columnsPerTile);

            Assert.AreEqual(expected: eRowsPerTile,
                            actual: gridInstance1.rowsPerTile);

            Assert.AreEqual(expected: eFullTilesColumnar,
                            actual: gridInstance1.fullTilesColumnar);

            Assert.AreEqual(expected: eFullTilesRowise,
                            actual: gridInstance1.fullTilesRowise);

            Assert.AreEqual(expected: eLastTileColumnRemainder,
                            actual: gridInstance1.lastTileColumnRemainder);

            Assert.AreEqual(expected: eLastTileRowRemainder,
                            actual: gridInstance1.lastTileRowRemainder);



            var gridInstanceLocal = new GridInstance(
                rasterColumns: 28,
                rasterRows: 29,
                columnsPerTile: 10,
                rowsPerTile: 9
                );

            eRasterCols = 28;
            eRasterRows = 29;
            eTileCols = 3;
            eTileRows = 4;
            eColsPerTile = 10;
            eRowsPerTile = 9;
            eFullTilesColumnar = 2;
            eFullTilesRowise = 3;
            eLastTileColumnRemainder = 8;
            eLastTileRowRemainder = 2;

            Assert.AreEqual(expected: eRasterCols,
                            actual: gridInstanceLocal.rasterColumns);

            Assert.AreEqual(expected: eRasterRows,
                            actual: gridInstanceLocal.rasterRows);

            Assert.AreEqual(expected: eTileCols,
                            actual: gridInstanceLocal.tileColumns);

            Assert.AreEqual(expected: eTileRows,
                            actual: gridInstanceLocal.tileRows);

            Assert.AreEqual(expected: eColsPerTile,
                            actual: gridInstanceLocal.columnsPerTile);

            Assert.AreEqual(expected: eRowsPerTile,
                            actual: gridInstanceLocal.rowsPerTile);

            Assert.AreEqual(expected: eFullTilesColumnar,
                            actual: gridInstanceLocal.fullTilesColumnar);

            Assert.AreEqual(expected: eFullTilesRowise,
                            actual: gridInstanceLocal.fullTilesRowise);

            Assert.AreEqual(expected: eLastTileColumnRemainder,
                            actual: gridInstanceLocal.lastTileColumnRemainder);

            Assert.AreEqual(expected: eLastTileRowRemainder,
                            actual: gridInstanceLocal.lastTileRowRemainder);
        }

        [TestMethod]
        public void GridInstance_TestSequentialTileConversion()
        {
            int expectedSequentialIndex = 9;
            int actualSequentialIndex = gridInstance2.TileSequentialCountFrom(1, 2);
            Assert.AreEqual(expected: expectedSequentialIndex, actual: actualSequentialIndex);
        }

        [TestMethod]
        public void GridInstance_ConversionsAreCorrect()
        {
            var result = gridInstance1.AsTiledCoordinates(3, 4);
            Assert.AreEqual(expected: 0, actual: result.TileColumn);
            Assert.AreEqual(expected: 0, actual: result.TileRow);
            Assert.AreEqual(expected: 3, actual: result.subTileColumn);
            Assert.AreEqual(expected: 4, actual: result.subTileRow);

            result = gridInstance1.AsTiledCoordinates(15, 15);
            Assert.AreEqual(expected: 1, actual: result.TileColumn);
            Assert.AreEqual(expected: 1, actual: result.TileRow);
            Assert.AreEqual(expected: 5, actual: result.subTileColumn);
            Assert.AreEqual(expected: 6, actual: result.subTileRow);

            result = gridInstance1.AsTiledCoordinates(28-2, 23-2);
            Assert.AreEqual(expected: 2, actual: result.TileColumn);
            Assert.AreEqual(expected: 2, actual: result.TileRow);
            Assert.AreEqual(expected: 6, actual: result.subTileColumn);
            Assert.AreEqual(expected: 3, actual: result.subTileRow);
        }

        [TestMethod]
        public void GridInstance_ConversionsTo1Darray_areCorrect()
        {
            var result = gridInstance1.AsArrayIndex(28 - 2, 23-1);
            Assert.AreEqual(expected: 642, actual: result);
        }

        [TestMethod]
        public void GridInstance_ConversionTo1Darray_columnOOB_throws()
        {
            Assert.ThrowsException<IndexOutOfRangeException>
                (()=> gridInstance1.AsArrayIndex(28, 23));
        }
    }
}
