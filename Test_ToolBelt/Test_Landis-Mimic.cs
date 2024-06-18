using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoTBelt.GeoTiff;
using Landis_Mimic;

namespace Test_ToolBelt
{
    [TestClass]
    public class Test_Landis_Mimic
    {
        private static string currentDirectory;
        private static string geoTiffFileName_singleBand;
        private static string path;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            static string improvedGetCurrentDirectory(string desiredDirectory)
            {
                var currDirList = Directory.GetCurrentDirectory().Split("\\").ToList();
                while (currDirList.Last() != desiredDirectory)
                {
                    currDirList.RemoveAt(currDirList.Count - 1);
                }
                currDirList.Add("Data");
                return string.Join("\\", currDirList);
            }
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
        public void WriteGeoTiff_IterateOverCells()
        {
            bool suppressDelete = true; // true = don't delete the new tiff.
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
                    var vv = ((LandisRaster<float>)map).theRaster;
                    for (int idx = 0; idx < totalCellCount; idx++)
                    {
                        float randomFloat = (float)random.NextDouble();
                        randomFloat *= 1000f;
                        int anInt = (int)randomFloat;
                        randomFloat = (float)anInt;
                        map.BufferPixel = randomFloat;

                        //int row = idx / dims.Columns;
                        //float newValue = 1000f * row;
                        //int col = idx % dims.Columns;
                        //newValue += col;
                        //map.BufferPixel = newValue;

                        map.WriteBufferPixel();
                    }
                }
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

