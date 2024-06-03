using System;
using GeoTBelt;
using GeoTBelt.GeoTiff;

namespace Landis_Mimic
{
    class RasterLandis<TPixel, TBasic> :
        IInputRaster<TPixel>, IOutputRaster<TPixel>
        where TPixel : Pixel, new()
        where TBasic : struct
    {
        System.Type pixelType = typeof(TPixel);
        System.Type basicType = typeof(TBasic);
        private int offsetCounter = -1;

        private GeoTiffRaster<byte> byteRaster = null;
        private GeoTiffRaster<sbyte> sbyteRaster = null;
        private GeoTiffRaster<short> shortRaster = null;
        private GeoTiffRaster<ushort> ushortRaster = null;
        private GeoTiffRaster<int> intRaster = null;
        private GeoTiffRaster<uint> uintRaster = null;
        private GeoTiffRaster<long> longRaster = null;
        private GeoTiffRaster<ulong> ulongRaster = null;
        private GeoTiffRaster<float> floatRaster = null;
        private GeoTiffRaster<double> doubleRaster = null;

        public GeoTiffRaster<TBasic> TheRaster 
        { 
            get
            {
                if (byteRaster is GeoTiffRaster<TBasic> byteR) { return byteR; }
                if (sbyteRaster is GeoTiffRaster<TBasic> sbyteR) { return sbyteR; }
                if (shortRaster is GeoTiffRaster<TBasic> shortR) { return shortR; }
                if (ushortRaster is GeoTiffRaster<TBasic> ushortR) { return ushortR; }
                if (intRaster is GeoTiffRaster<TBasic> intR) { return intR; }
                if (uintRaster is GeoTiffRaster<TBasic> uintR) { return uintR; }
                if (longRaster is GeoTiffRaster<TBasic> longR) { return longR; }
                if (ulongRaster is GeoTiffRaster<TBasic> ulongR) { return ulongR; }
                if (floatRaster is GeoTiffRaster<TBasic> floatR) { return floatR; }
                if (doubleRaster is GeoTiffRaster<TBasic> doubleR) { return doubleR; }

                return null;
            }
        }

        internal RasterLandis(string path)
        {
            validateGenericHarmony();

            switch (typeof(TBasic))
            {
            case Type t when t == typeof(byte): byteRaster = 
                (GeoTiffRaster<byte>)Raster<byte>.Load(path); break;

            case Type t when t == typeof(sbyte): sbyteRaster = 
                (GeoTiffRaster<sbyte>)Raster<sbyte>.Load(path); break;

            case Type t when t == typeof(short): shortRaster = 
                (GeoTiffRaster<short>)Raster<short>.Load(path); break;

            case Type t when t == typeof(ushort): ushortRaster = 
                (GeoTiffRaster<ushort>)Raster<ushort>.Load(path); break;

            case Type t when t == typeof(int): intRaster = 
                (GeoTiffRaster<int>)Raster<int>.Load(path); break;

            case Type t when t == typeof(uint): uintRaster = 
                (GeoTiffRaster<uint>)Raster<uint>.Load(path); break;

            case Type t when t == typeof(long): longRaster = 
                (GeoTiffRaster<long>)Raster<long>.Load(path); break;

            case Type t when t == typeof(ulong): ulongRaster = 
                (GeoTiffRaster<ulong>)Raster<ulong>.Load(path); break;

            case Type t when t == typeof(float): floatRaster = 
                (GeoTiffRaster<float>)Raster<float>.Load(path); break;

            case Type t when t == typeof(double): doubleRaster = 
                (GeoTiffRaster<double>)Raster<double>.Load(path); break;

            default: throw new NotSupportedException(
                $"Type {typeof(TBasic)} is not supported.");
            }
        }

        private void affirmTheseRelateCorrectly(Type tpixelType, Type t)
        {
            if (pixelType == tpixelType &&
                basicType != t)
                throw new ArgumentException(
                    $"Incompatible type parameters: {tpixelType} and {t}.");
        }
        
        /// <summary>
        /// Generic TPixel and TBasic must be of related types.
        /// This method insures they are.
        /// </summary>
        private void validateGenericHarmony()
        {
            affirmTheseRelateCorrectly(typeof(BytePixel), typeof(System.Byte));
            affirmTheseRelateCorrectly(typeof(SbytePixel), typeof(System.SByte));
            affirmTheseRelateCorrectly(typeof(ShortPixel), typeof(System.Int16));
            affirmTheseRelateCorrectly(typeof(UshortPixel), typeof(System.UInt16));
            affirmTheseRelateCorrectly(typeof(IntPixel), typeof(System.Int32));
            affirmTheseRelateCorrectly(typeof(UintPixel), typeof(System.UInt32));
            affirmTheseRelateCorrectly(typeof(LongPixel), typeof(System.Int64));
            affirmTheseRelateCorrectly(typeof(UlongPixel), typeof(System.UInt64));
            affirmTheseRelateCorrectly(typeof(FloatPixel), typeof(System.Single));
            affirmTheseRelateCorrectly(typeof(DoublePixel), typeof(System.Double));
        }


        private bool disposedValue;

        public TPixel BufferPixel { get; set; }

        public double CellSize { get { return TheRaster.cellSize; } }

        private Dimensions _dimensions = default;
        public Dimensions Dimensions
        {
            get
            {
                if (default == _dimensions)
                {
                    _dimensions = new Dimensions(
                        TheRaster.numRows, TheRaster.numColumns);
                }
                return _dimensions;
            }
        }

        public void Close()
        {
            // Todo: Figure out what, if anything, goes here.
        }

        public TPixel GetAtPosition(int offset)
        {
            throw new NotImplementedException();
        }

        public void ReadBufferPixel()
        {
            offsetCounter++;
            TBasic pixelVal = TheRaster.GetValueAt(offsetCounter);
            Pixel f = default;
            this.BufferPixel = new TPixel();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)'
        //      has code to free unmanaged resources
        // ~RasterLandis()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        void IOutputRaster<TPixel>.SetAtPosition(int offset, TPixel value)
        {
            throw new NotImplementedException();
        }
    }
}
