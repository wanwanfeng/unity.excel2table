using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Library.Extensions;

namespace Excel
{
    public static class EditorExcelRead
    {
        public static bool SetValue(this FieldInfo fileInfo, object obj, BinaryReader value, Type type)
        {
            //bool、int、float、double、long、decimal、byte、short、char、string
            try
            {
                if (type == typeof (bool))
                {
                    fileInfo.SetValue(obj, value.ReadBoolean());
                }
                else if (type == typeof (int))
                {
                    fileInfo.SetValue(obj, value.ReadInt32());
                }
                else if (type == typeof (float))
                {
                    fileInfo.SetValue(obj, value.ReadSingle());
                }
                else if (type == typeof (double))
                {
                    fileInfo.SetValue(obj, value.ReadDouble());
                }
                else if (type == typeof (long))
                {
                    fileInfo.SetValue(obj, value.ReadInt64());
                }
                else if (type == typeof (decimal))
                {
                    fileInfo.SetValue(obj, value.ReadDecimal());
                }
                else if (type == typeof (byte))
                {
                    fileInfo.SetValue(obj, value.ReadByte());
                }
                else if (type == typeof (short))
                {
                    fileInfo.SetValue(obj, value.ReadSByte());
                }
                else if (type == typeof (char))
                {
                    fileInfo.SetValue(obj, value.ReadChar());
                }
                else if (type == typeof (string))
                {
                    fileInfo.SetValue(obj, value.ReadString());
                }
                else if (type == typeof (bool[]))
                {
                    fileInfo.SetValue(obj, value.ReadString().AsStringArray('|').AsBoolArray());
                }
                else if (type == typeof (int[]))
                {
                    fileInfo.SetValue(obj, value.ReadString().AsStringArray('|').AsIntArray());
                }
                else if (type == typeof (float[]))
                {
                    fileInfo.SetValue(obj, value.ReadString().AsStringArray('|').AsFloatArray());
                }
                else if (type == typeof (double[]))
                {
                    fileInfo.SetValue(obj, value.ReadString().AsStringArray('|').AsDoubleArray());
                }
                else if (type == typeof (long[]))
                {
                    fileInfo.SetValue(obj, value.ReadString().AsStringArray('|').AsLongArray());
                }
                else if (type == typeof (decimal[]))
                {
                    fileInfo.SetValue(obj, value.ReadString().AsStringArray('|').AsDecimalArray());
                }
                else if (type == typeof (byte[]))
                {
                    fileInfo.SetValue(obj, value.ReadString().AsStringArray('|').AsByteArray());
                }
                else if (type == typeof (short[]))
                {
                    fileInfo.SetValue(obj, value.ReadString().AsStringArray('|').AsShortArray());
                }
                else if (type == typeof (char[]))
                {
                    fileInfo.SetValue(obj, value.ReadString().ToCharArray());
                }
                else if (type == typeof (string[]))
                {
                    fileInfo.SetValue(obj, value.ReadString().AsStringArray());
                }
                else if (type == typeof (List<bool>))
                {
                    fileInfo.SetValue(obj, value.ReadString().AsStringArray('|').AsBoolArray().ToList());
                }
                else if (type == typeof (List<int>))
                {
                    fileInfo.SetValue(obj, value.ReadString().AsStringArray('|').AsIntArray().ToList());
                }
                else if (type == typeof (List<float>))
                {
                    fileInfo.SetValue(obj, value.ReadString().AsStringArray('|').AsFloatArray().ToList());
                }
                else if (type == typeof (List<double>))
                {
                    fileInfo.SetValue(obj, value.ReadString().AsStringArray('|').AsDoubleArray().ToList());
                }
                else if (type == typeof (List<long>))
                {
                    fileInfo.SetValue(obj, value.ReadString().AsStringArray('|').AsLongArray().ToList());
                }
                else if (type == typeof (List<decimal>))
                {
                    fileInfo.SetValue(obj, value.ReadString().AsStringArray('|').AsDecimalArray().ToList());
                }
                else if (type == typeof (List<byte>))
                {
                    fileInfo.SetValue(obj, value.ReadString().AsStringArray('|').AsByteArray().ToList());
                }
                else if (type == typeof (List<short>))
                {
                    fileInfo.SetValue(obj, value.ReadString().AsStringArray('|').AsShortArray().ToList());
                }
                else if (type == typeof (List<char>))
                {
                    fileInfo.SetValue(obj, value.ReadString().ToCharArray().ToList());
                }
                else if (type == typeof (List<string>))
                {
                    fileInfo.SetValue(obj, value.ReadString().AsStringArray().ToList());
                }
                else
                {
                    fileInfo.SetValue(obj, null);
                }
                return true;
            }
            catch (EndOfStreamException e)
            {
                //Debug.Log("已到末尾！" + e.Message);
                return false;
            }
            catch (Exception e)
            {
                //Debug.Log("出错！" + e.Message);
                return false;
            }
        }

