using GeoTBelt;

namespace Test_ToolBelt
{
    [TestClass]
    public class RasterTests
    {
        private string currentDirectory = Directory.GetCurrentDirectory();
        private string ascTestFile = "TestData_00765413.asc";
        private string ascTestFileFullPath = default(string);

        private void Setup()
        {
            ascTestFileFullPath = Path.Combine(currentDirectory, ascTestFile);
        }

        [TestMethod]
        public void ASC_file_loads()
        {
            Setup();
            Raster raster = Raster.Load(ascTestFileFullPath);
        }
    }
}