using BitMiracle.LibTiff.Classic;
using System.Runtime.InteropServices;
using System.Text;

namespace GeoTBelt.GeoTiff
{
    internal record GTBTifTag
    {
        public TiffTag TagId { get; init; }
        public string TagName { get; init; }
        public dynamic value { get; init; }
        public Type valuesType { get; init; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{TagId.ToString()} Type: {valuesType}   Val: {value}");
            return sb.ToString();
        }

        private dynamic[][] parseArrayOfNumbersToMatrix
            (byte[] bytes, Type expectedType, int columns, out int rows, out bool IsPerfectFit)
        {
            if (!expectedType.IsPrimitive)
            {
                throw new ArgumentException("The expected type must be a primitive type.");
            }

            int sizeInBytes = Marshal.SizeOf(expectedType);
            int remainder = bytes.Length % columns;

            int rawRowWidth = bytes.Length / (sizeInBytes * columns);

            dynamic[][] returnArray = new dynamic[rawRowWidth][];

            throw new NotImplementedException();
        }

        public static bool IsThere(Tiff tif, int tagAsInteger)
        {
            TiffTag tagId = (TiffTag)tagAsInteger;

            FieldValue[] value = tif.GetField(tagId);
            if (value == null) { return false; }
            return true;
        }


        public static GTBTifTag Create(Tiff tif, int tagAsInteger) // TiffTag tagId, dynamic tagValue)
        {
            TiffTag tagId = (TiffTag)tagAsInteger;

            var test = new Dictionary<dynamic, string>();
            var otherTest = AllTags.Tag("SubfileType");

            FieldValue[] value = tif.GetField(tagId);
            if (value == null) { return null; }

            throw new NotImplementedException();

            dynamic tagValue = value;

            if (!tagValue.GetType().IsArray)
            {
                throw new NotImplementedException();
            }
            else if (tagValue.Length > 1)
            {
                Type elementType = tagValue.GetType().GetElementType();
                if (elementType == typeof(double))
                {

                    return new GTBTifTag
                    {
                        TagId = tagId,
                        TagName = tagId.ToString(),
                        valuesType = typeof(byte[]),
                        value = (byte[])tagValue
                    };
                }
                else if (elementType == typeof(string))
                {
                    int oij = 88;
                    return new GTBTifTag
                    {
                        TagId = tagId,
                        TagName = tagId.ToString(),
                        valuesType = typeof(string[]),
                        value = tagValue
                    };
                }
                else
                {
                    //throw new NotImplementedException();
                    return null;
                }


                throw new NotImplementedException();
            }
            else
            {
                dynamic tempValue = tagValue[0].Value;
                Type aType = tempValue.GetType();
                if (tempValue is double) aType = typeof(double);
                else if (tempValue is float) aType = typeof(float);
                else if (tempValue is short) aType = typeof(short);
                else if (tempValue is int) aType = typeof(int);
                else if (tempValue is long) aType = typeof(long);
                else if (tempValue is ulong) aType = typeof(ulong);
                else if (tempValue is byte) aType = typeof(byte);
                else if (tempValue is bool) aType = typeof(bool);
                else if (tempValue is decimal) aType = typeof(decimal);
                else if (tempValue is char) aType = typeof(char);
                else if (tempValue is sbyte) aType = typeof(sbyte);
                else if (tempValue is uint) aType = typeof(uint);
                else if (tempValue is ushort) aType = typeof(ushort);
                // else if ((long)tempValue > int.MaxValue) aType = typeof(long);
                else aType = typeof(int);

                return new GTBTifTag
                {
                    TagId = tagId,
                    TagName = tagId.ToString(),
                    value = tempValue,
                    valuesType = aType
                };
            }


            return new GTBTifTag
            {
                TagId = tagId,
                value = tagValue
            };
        }
    }
}
