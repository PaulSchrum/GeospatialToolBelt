using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTBelt
{
    public class Band
    {
        public dynamic[,] CellArray { get; protected set; }
        public Raster MyParent { get; protected set; }
        public Type Type { get; set; } = typeof(double);

        public Band(Raster parent)
        { 
            MyParent = parent;
        }

        public void CreateCellArray(int rowCount, int columnCount)
        {
            CellArray = new dynamic[rowCount, columnCount];
        }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            var NoDataValue = MyParent.NoDataValue;
            for (int currentRow = 0; currentRow < MyParent.numRows; currentRow++)
            {
                for (int currentColumn = 0; currentColumn < MyParent.numColumns; currentColumn++)
                {
                    if (CellArray[currentRow, currentColumn] == double.NaN)
                    {
                        output.Append(NoDataValue + " ");
                    }
                    else
                    {
                        string outValue = $"{CellArray[currentRow, currentColumn]:0.###} ";
                        output.Append(outValue);
                    }
                }
                output.AppendLine("");
            }
            return output.ToString();
        }
    }
}
