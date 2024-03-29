using GeoTBelt;
using GeoTBelt.GeoTiff;
using System.Linq;

namespace Test_ToolBelt
{
    [TestClass]
    public class RasterTests
    {
        private static string currentDirectory;

        private static string ascTestFileName;
        private static string ascTestFileFullPath;
        private static Raster ascRaster;
        private static string ascOutputTestFile;
        private static string ascOutputTestFileFullPath;

        private static GeoTiffRaster geoTiffRaster_singleBand;
        private static string geoTiffFileName_singleBand;
        private static string geoTiffFileFullPath_singleBand;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            currentDirectory = improvedGetCurrentDirectory("Test_ToolBelt");
        }

        private static void initializeAscRaster()
        {
            if(ascRaster is null)
            {
                ascTestFileName = "TestData_00765413.asc";
                ascOutputTestFile = "TestData_00765413_out.asc";
                ascOutputTestFileFullPath = Path.Combine(currentDirectory, ascOutputTestFile);
                var pathAsList = currentDirectory.Split("\\").ToList();
                pathAsList = pathAsList.Take(pathAsList.Count - 3).ToList();
                //currentDirectory = string.Join("\\", pathAsList);
                ascTestFileFullPath = Path.Combine(currentDirectory, ascTestFileName);
                ascRaster = Raster.Load(ascTestFileFullPath);
            }

        }


        private static string improvedGetCurrentDirectory(string desiredDirectory)
        {
            var currDirList = Directory.GetCurrentDirectory().Split("\\").ToList();
            while (currDirList.Last() != desiredDirectory)
            {
                currDirList.RemoveAt(currDirList.Count - 1);
            }
            return string.Join("\\", currDirList);
        }

        #region ASC format tests
        [TestMethod]
        public void ASC_PopulatesSingleBand() 
        {
            initializeAscRaster();
            Assert.AreEqual(1, ascRaster.bands.Count);

            double expected = 4477.26d;
            double? actual = ascRaster.bands[0].At(0, 0);
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected: expected, actual: (double) actual,
                delta: 0.005);

