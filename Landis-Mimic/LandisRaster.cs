using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoTBelt;
using GeoTBelt.GeoTiff;

namespace Landis_Mimic
{
    public class LandisRaster<T> : IInputRaster<T>, IOutputRaster<T>
        where T : struct
        {
        internal LandisRaster() { }

        protected bool disposed = false;
        public GeoTiffRaster<T> theRaster { get; internal set; } = null;

        private int index = -1;
        private T buffPix = default;
        public T BufferPixel
        {
            get { return buffPix; }
            set { buffPix = value; }
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

            set { _dimensions = value; }
        }

        public void SaveAs(string fileName=null)
        {
            try
            {
                if(fileName is not null)
                    theRaster.SaveAs(theRaster.fileName);
                else
                    theRaster.SaveAs(theRaster.fileName);
            }
            catch (IOException ex) 
            { 
                
            }
        }

        public void Dispose()
        {
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

        public void WriteBufferPixel()
        {
            if(++index >= Count())
                throw new IndexOutOfRangeException();
            theRaster.SetValueAtLinearIndex(BufferPixel, index);
        }

    }
}
