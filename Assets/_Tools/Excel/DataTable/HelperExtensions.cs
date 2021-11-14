using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Library.Extensions;
using PLitJson;

namespace Excel
{
    public interface ImpHelper
    {
        string Extensions { get; }
        object ProcessData<T>(object obj);
        void Export(string savePath, Dictionary<int, List<Cell>> dic, string tableName);
    }

    public static partial class HelperExtensions
    {
        public static bool SetValue(this FieldInfo fileInfo, object obj, BinaryReader value, Type type)
        {
            //bool、int、float、double、long、decimal、byte、short、char、string

            switch (fileInfo.GetValue(obj))
            {
                case bool _: fileInfo.SetValue(obj, value.ReadBoolean()); break;
                case int _: fileInfo.SetValue(obj, value.ReadInt32()); break;
                case float _: fileInfo.SetValue(obj, value.ReadSingle()); break;
                case double _: fileInfo.SetValue(obj, value.ReadDouble()); break;
                case long _: fileInfo.SetValue(obj, value.ReadInt64()); break;
                case decimal _: fileInfo.SetValue(obj, value.ReadDecimal()); break;
                case byte _: fileInfo.SetValue(obj, value.ReadByte()); break;
                case short _: fileInfo.SetValue(obj, value.ReadInt16()); break;
                case char _: fileInfo.SetValue(obj, value.ReadChar()); break;
                case string _: fileInfo.SetValue(obj, value.ReadString()); break;

                case bool[] _: fileInfo.SetValue(obj, value.ReadString().ToBoolArray('|')); break;
                case int[] _: fileInfo.SetValue(obj, value.ReadString().ToIntArray('|')); break;
                case float[] _: fileInfo.SetValue(obj, value.ReadString().ToFloatArray('|')); break;
                case double[] _: fileInfo.SetValue(obj, value.ReadString().ToDoubleArray('|')); break;
                case long[] _: fileInfo.SetValue(obj, value.ReadString().ToLongArray('|')); break;
                case decimal[] _: fileInfo.SetValue(obj, value.ReadString().ToDecimalArray('|')); break;
                case byte[] _: fileInfo.SetValue(obj, value.ReadString().ToBoolArray('|')); break;
                case short[] _: fileInfo.SetValue(obj, value.ReadString().ToShortArray('|')); break;
                case char[] _: fileInfo.SetValue(obj, value.ReadString().ToCharArray()); break;
                case string[] _: fileInfo.SetValue(obj, value.ReadString().Split('|')); break;

                case List<bool> _: fileInfo.SetValue(obj, value.ReadString().ToBoolArray('|').ToList()); break;
                case List<int> _: fileInfo.SetValue(obj, value.ReadString().ToIntArray('|').ToList()); break;
                case List<float> _: fileInfo.SetValue(obj, value.ReadString().ToFloatArray('|').ToList()); break;
                case List<double> _: fileInfo.SetValue(obj, value.ReadString().ToDoubleArray('|').ToList()); break;
                case List<long> _: fileInfo.SetValue(obj, value.ReadString().ToLongArray('|').ToList()); break;
                case List<decimal> _: fileInfo.SetValue(obj, value.ReadString().ToDecimalArray('|').ToList()); break;
                case List<byte> _: fileInfo.SetValue(obj, value.ReadString().ToBoolArray('|').ToList()); break;
                case List<short> _: fileInfo.SetValue(obj, value.ReadString().ToShortArray('|').ToList()); break;
                case List<char> _: fileInfo.SetValue(obj, value.ReadString().ToCharArray().ToList()); break;
                case List<string> _: fileInfo.SetValue(obj, value.ReadString().Split('|').ToList()); break;

                default: fileInfo.SetValue(obj, null); break;
            }

            return true;
        }