        public static void ReadValue(this BinaryReader value, string type)
        {
            if (type == ("bool"))
            {
                value.ReadBoolean();
            }
            else if (type == ("int"))
            {
                value.ReadInt32();
            }
            else if (type == ("float"))
            {
                value.ReadSingle();
            }
            else if (type == ("double"))
            {
                value.ReadDouble();
            }
            else if (type == ("long"))
            {
                value.ReadInt64();
            }
            else if (type == ("decimal"))
            {
                value.ReadDecimal();
            }
            else if (type == ("byte"))
            {
                value.ReadByte();
            }
            else if (type == ("short"))
            {
                value.ReadSByte();
            }
            else if (type == ("char"))
            {
                value.ReadChar();
            }
            else if (type == ("string"))
            {
                value.ReadString();
            }
            else
            {
                value.ReadString();
            }
        }

        public static void SetValue(this FieldInfo fileInfo, object obj, string value, Type type)
        {
            if (type == typeof (bool))
            {
                fileInfo.SetValue(obj, value.AsBool());
            }
            else if (type == typeof (int))
            {
                fileInfo.SetValue(obj, value.AsInt());
            }
            else if (type == typeof (float))
            {
                fileInfo.SetValue(obj, value.AsFloat());
            }
            else if (type == typeof (double))
            {
                fileInfo.SetValue(obj, value.AsDouble());
            }
            else if (type == typeof (long))
            {
                fileInfo.SetValue(obj, value.AsLong());
            }
            else if (type == typeof (decimal))
            {
                fileInfo.SetValue(obj, value.AsDecimal());
            }
            else if (type == typeof (byte))
            {
                fileInfo.SetValue(obj, value.AsByte());
            }
            else if (type == typeof (short))
            {
                fileInfo.SetValue(obj, value.AsShort());
            }
            else if (type == typeof (char))
            {
                fileInfo.SetValue(obj, value.ToCharArray()[0]);
            }
            else if (type == typeof (string))
            {
                fileInfo.SetValue(obj, value);
            }
            else if (type == typeof (bool[]))
            {
                fileInfo.SetValue(obj, value.AsStringArray('|').AsBoolArray());
            }
            else if (type == typeof (int[]))
            {
                fileInfo.SetValue(obj, value.AsStringArray('|').AsIntArray());
            }
            else if (type == typeof (float[]))
            {
                fileInfo.SetValue(obj, value.AsStringArray('|').AsFloatArray());
            }
            else if (type == typeof (double[]))
            {
                fileInfo.SetValue(obj, value.AsStringArray('|').AsDoubleArray());
            }
            else if (type == typeof (long[]))
            {
                fileInfo.SetValue(obj, value.AsStringArray('|').AsLongArray());
            }
            else if (type == typeof (decimal[]))
            {
                fileInfo.SetValue(obj, value.AsStringArray('|').AsDecimalArray());
            }
            else if (type == typeof (byte[]))
            {
                fileInfo.SetValue(obj, value.AsStringArray('|').AsByteArray());
            }
            else if (type == typeof (short[]))
            {
                fileInfo.SetValue(obj, value.AsStringArray('|').AsShortArray());
            }
            else if (type == typeof (char[]))
            {
                fileInfo.SetValue(obj, value.ToCharArray());
            }
            else if (type == typeof (string[]))
            {
                fileInfo.SetValue(obj, value.AsStringArray());
            }
            else if (type == typeof (List<bool>))
            {
                fileInfo.SetValue(obj, value.AsStringArray('|').AsBoolArray().ToList());
            }
            else if (type == typeof (List<int>))
            {
                fileInfo.SetValue(obj, value.AsStringArray('|').AsIntArray().ToList());
            }
            else if (type == typeof (List<float>))
            {
                fileInfo.SetValue(obj, value.AsStringArray('|').AsFloatArray().ToList());
            }
            else if (type == typeof (List<double>))
            {
                fileInfo.SetValue(obj, value.AsStringArray('|').AsDoubleArray().ToList());
            }
            else if (type == typeof (List<long>))
            {
                fileInfo.SetValue(obj, value.AsStringArray('|').AsLongArray().ToList());
            }
            else if (type == typeof (List<decimal>))
            {
                fileInfo.SetValue(obj, value.AsStringArray('|').AsDecimalArray().ToList());
            }
            else if (type == typeof (List<byte>))
            {
                fileInfo.SetValue(obj, value.AsStringArray('|').AsByteArray().ToList());
            }
            else if (type == typeof (List<short>))
            {
                fileInfo.SetValue(obj, value.AsStringArray('|').AsShortArray().ToList());
            }
            else if (type == typeof (List<char>))
            {
                fileInfo.SetValue(obj, value.ToCharArray().ToList());
            }
            else if (type == typeof (List<string>))
            {
                fileInfo.SetValue(obj, value.AsStringArray().ToList());
            }
            else
            {
                fileInfo.SetValue(obj, null);
            }
        }

