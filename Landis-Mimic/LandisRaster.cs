using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoTBelt;

namespace Landis_Mimic
{
    public class LandisRaster<T> : IInputRaster<T> where T : struct
        //, IOutputRaster<T> To do later.
    {
        internal LandisRaster() { }

        private Raster<T> raster;
        public Raster<T> theRaster { get; internal set; } = null;

        private int index = -1;
        private T buffPix = default;
        public T BufferPixel
        {
            get { return buffPix; }
        }

        protected Dimensions _dimensions = default;
        public Dimensions Dimensions
        {
            get
            {
                if(_dimensions == default(Dimensions))
                {
                    var rows = theRaster.Grid.rasterRows;
                    var cols = theRaster.Grid.rasterColumns;
                    _dimensions = 
                        new Dimensions(rows, cols);
                }
                return _dimensions;
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            if (theRaster is not null)
                return theRaster.numRows * theRaster.numColumns;
            return 0;
        }

        public T ReadBufferPixel()
        {
            if (Count() == 0)
                return (default);

            index++;
            if(index >= Count())
                throw new IndexOutOfRangeException();

            buffPix = theRaster.GetValueAt(index);
            return buffPix;
        }
    }
}
