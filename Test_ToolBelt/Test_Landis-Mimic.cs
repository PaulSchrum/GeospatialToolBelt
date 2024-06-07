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
                return string.Join("\\", currDirList);
            }
            currentDirectory = improvedGetCurrentDirectory("Test_ToolBelt");

            geoTiffFileName_singleBand = "TestData_00765413_uncompressed.tif";
            path = Path.Combine(currentDirectory, geoTiffFileName_singleBand);
        }

        [TestMethod]
        public void ReadGeoTiff_IterateOverCells ()
        {
            IInputRaster<FloatPixel> map = 
                RasterFactory.OpenRaster<FloatPixel>(path);

            using (map)
            {
                FloatPixel pixel = default;
                float[] inMemoryTargetArray = new float[map.Dimensions.Rows * map.Dimensions.Columns];
                int someIndex = 0;
                int totalCellCount = map.Dimensions.Rows * map.Dimensions.Columns;

                for (int i = 0; i < map.Dimensions.Rows * map.Dimensions.Columns; i++)
                {
                    map.ReadBufferPixel();
                    pixel = map.BufferPixel;
                    float mapValue = pixel.MapCode.Value;
                    if (mapValue != 0)
                    {
                        int jiminyCricket = 99;
                    }
                    inMemoryTargetArray[someIndex++] = mapValue;
                }
                Assert.AreEqual(expected: totalCellCount,
                    actual: someIndex);
            }
        }
    }
}
