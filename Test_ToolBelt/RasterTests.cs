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
            ascTestFileFullPath = Path.Combine(currentDirectory, ascTestFile);
            if (ascRaster is null)
            {
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

            double expected = 4474.663d;
            double? actual = ascRaster.bands[0].CellArray[2, 3];

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected: expected, actual: (double) actual,
                delta: 0.005);
        }
    }
}
