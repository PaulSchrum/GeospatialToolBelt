using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landis_Mimic
{
    public interface IRaster : IDisposable
    {
        double CellSize { get; }
        Dimensions Dimensions { get; }
        void Close();
    }

    public interface IRaster<T> : IRaster
    {
        T BufferPixel { get; }
    }

    public interface IInputRaster<T> : IRaster<T>
    {
        void ReadBufferPixel();
        T GetAtPosition(int offset);
    }

    public interface IOutputRaster<T> : IRaster<T>
    {
        void SetAtPosition(int offset, T value);
    }

}
