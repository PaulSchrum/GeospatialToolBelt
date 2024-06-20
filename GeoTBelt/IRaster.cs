using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTBelt
{
    public interface IRaster<T> where T : struct
    {
        public T GetValueAtRowColumn(int row, int column, int band = 0);
        public T GetValueAt(int offset);
        public void SetValueAtRowColumn(T value, int row, int column, int band = 0);
        public void SetValueAtLinearIndex(T value, int offset, int band = 0);
    }
}
