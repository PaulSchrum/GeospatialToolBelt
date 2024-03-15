using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace GeoTBelt
{
    public class Band
    {
        protected dynamic[] tempCellArray { get; set; }
        public Raster MyParent { get; protected set; }
        public Type theType { get; set; } = typeof(double);

        private int theTypeSize_ = 0;
        public int theTypeSize 
        { 
            get
            {
                if(theTypeSize_ == 0)
                    theTypeSize_ = Marshal.SizeOf(theType);
                return theTypeSize_;
            }
        }
        public Byte[] CellArray { get; protected set; }

        private Band() { }

        public Band(Raster parent)
        { 
            MyParent = parent;
            theType = null;
            tempCellArray = null;
        }

        public Band(Raster parent, dynamic[] dataFrame, Type type,
            int numRows, int numCols)
            : this(parent)
        {
            tempCellArray = dataFrame;
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
                tempCellArray = dataFrameAsBytes.Cast<dynamic>().ToArray();
            }
            else if(theType.Equals(typeof(System.UInt16)))
            {
                // how assign dataFramAsBytes to CellArray using BitConverter?
                tempCellArray = new dynamic[numCells];
                for (int i = 0; i < numCells; i++)
                {
                    tempCellArray[i] =  BitConverter.ToUInt16(dataFrameAsBytes, i * typeSize);
                }
            }
            else if (theType.Equals(typeof(System.Int16)))
            {
                tempCellArray = new dynamic[numCells];
                for (int i = 0; i < numCells; i++)
                {
                    tempCellArray[i] = BitConverter.ToInt16(dataFrameAsBytes, i * typeSize);
                }
            }
            else if (theType.Equals(typeof(System.UInt32)))
            {
                tempCellArray = new dynamic[numCells];
                for (int i = 0; i < numCells; i++)
                {
                    tempCellArray[i] = BitConverter.ToUInt32(dataFrameAsBytes, i * typeSize);
                }
            }
            else if (theType.Equals(typeof(System.Int32)))
            {
                tempCellArray = new dynamic[numCells];
                for (int i = 0; i < numCells; i++)
                {
                    tempCellArray[i] = BitConverter.ToInt32(dataFrameAsBytes, i * typeSize);
                }
            }
            else if (theType.Equals(typeof(System.UInt64)))
            {
                tempCellArray = new dynamic[numCells];
                for (int i = 0; i < numCells; i++)
                {
                    tempCellArray[i] = BitConverter.ToUInt64(dataFrameAsBytes, i * typeSize);
                }
            }
            else if (theType.Equals(typeof(System.Int64)))
            {
                tempCellArray = new dynamic[numCells];
                for (int i = 0; i < numCells; i++)
                {
                    tempCellArray[i] = BitConverter.ToInt64(dataFrameAsBytes, i * typeSize);
                }
            }
            else if (theType.Equals(typeof(System.Single)))
            {
                tempCellArray = new dynamic[numCells];
                for (int i = 0; i < numCells; i++)
                {
                    tempCellArray[i] = BitConverter.ToSingle(dataFrameAsBytes, i * typeSize);
                }
            }
            else if (theType.Equals(typeof(System.Double)))
            {
                tempCellArray = new dynamic[numCells];
                for (int i = 0; i < numCells; i++)
                {
                    tempCellArray[i] = BitConverter.ToDouble(dataFrameAsBytes, i * typeSize);
                }
            }
            

        }

        /// <summary>
        /// Must be called after the dynamic[] tempCellArray has been populated.
        /// This method takes the contents of tempCellArray and puts them in CellArray.
        /// It then destructs tempCellArry so only CellArray holds the data.
        /// </summary>
        public void PopulatingIsComplete()
        {
            int rawArraySize = tempCellArray.Length * this.theTypeSize;
            CellArray = new byte[rawArraySize];

            for (int tempIdx = 0; tempIdx < tempCellArray.Length; tempIdx++)
            {
                int byteIdx = tempIdx * this.theTypeSize;
                dynamic aCellValue = tempCellArray[tempIdx];

                byte[] bytes;
                if (theType == typeof(int))
                {
                    bytes = BitConverter.GetBytes((int)aCellValue);
                }
                else if (theType == typeof(float))
                {
                    bytes = BitConverter.GetBytes((float)aCellValue);
                }
                else if (theType == typeof(double))
                {
                    bytes = BitConverter.GetBytes((double)aCellValue);
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported type {theType.FullName}");
                }

                Buffer.BlockCopy(bytes, 0, CellArray, byteIdx, bytes.Length);
            }

            tempCellArray = null;
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
            return tempCellArray[IndexAsLinearArray(idx)];
        }


        public double GetAsDouble(params int[] idx) 
        {
            int rawIndex = IndexAsLinearArray(idx) * theTypeSize;
            return BitConverter.ToDouble(CellArray, rawIndex);
        }

        /// <summary>
        /// Set value of individual grid cell by index. Row major, column minor,
        ///     depth last. 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="value"></param>
        public void Set(dynamic value, params int[] idx)
        {
            tempCellArray[IndexAsLinearArray(idx)] = value;
        }

        public void CreateCellArray(int rowCount, int columnCount)
        {
            tempCellArray = new dynamic[rowCount * columnCount];
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
