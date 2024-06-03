using GeoTBelt;
using GeoTBelt.GeoTiff;
using System.Data.Common;
using System.Linq;

namespace Test_ToolBelt
{
    [TestClass]
    public class RasterTests
    {
        private static string currentDirectory;

        private static string ascTestFileName;
        private static string ascTestFileFullPath;
        private static Raster<float> ascRaster;
        private static string ascOutputTestFile;
        private static string ascOutputTestFileFullPath;

        private static GeoTiffRaster<float> geoTiffRaster_singleBand;
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
                ascRaster = Raster<float>.Load(ascTestFileFullPath);
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
            Assert.AreEqual(1, ascRaster.BandCount);

            float expected = 4477.26f;
            float actual = ascRaster.GetValueAt(0, 0);
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected: expected, actual: (double) actual,
                delta: 0.005);

            expected = 4476.672f;
            //actual = ascRaster.bands[0].At(1, 1);
            actual = ascRaster.GetValueAt(1, 1);
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected: expected, actual: (double)actual,
                delta: 0.005);

        }

        [TestMethod]
        public void ASC_WriteASCRaster_BlockWriteMethod_correctly()
        {
            initializeAscRaster();
            Assert.IsNotNull(ascRaster);
            float intermediateValue = ascRaster.GetValueAt(3, 3);
            float expected = intermediateValue + 5f;
            ascRaster.SetValueAt(expected, 3, 3);
            ascRaster.WriteASCRaster(ascOutputTestFileFullPath);

            var outputRaster = Raster<float>.Load(ascOutputTestFileFullPath);
            Assert.IsNotNull(outputRaster);
            Assert.AreEqual(1, outputRaster.BandCount);
            float actual = outputRaster.GetValueAt(3, 3);
            Assert.AreEqual(expected: expected, actual: actual,
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
                geoTiffRaster_singleBand =
                    (GeoTiffRaster<float>)Raster<float>.Load
                    (geoTiffFileFullPath_singleBand);
            }
        }

        [TestMethod]
        public void GeoTiff_LoadSingleBand_ModifyIt_SaveNewGeoTiff()
        {
            float addValue = 5f;

            var newOutputFileName = "OutputTest1.tiff";
            var newOutputFileFullName = Path.Combine(currentDirectory, newOutputFileName);
            if(File.Exists(newOutputFileFullName))
                File.Delete(newOutputFileFullName);

            initializeSingleBandTiff();

            float expected0_0 = geoTiffRaster_singleBand.GetValueAt(0, 0);
            float expected799_799 = geoTiffRaster_singleBand.GetValueAt(799, 799);
            float expected798_797 = geoTiffRaster_singleBand.GetValueAt(798, 797);
            float expected250_150 = geoTiffRaster_singleBand.GetValueAt(250, 150);

            int columnCount = geoTiffRaster_singleBand.numColumns;
            int rowCount = geoTiffRaster_singleBand.numRows;

            for (int row = 0; row < rowCount; row++)
            {
                for (int column = 0; column < columnCount; column++) 
                {
                    float cellValue = geoTiffRaster_singleBand.GetValueAt(column, row) + addValue;
                    geoTiffRaster_singleBand.SetValueAt(cellValue, column, row);
                }
            }

            try
            {
                geoTiffRaster_singleBand.SaveAs(newOutputFileFullName);
            }
            catch (Exception e)
            {
                if (File.Exists(newOutputFileFullName))
                    File.Delete(newOutputFileFullName);
                throw e;
            }

            // Test the newly created raster for correctness.
            GeoTiffRaster<float> geoTiffOut = (GeoTiffRaster<float>)Raster<float>
                .Load(newOutputFileFullName);

            Assert.AreEqual(expected: 2,
                actual: geoTiffOut.RowsPerStrip);

            Assert.AreEqual(expected: 1,
                actual: geoTiffOut.BandCount);

            float actual0_0 = geoTiffOut.GetValueAt(0, 0);
            Assert.AreEqual(
                expected: expected0_0 + addValue, 
                actual: actual0_0, delta: 0.0001);

            float actual799_799 = geoTiffOut.GetValueAt(799, 799);
            Assert.AreEqual(
                expected: expected799_799 + addValue, 
                actual: actual799_799, delta: 0.0001);

            //float actual798_797 = geoTiffOut.GetValueAt(797, 798);
            //Assert.AreEqual(
            //    expected: expected798_797 + addValue,
            //    actual: actual798_797, delta: 0.0001);

            float actual250_150 = geoTiffOut.GetValueAt(250, 150);
            Assert.AreEqual(
                expected: expected250_150 + addValue,
                actual: actual250_150, delta: 0.0001);

            Assert.AreEqual(
                expected: 642_500f,
                actual: geoTiffOut.bottomYCoordinate,
                delta: 0.005);
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
                actual: geoTiffRaster_singleBand.BandCount);

            float actual0_0 = geoTiffRaster_singleBand.GetValueAt(0, 0);
            Assert.AreEqual(expected: 4477.26f, actual: actual0_0, delta: 0.0001);

            float actual799_799 = geoTiffRaster_singleBand.GetValueAt(799, 799);
            Assert.AreEqual(expected: 5055.912f, actual: actual799_799, delta: 0.0001);

            float actual798_797 = geoTiffRaster_singleBand.GetValueAt(798, 797);
            Assert.AreEqual(expected: 5057.60498f, actual: actual798_797, delta: 0.0001);


            Assert.AreEqual(
                expected: 642_500f,
                actual: geoTiffRaster_singleBand.bottomYCoordinate,
                delta: 0.005);
            Assert.AreEqual(
                expected: 645_000,
                actual: geoTiffRaster_singleBand.topYCoordinate,
                delta: 0.005);
            Assert.AreEqual(
                expected: 750_000f,
                actual: geoTiffRaster_singleBand.leftXCoordinate,
                delta: 0.005);
            Assert.AreEqual(
                expected: 752_500f,
                actual: geoTiffRaster_singleBand.rightXCoordinate,
                delta: 0.005);

            float fails;
            bool exceptionThrown = false;
            try
            {
                fails = geoTiffRaster_singleBand.GetValueAt(800, 800);
            }
            catch (IndexOutOfRangeException)
            { exceptionThrown = true; }
            catch (Exception ex) { }
            Assert.IsTrue(exceptionThrown);

            exceptionThrown = false;
            try
            {
                fails = geoTiffRaster_singleBand.GetValueAt(100, 800);
            }
            catch (IndexOutOfRangeException)
            { exceptionThrown = true; }
            catch (Exception ex) { }
            Assert.IsTrue(exceptionThrown);

            exceptionThrown = false;
            try
            {
                fails = geoTiffRaster_singleBand.GetValueAt(800, 100);
            }
            catch (IndexOutOfRangeException)
            { exceptionThrown = true; }
            catch (Exception ex) { }
            Assert.IsTrue(exceptionThrown);
        }


        //private static GeoTiffRaster<byte> geoTiffRaster_5BandSmall;
        private static string geoTiffFileName_5BandSmall;
        private static string geoTiffFileFullPath_5BandSmall;

        private static void initialize5BandSmallTiff()
        {
        //    if (geoTiffRaster_5BandSmall is null)
        //    {
        //        geoTiffFileName_5BandSmall = "NCSU Biltmore Hall small uncomp.tif";
        //        geoTiffFileFullPath_5BandSmall = Path.Combine(currentDirectory, geoTiffFileName_5BandSmall);
        //        geoTiffRaster_5BandSmall = (GeoTiffRaster)Raster.Load(geoTiffFileFullPath_5BandSmall);
        //    }
        }

        [Ignore]
        [TestMethod]
        public void GeoTiff_small_Populates5Band()
        {
            //initialize5BandSmallTiff();
            //Assert.IsNotNull(geoTiffRaster_5BandSmall);

            //Assert.AreEqual(
            //    expected: 2,
            //    actual: geoTiffRaster_5BandSmall.RowsPerStrip);

            //Assert.AreEqual(expected: 5,
            //    actual: geoTiffRaster_5BandSmall.bands.Count);
        }


        // private static GeoTiffRaster<byte> geoTiffRaster_5BandLarge;
        private static string geoTiffFileName_5BandLarge;
        private static string geoTiffFileFullPath_5BandLarge;

        //private static void initialize5BandLargeTiff()
        //{
        //    if (geoTiffRaster_5BandLarge is null)
        //    {
        //        geoTiffFileName_5BandLarge = "NCSU Jordan Hall uncomp.tif";
        //        geoTiffFileFullPath_5BandLarge = Path.Combine(currentDirectory, geoTiffFileName_5BandLarge);
        //        geoTiffRaster_5BandLarge = (GeoTiffRaster)
        //            Raster<byte>.Load(geoTiffFileFullPath_5BandLarge);
        //    }
        //}

        [Ignore]
        [TestMethod]
        public void GeoTiff_large_Populates5Band()
        {
            //initialize5BandLargeTiff();
            //Assert.IsNotNull(geoTiffRaster_5BandLarge);

            //Assert.AreEqual(
            //    expected: 2,
            //    actual: geoTiffRaster_5BandLarge.RowsPerStrip);

            //Assert.AreEqual(expected: 5,
            //    actual: geoTiffRaster_5BandLarge.bands.Count);
        }


        [ClassCleanup]
        public static void Cleanup()
        {
            if(File.Exists(ascOutputTestFileFullPath))
                File.Delete(ascOutputTestFileFullPath);
        }
    }
}
