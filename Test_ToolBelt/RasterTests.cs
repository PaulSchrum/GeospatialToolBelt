using GeoTBelt;

namespace Test_ToolBelt
{
    [TestClass]
    public class RasterTests
    {
        private string currentDirectory = Directory.GetCurrentDirectory();
        private string ascTestFile = "TestData_00765413.asc";
        private string ascTestFileFullPath = default(string);

        private Raster ascRaster = default(Raster);

        public RasterTests()
        {
            var pathAsList = currentDirectory.Split("\\").ToList();
            pathAsList = pathAsList.Take(pathAsList.Count - 3).ToList();
            currentDirectory = string.Join("\\", pathAsList);
        }

        private void SetupAscFormatData()
        {
            if (ascRaster is null)
            {
                ascTestFileFullPath = Path.Combine(currentDirectory, ascTestFile);
                ascRaster = Raster.Load(ascTestFileFullPath);
            }
            Assert.IsNotNull(ascRaster);
        }

        [TestMethod]
        public void ASC_file_loads()
        {
            SetupAscFormatData();
        }

        [TestMethod]
        public void ASC_PopulatesSingleBand() 
        {
            SetupAscFormatData();
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