        public static void SetValue(this FieldInfo fileInfo, object obj, string value, Type type)
        {
            switch (fileInfo.GetValue(obj))
            {
                case bool _: fileInfo.SetValue(obj, value.AsBool()); break;
                case int _: fileInfo.SetValue(obj, value.AsInt()); break;
                case float _: fileInfo.SetValue(obj, value.AsFloat()); break;
                case double _: fileInfo.SetValue(obj, value.AsDouble()); break;
                case long _: fileInfo.SetValue(obj, value.AsLong()); break;
                case decimal _: fileInfo.SetValue(obj, value.AsDecimal()); break;
                case byte _: fileInfo.SetValue(obj, value.AsByte()); break;
                case short _: fileInfo.SetValue(obj, value.AsShort()); break;
                case char _: fileInfo.SetValue(obj, value[0]); break;
                case string _: fileInfo.SetValue(obj, value); break;

                case bool[] _: fileInfo.SetValue(obj, value.ToBoolArray('|')); break;
                case int[] _: fileInfo.SetValue(obj, value.ToIntArray('|')); break;
                case float[] _: fileInfo.SetValue(obj, value.ToFloatArray('|')); break;
                case double[] _: fileInfo.SetValue(obj, value.ToDoubleArray('|')); break;
                case long[] _: fileInfo.SetValue(obj, value.ToLongArray('|')); break;
                case decimal[] _: fileInfo.SetValue(obj, value.ToDecimalArray('|')); break;
                case byte[] _: fileInfo.SetValue(obj, value.ToBoolArray('|')); break;
                case short[] _: fileInfo.SetValue(obj, value.ToShortArray('|')); break;
                case char[] _: fileInfo.SetValue(obj, value.ToCharArray()); break;
                case string[] _: fileInfo.SetValue(obj, value.Split('|')); break;

                case List<bool> _: fileInfo.SetValue(obj, value.ToBoolArray('|').ToList()); break;
                case List<int> _: fileInfo.SetValue(obj, value.ToIntArray('|').ToList()); break;
                case List<float> _: fileInfo.SetValue(obj, value.ToFloatArray('|').ToList()); break;
                case List<double> _: fileInfo.SetValue(obj, value.ToDoubleArray('|').ToList()); break;
                case List<long> _: fileInfo.SetValue(obj, value.ToLongArray('|').ToList()); break;
                case List<decimal> _: fileInfo.SetValue(obj, value.ToDecimalArray('|').ToList()); break;
                case List<byte> _: fileInfo.SetValue(obj, value.ToBoolArray('|').ToList()); break;
                case List<short> _: fileInfo.SetValue(obj, value.ToShortArray('|').ToList()); break;
                case List<char> _: fileInfo.SetValue(obj, value.ToCharArray().ToList()); break;
                case List<string> _: fileInfo.SetValue(obj, value.Split('|').ToList()); break;

                default: fileInfo.SetValue(obj, null); break;
            }
        }

        #region Binary

        public static void ReadValue(this BinaryReader value, string type)
        {
            switch (type)
            {
                case "bool": value.ReadBoolean(); break;
                case "int": value.ReadInt32(); break;
                case "float": value.ReadSingle(); break;
                case "double": value.ReadDouble(); break;
                case "long": value.ReadInt64(); break;
                case "decimal": value.ReadDecimal(); break;
                case "byte": value.ReadByte(); break;
                case "short": value.ReadSByte(); break;
                case "char": value.ReadChar(); break;
                case "string": value.ReadString(); break;
                default: value.ReadString(); break;
            }
        }

        public static void Write(this BinaryWriter writer, string value, string type)
        {
            type = type.ToLower();
            switch (type)
            {
                case "bool": writer.Write(value.AsBool()); break;
                case "int": writer.Write(value.AsInt()); break;
                case "float": writer.Write(value.AsFloat()); break;
                case "double": writer.Write(value.AsDouble()); break;
                case "long": writer.Write(value.AsLong()); break;
                case "decimal": writer.Write(value.AsDecimal()); break;
                case "byte": writer.Write(value.AsByte()); break;
                case "short": writer.Write(value.AsShort()); break;
                case "char":
                case "string":
                case "bool[]":
                case "int[]":
                case "float[]":
                case "double[]":
                case "long[]":
                case "decimal[]":
                case "byte[]":
                case "short[]":
                case "string[]":
                case "list<bool>":
                case "list<int>":
                case "list<float>":
                case "list<double>":
                case "list<long>":
                case "list<decimal>":
                case "list<byte>":
                case "list<short>":
                case "list<string>":
                default:
                    writer.Write(value); //直接写入字符串，读取的时候进行解析
                    break;
            }
        }

        #endregion

        #region JsonWriter

        public static void Write(this JsonWriter writer, string value, string type)
        {
            type = type.ToLower();
            switch (type)
            {
                case "bool": writer.Write(value.AsBool()); break;
                case "int": writer.Write(value.AsInt()); break;
                case "float": writer.Write(value.AsFloat()); break;
                case "double": writer.Write(value.AsDouble()); break;
                case "long": writer.Write(value.AsLong()); break;
                case "decimal": writer.Write(value.AsDecimal()); break;
                case "byte": writer.Write(value.AsByte()); break;
                case "short": writer.Write(value.AsShort()); break;
                case "string": 
                case "char": writer.Write(value); break;
                case "bool[]":
                case "int[]":
                case "float[]":
                case "double[]":
                case "long[]":
                case "decimal[]":
                case "byte[]":
                case "short[]":
                case "string[]":
                case "list<bool>":
                case "list<int>":
                case "list<float>":
                case "list<double>":
                case "list<long>":
                case "list<decimal>":
                case "list<byte>":
                case "list<short>":
                case "list<string>":
                    {
                        type = type.Replace("[]", "").Replace("list<", "").Replace(">", "");
                        writer.WriteArrayStart();
                        string[] array = value.Split('|');
                        for (var i = 0; i < array.Length; i++)
                        {
                            writer.Write(array[i], type);
                        }
                        writer.WriteArrayEnd();
                    }
                    break;
                default:
                    writer.Write(value);
                    break;
            }
        }

        #endregion
    }
}