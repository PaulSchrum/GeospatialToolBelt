using GeoTBelt;

namespace Test_ToolBelt
{
    [TestClass]
    public class RasterTests
    {
        private static string currentDirectory;
        private static string ascTestFile;
        private static string ascTestFileFullPath;
        private static Raster ascRaster;
        private static string ascOutputTestFile;
        private static string ascOutputTestFileFullPath;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            ascTestFile = "TestData_00765413.asc";
            ascOutputTestFile = "TestData_00765413_out.asc";
            currentDirectory = Directory.GetCurrentDirectory();
            ascOutputTestFileFullPath = Path.Combine(currentDirectory, ascOutputTestFile);

            var pathAsList = currentDirectory.Split("\\").ToList();
            pathAsList = pathAsList.Take(pathAsList.Count - 3).ToList();
            currentDirectory = string.Join("\\", pathAsList);
            ascTestFileFullPath = Path.Combine(currentDirectory, ascTestFile);
            ascRaster = Raster.Load(ascTestFileFullPath);
        }

        [TestMethod]
        public void ASC_PopulatesSingleBand() 
        {
            Assert.AreEqual(1, ascRaster.bands.Count);

            double expected = 4477.26d;
            double? actual = ascRaster.bands[0].CellArray[0, 0];
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected: expected, actual: (double) actual,
                delta: 0.005);

            expected = 4476.672d;
            actual = ascRaster.bands[0].CellArray[1, 1];
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected: expected, actual: (double)actual,
                delta: 0.005);

        }

        [TestMethod]
        public void ASC_WriteASCRaster_BlockWriteMethod_correctly()
        {
            Assert.IsNotNull(ascRaster);
            double? intermediateValue = ascRaster.bands[0].CellArray[3, 3];
            Assert.IsNotNull(intermediateValue);
            double expected = (double)intermediateValue + 5.0;
            ascRaster.bands[0].CellArray[3, 3] = expected;
            ascRaster.WriteASCRaster(ascOutputTestFileFullPath);

            var outputRaster = Raster.Load(ascOutputTestFileFullPath);
            Assert.IsNotNull(outputRaster);
            Assert.AreEqual(1, outputRaster.bands.Count);
            double? actual = outputRaster.bands[0].CellArray[3, 3];
            Assert.AreEqual(expected: expected, actual: (double)actual,
                delta: 0.005);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            if(File.Exists(ascOutputTestFileFullPath))
                File.Delete(ascOutputTestFileFullPath);
        }
    }
}