            expected = 4476.672d;
            actual = ascRaster.bands[0].At(1, 1);
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected: expected, actual: (double)actual,
                delta: 0.005);

        }

        [TestMethod]
        public void ASC_WriteASCRaster_BlockWriteMethod_correctly()
        {
            initializeAscRaster();
            Assert.IsNotNull(ascRaster);
            double? intermediateValue = ascRaster.bands[0].At(3, 3);
            Assert.IsNotNull(intermediateValue);
            double expected = (double)intermediateValue + 5.0;
            ascRaster.bands[0].Set(expected, 3, 3);
            ascRaster.WriteASCRaster(ascOutputTestFileFullPath);

            var outputRaster = Raster.Load(ascOutputTestFileFullPath);
            Assert.IsNotNull(outputRaster);
            Assert.AreEqual(1, outputRaster.bands.Count);
            double? actual = outputRaster.bands[0].At(3, 3);
            Assert.AreEqual(expected: expected, actual: (double)actual,
                delta: 0.005);
        }
        #endregion ASC format tests

        private static void initializeSingleBandTiff()
        {
            if (geoTiffRaster_singleBand is null)
            {
                // geoTiffFileName = "NCSU Biltmore Hall small uncomp.tif";
                geoTiffFileName_singleBand = "TestData_00765413_uncompressed.tif";
                geoTiffFileFullPath_singleBand = Path.Combine(currentDirectory, geoTiffFileName_singleBand);
                geoTiffRaster_singleBand = (GeoTiffRaster)Raster.Load(geoTiffFileFullPath_singleBand);
            }
        }

        [TestMethod]
        public void GeoTiff_PopulatesSingleBand()
        {
            initializeSingleBandTiff();
            Assert.IsNotNull(geoTiffRaster_singleBand);
            
            Assert.AreEqual(
                expected: 2, 
                actual: geoTiffRaster_singleBand.RowsPerStrip);

            Assert.AreEqual(expected: 1,
                actual: geoTiffRaster_singleBand.bands.Count);

            float actual0_0 = (float) geoTiffRaster_singleBand.bands[0].At(0, 0);
            Assert.AreEqual(expected: 4477.26f, actual: actual0_0, delta: 0.0001);

            float actual799_799 = (float)geoTiffRaster_singleBand.bands[0].At(799, 799);
            Assert.AreEqual(expected: 5055.912f, actual: actual799_799, delta: 0.0001);

            float actual798_797 = (float)geoTiffRaster_singleBand.bands[0].At(798, 797);
            Assert.AreEqual(expected: 5055.26f, actual: actual798_797, delta: 0.0001);

            bool exceptionThrown = false;
            try
            {
                float fails = (float)geoTiffRaster_singleBand.bands[0].At(800, 800);
            }
            catch (IndexOutOfRangeException)
            { exceptionThrown = true; } 
            catch (Exception ex) { }
            Assert.IsTrue(exceptionThrown);

            exceptionThrown = false;
            try
            {
                float fails = (float)geoTiffRaster_singleBand.bands[0].At(100, 800);
            }
            catch (IndexOutOfRangeException)
            { exceptionThrown = true; }
            catch (Exception ex) { }
            Assert.IsTrue(exceptionThrown);

            exceptionThrown = false;
            try
            {
                float fails = (float)geoTiffRaster_singleBand.bands[0].At(800, 100);
            }
            catch (IndexOutOfRangeException)
            { exceptionThrown = true; }
            catch (Exception ex) { }
            Assert.IsTrue(exceptionThrown);
        }


        private static GeoTiffRaster geoTiffRaster_5BandSmall;
        private static string geoTiffFileName_5BandSmall;
        private static string geoTiffFileFullPath_5BandSmall;

        private static void initialize5BandSmallTiff()
        {
            if (geoTiffRaster_5BandSmall is null)
            {
                geoTiffFileName_5BandSmall = "NCSU Biltmore Hall small uncomp.tif";
                geoTiffFileFullPath_5BandSmall = Path.Combine(currentDirectory, geoTiffFileName_5BandSmall);
                geoTiffRaster_5BandSmall = (GeoTiffRaster)Raster.Load(geoTiffFileFullPath_5BandSmall);
            }
        }

        [Ignore]
        [TestMethod]
        public void GeoTiff_small_Populates5Band()
        {
            initialize5BandSmallTiff();
            Assert.IsNotNull(geoTiffRaster_5BandSmall);

            Assert.AreEqual(
                expected: 2,
                actual: geoTiffRaster_5BandSmall.RowsPerStrip);

            Assert.AreEqual(expected: 5,
                actual: geoTiffRaster_5BandSmall.bands.Count);
        }


        private static GeoTiffRaster geoTiffRaster_5BandLarge;
        private static string geoTiffFileName_5BandLarge;
        private static string geoTiffFileFullPath_5BandLarge;

        private static void initialize5BandLargeTiff()
        {
            if (geoTiffRaster_5BandLarge is null)
            {
                geoTiffFileName_5BandLarge = "NCSU Jordan Hall uncomp.tif";
                geoTiffFileFullPath_5BandLarge = Path.Combine(currentDirectory, geoTiffFileName_5BandLarge);
                geoTiffRaster_5BandLarge = (GeoTiffRaster)
                    Raster.Load(geoTiffFileFullPath_5BandLarge);
            }
        }

        [Ignore]
        [TestMethod]
        public void GeoTiff_large_Populates5Band()
        {
            initialize5BandLargeTiff();
            Assert.IsNotNull(geoTiffRaster_5BandLarge);

            Assert.AreEqual(
                expected: 2,
                actual: geoTiffRaster_5BandLarge.RowsPerStrip);

            Assert.AreEqual(expected: 5,
                actual: geoTiffRaster_5BandLarge.bands.Count);
        }


        [ClassCleanup]
        public static void Cleanup()
        {
            if(File.Exists(ascOutputTestFileFullPath))
                File.Delete(ascOutputTestFileFullPath);
        }
    }
}
