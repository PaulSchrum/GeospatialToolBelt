using GeoTBelt;

namespace Test_ToolBelt
{
    [TestClass]
    public class RasterTests
    {
        private static string currentDirectory;
        private static string ascTestFile = "TestData_00765413.asc";
        private static string ascTestFileFullPath;
        private static Raster ascRaster;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            currentDirectory = Directory.GetCurrentDirectory();
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
    }
}
