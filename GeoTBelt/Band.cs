using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace GeoTBelt
{
    public class Band
    {
        public dynamic[] CellArray { get; protected set; }
        public Raster MyParent { get; protected set; }
        public Type theType { get; set; } = typeof(double);

        private Band() { }

        public Band(Raster parent)
        { 
            MyParent = parent;
            theType = null;
            CellArray = null;
        }

        public Band(Raster parent, dynamic[] dataFrame, Type type,
            int numRows, int numCols)
            : this(parent)
        {
            CellArray = dataFrame;
            theType = type;
        }

        public Band (Raster parent, Byte[] dataFrameAsBytes, Type aType,
                       int numRows, int numCols)
            : this(parent)
        {
            int typeSize = Auxiliaries.TypeSize(aType);
            theType = aType;
            int numCells = dataFrameAsBytes.Length / typeSize;
            if (theType.Equals(typeof(Byte)))
            {
                CellArray = dataFrameAsBytes.Cast<dynamic>().ToArray();
            }
            else if(theType.Equals(typeof(System.UInt16)))
            {
                // how assign dataFramAsBytes to CellArray using BitConverter?
                CellArray = new dynamic[numCells];
                for (int i = 0; i < numCells; i++)
                {
                    CellArray[i] =  BitConverter.ToUInt16(dataFrameAsBytes, i * typeSize);
                }
            }
            else if (theType.Equals(typeof(System.Int16)))
            {
                CellArray = new dynamic[numCells];
                for (int i = 0; i < numCells; i++)
                {
                    CellArray[i] = BitConverter.ToInt16(dataFrameAsBytes, i * typeSize);
                }
            }
            else if (theType.Equals(typeof(System.UInt32)))
            {
                CellArray = new dynamic[numCells];
                for (int i = 0; i < numCells; i++)
                {
                    CellArray[i] = BitConverter.ToUInt32(dataFrameAsBytes, i * typeSize);
                }
            }
            else if (theType.Equals(typeof(System.Int32)))
            {
                CellArray = new dynamic[numCells];
                for (int i = 0; i < numCells; i++)
                {
                    CellArray[i] = BitConverter.ToInt32(dataFrameAsBytes, i * typeSize);
                }
            }
            else if (theType.Equals(typeof(System.UInt64)))
            {
                CellArray = new dynamic[numCells];
                for (int i = 0; i < numCells; i++)
                {
                    CellArray[i] = BitConverter.ToUInt64(dataFrameAsBytes, i * typeSize);
                }
            }
            else if (theType.Equals(typeof(System.Int64)))
            {
                CellArray = new dynamic[numCells];
                for (int i = 0; i < numCells; i++)
                {
                    CellArray[i] = BitConverter.ToInt64(dataFrameAsBytes, i * typeSize);
                }
            }
            else if (theType.Equals(typeof(System.Single)))
            {
                CellArray = new dynamic[numCells];
                for (int i = 0; i < numCells; i++)
                {
                    CellArray[i] = BitConverter.ToSingle(dataFrameAsBytes, i * typeSize);
                }
            }
            else if (theType.Equals(typeof(System.Double)))
            {
                CellArray = new dynamic[numCells];
                for (int i = 0; i < numCells; i++)
                {
                    CellArray[i] = BitConverter.ToDouble(dataFrameAsBytes, i * typeSize);
                }
            }
            

        }

        /// <summary>
        /// Gets the index of a linear array as if it was parsed as
        ///     a multi-dimensional array.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private int IndexAsLinearArray(params int[] idx)
        {
            int? depIdx = null;
            int? colIdx = null;
            int rowIdx = idx[0];
            if (idx.Length > 1) { colIdx = idx[1]; }
            if (idx.Length > 2) { depIdx = idx[2]; }

            switch (idx.Length)
            {
                case 1:
                    return rowIdx;

                case 2:
                {
                    if (rowIdx >= MyParent.numRows && colIdx >= MyParent.numColumns)
                    {
                        throw new IndexOutOfRangeException(
                        $"Row index {rowIdx} and column index {colIdx} are out of range. " +
                        $"Size of the raster is [{MyParent.numRows}, {MyParent.numColumns}]. ");
                    }
                    if(rowIdx >= MyParent.numRows)
                    {
                        throw new IndexOutOfRangeException(
                        $"Row index {rowIdx} is out of range. " +
                        $"Number of rows is {MyParent.numRows}.");
                    }
                    if(colIdx >= MyParent.numColumns)
                    {
                        throw new IndexOutOfRangeException(
                        $"Column index {colIdx} is out of range. " +
                        $"Number of columns is {MyParent.numColumns}.");
                    }
                    int linearIndex = rowIdx * MyParent.numColumns;
                    linearIndex += colIdx ?? default;
                    return linearIndex;
                }
                case 3:
                {   // technical debt: case of 3D raster
                    throw new NotImplementedException();
                }
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Read value of individual grid cell by index. Row major, column minor,
        ///     depth last. 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public dynamic At(params int[] idx)
        {
            return CellArray[IndexAsLinearArray(idx)];
        }

        /// <summary>
        /// Set value of individual grid cell by index. Row major, column minor,
        ///     depth last. 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="value"></param>
        public void Set(dynamic value, params int[] idx)
        {
            CellArray[IndexAsLinearArray(idx)] = value;
        }

        public void CreateCellArray(int rowCount, int columnCount)
        {
            CellArray = new dynamic[rowCount * columnCount];
        }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            var NoDataValue = MyParent.NoDataValue;
            for (int currentRow = 0; currentRow < MyParent.numRows; currentRow++)
            {
                for (int currentColumn = 0; currentColumn < MyParent.numColumns; currentColumn++)
                {
                    if (false) //At([currentRow, currentColumn]) ))
                    {  // Technical debt: Figure out how to test for NoDataValue
                        output.Append(NoDataValue + " ");
                    }
                    else
                    {
                        string outValue = $"{At(currentRow, currentColumn):0.###} ";
                        output.Append(outValue);
                    }
                }
                output.AppendLine("");
            }
            return output.ToString();
        }

        private int typeSizeInBits
        {
            get
            {
                return typeSizeInBytes * 8;
            }
        }

        private int typeSizeInBytes
        {
            get
            {
                return Auxiliaries.TypeSize(this.theType);
            }
        }

    }
}