        #region 从Bytes读入

        public static List<T> FromBytes<T>(byte[] content) where T : class, new()
        {
            List<T> list = new List<T>();
            if (content.Length == 0) return list;

            var ms = new MemoryStream(content);
            var br = new BinaryReader(ms);
            var count = br.ReadInt32();
            var length = br.ReadInt32();

            var fieldList = new List<string>();
            var typeList = new List<string>();
            for (var i = 0; i < length; i++)
            {
                fieldList.Add(br.ReadString());
                typeList.Add(br.ReadString());
            }
            for (var i = 0; i < count; i++)
            {
                T classInstance = new T();
                for (var j = 0; j < length; j++)
                {
                    var field = fieldList[j];
                    var type = typeList[j];

                    var fileInfo = typeof (T).GetField(field);
                    if (fileInfo == null)
                    {
                        //跳过不存在的字段值以保持偏移
                        br.ReadValue(type);
                    }
                    else
                    {
                        fileInfo.SetValue(classInstance, br, fileInfo.FieldType);
                    }
                }
                list.Add(classInstance);
            }
            br.Close();
            ms.Dispose();
            return list;
        }

        #endregion

        #region 从Txt读入

        public static List<T> FromTxt<T>(string content) where T : class, new()
        {
            List<T> list = new List<T>();
            if (string.IsNullOrEmpty(content)) return list;
            string[] results = content.Replace("\r\n", "\n").Split('\n');
            T classInstance = default(T);

            foreach (var line in results)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                if (line.StartsWith("#BEGIN"))
                {
                    classInstance = new T();
                    continue;
                }
                if (line.StartsWith("#END"))
                {
                    list.Add(classInstance);
                    continue;
                }
                var result = line.Split(':');
                if (result.Length != 2)
                    continue;
                var fileInfo = typeof (T).GetField(result[0]);
                if (fileInfo == null)
                    continue;
                fileInfo.SetValue(classInstance, result[1], fileInfo.FieldType);
            }
            return list;
        }

        #endregion

        #region 从Csv读入

        public static List<T> FromCsv<T>(string content) where T : class, new()
        {
            List<T> list = new List<T>();
            if (string.IsNullOrEmpty(content)) return list;
            string[] results = content.Replace("\r\n", "\n").Split('\n');
            var fieldList = results[0].AsStringArray();
            var typeList = results[1].AsStringArray();
            for (var i = 2; i < results.Length; i++)
            {
                string line = results[i];
                if (string.IsNullOrEmpty(line))
                    continue;
                string[] result = line.AsStringArray();
                if (result.Length == 0) continue;
                T classInstance = new T();
                for (var j = 0; j < result.Length; j++)
                {
                    var fileInfo = typeof (T).GetField(fieldList[j]);
                    if (fileInfo == null || string.IsNullOrEmpty(result[j]))
                        continue;
                    fileInfo.SetValue(classInstance, result[j], fileInfo.FieldType);
                }
                list.Add(classInstance);
            }
            return list;
        }

        #endregion


        #region 从XmlAttribute读入

        internal static List<T> FromXmlAttribute<T>(string content) where T : class, new()
        {
            var list = new List<T>();
            if (string.IsNullOrEmpty(content)) return list;

            var document = new XmlDocument();
            document.LoadXml(content);

            var root = document.DocumentElement;
            if (root == null) return list;

            var table = root.SelectNodes("Table");
            if (table == null) return list;

            foreach (XmlElement node in table)
            {
                if (!node.HasAttributes)
                {
                    continue;
                }
                T classInstance = new T();
                var fieldInfos = typeof (T).GetFields();
                foreach (var fieldInfo in fieldInfos)
                {
                    if (node.HasAttribute(fieldInfo.Name))
                    {
                        fieldInfo.SetValue(classInstance, node.GetAttribute(fieldInfo.Name), fieldInfo.FieldType);
                    }
                }
                list.Add(classInstance);
            }
            return list;
        }

        #endregion
    }
}