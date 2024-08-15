using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoTBelt;
using GeoTBelt.GeoTiff;
using Landis_Mimic;
using BitMiracle.LibTiff.Classic;

namespace Test_ToolBelt
{
    [TestClass]
    public class Test_Landis_Mimic
    {
        private static string currentDirectory;
        private static string geoTiffFileName_singleBand;
        private static string path;


        private static string improvedGetCurrentDirectory(string desiredDirectory)
        {
            var currDirList = Directory.GetCurrentDirectory().Split('\\').ToList();
            while (currDirList.Last() != desiredDirectory)
            {
                currDirList.RemoveAt(currDirList.Count - 1);
            }
            currDirList.Add("Data");
            return string.Join("\\", currDirList);
        }

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            currentDirectory = improvedGetCurrentDirectory("Test_ToolBelt");

            geoTiffFileName_singleBand = "TestData_00765413_uncompressed.tif";
            path = Path.Combine(currentDirectory, geoTiffFileName_singleBand);
        }

        [TestMethod]
        public void ReadGeoTiff_IterateOverCells ()
        {
            IInputRaster<float> map = 
                RasterFactory.OpenRaster<float>(path);

            using (map)
            {
                float pixelValue = default;
                int totalCellCount = map.Dimensions.Rows * map.Dimensions.Columns;
                float[] inMemoryTargetArray = new float[totalCellCount];
                int someIndex = 0;

                for (int i = 0; i < map.Dimensions.Rows * map.Dimensions.Columns; i++)
                {
                    map.ReadBufferPixel();
                    pixelValue = map.BufferPixel;
                    inMemoryTargetArray[someIndex++] = pixelValue;
                }
                Assert.AreEqual(expected: totalCellCount,
                    actual: someIndex);

                float actualValue = inMemoryTargetArray[0];
                Assert.AreEqual(expected: 4477.260, actual: actualValue, delta: 0.01);

                actualValue = inMemoryTargetArray[20];
                Assert.AreEqual(expected: 4456.092, actual: actualValue, delta: 0.01);

                actualValue = inMemoryTargetArray[640000-1];
                Assert.AreEqual(expected: 5055.912, actual: actualValue, delta: 0.01);

                actualValue = inMemoryTargetArray[640000-20];
                Assert.AreEqual(expected: 5042.647, actual: actualValue, delta: 0.01);
            }
        }

        [TestMethod]
        public void GeoTiff_GetCellType_Succeeds()
        {
            if (currentDirectory is null)
                currentDirectory = improvedGetCurrentDirectory("Test_ToolBelt");

            var rasterFileName = "Landis\\Bug01\\ecoregions.tif";
            string localPath = Path.Combine(currentDirectory, rasterFileName);

            Assert.IsTrue(File.Exists(localPath));

            Type theRasterType = GeoTiffTools.GetRasterCellType(localPath);
            Type expectedType = typeof(byte);
            Assert.AreEqual(expected: expectedType, actual: theRasterType);
        }

        [TestMethod]
        public void ReadGeoTiff_BugReadingByteRaster_Landis01()
        {
            if(currentDirectory is null)
                currentDirectory = improvedGetCurrentDirectory("Test_ToolBelt");

            var Bug01TestPath = "Landis\\Bug01\\ecoregions.tif";
            string localPath = Path.Combine(currentDirectory, Bug01TestPath);

            Assert.IsTrue(File.Exists(localPath));

            IInputRaster<byte> mapBug01 = default(IInputRaster<byte>);
            mapBug01 = RasterFactory.OpenRaster<byte>(localPath);

            Assert.IsNotNull(mapBug01);

            for (int i = 0; i < 4; i++)
            {
                byte asByte = mapBug01.ReadBufferPixel();
                byte ab = mapBug01.BufferPixel;
            }

            mapBug01.Dispose(); mapBug01 = null;

            IInputRaster<int> mapBug0int = default(IInputRaster<int>);
            mapBug0int = RasterFactory.OpenRaster<int>(localPath); // Bug was here.

            Assert.IsNotNull(mapBug0int);

            for (int i = 0; i < 4; i++)
            {
                int asInt = mapBug0int.ReadBufferPixel();
                int ai = mapBug0int.BufferPixel;
            }

            mapBug0int.Dispose(); mapBug0int = null;

            bool ExceptionWasThrown = false;
            IInputRaster<int> secondCheck = default(IInputRaster<int>);
            try 
            {
                secondCheck = RasterFactory.OpenRaster<int>(localPath,
                    SuppressTypeMismatchExceptions: false);
            }
            catch (Exception ex) 
            { 
                ExceptionWasThrown = true;
            }
            Assert.IsTrue(ExceptionWasThrown);
        }

        [TestMethod]
        public void WriteGeoTiff_IterateOverCells()
        {
            bool suppressDelete = false; // true = don't delete the new tiff.
            string filename = "WriteGeoTiff_IterateOverCells.tiff";
            string fullPath = Path.Combine(currentDirectory, filename);

            Dimensions dims = new Dimensions(800, 800);
            int totalCellCount = dims.Rows * dims.Columns;
            Random random = new Random(78);

            try
            {
                using (
                    IOutputRaster<float> map = 
                    RasterFactory.CreateRaster<float>(fullPath, dims))
                {
                    for (int idx = 0; idx < totalCellCount; idx++)
                    {
                        float randomFloat = (float)random.NextDouble();
                        randomFloat *= 1000f;
                        int anInt = (int)randomFloat;
                        randomFloat = (float)anInt;
                        map.BufferPixel = randomFloat;
                        map.WriteBufferPixel();

                        //int row = idx / dims.Columns;
                        //float newValue = 1000f * row;
                        //int col = idx % dims.Columns;
                        //newValue += col;
                        //map.BufferPixel = newValue;
                        //map.WriteBufferPixel();

                    }
                    map.SaveAs(fullPath);
                }

                // Read as a new object. Test values at row 2, column 5,
                // and at row 798, column 400

                var inputRaster = Raster<float>.Load(fullPath);
                int row = 2; int col = 5;
                float expected = 932f;
                float actual = inputRaster.GetValueAtRowColumn(row, col, band: 1);
                Assert.AreEqual(expected, actual, delta: 0.01);

                row = 798; col = 797;
                expected = 576f;
                actual = inputRaster.GetValueAtRowColumn(row, col, band: 1);
                Assert.AreEqual(expected, actual, delta: 0.01);

            }
            finally
            {
                if (!suppressDelete)
                    if(File.Exists(fullPath))
                        File.Delete(fullPath);
            }

        }
    }
}

