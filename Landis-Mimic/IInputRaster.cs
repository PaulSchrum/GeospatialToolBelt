using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Landis_Mimic
{
    public interface IInputRaster<T> : System.IDisposable
        where T : struct
    {
        T BufferPixel { get; }
        T ReadBufferPixel();

        Dimensions Dimensions { get; }
    }
}
