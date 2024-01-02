using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTBelt
{
    internal class Auxiliaries
    {
        internal static int TypeSize(Type aType)
        {
            switch (Type.GetTypeCode(aType))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                    return sizeof(byte); // or sizeof(sbyte)
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    return sizeof(short); // or sizeof(ushort)
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    return sizeof(int); // or sizeof(uint)
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return sizeof(long); // or sizeof(ulong)
                case TypeCode.Single:
                    return sizeof(float);
                case TypeCode.Double:
                    return sizeof(double);
                case TypeCode.Decimal:
                    return sizeof(decimal);
                case TypeCode.Char:
                    return sizeof(char);
                case TypeCode.Boolean:
                    return sizeof(bool);
                default:
                    throw new ArgumentException("Unsupported type");
            }
        }
    }
}
