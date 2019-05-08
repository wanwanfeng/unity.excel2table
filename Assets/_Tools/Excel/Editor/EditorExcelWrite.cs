using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Library.Extensions;
using LitJson;
using UnityEditor;

namespace Excel
{
    internal static class EditorExcelWrite
    {
        #region BinaryWriter

        public static void Write(this BinaryWriter writer, string value, string type)
        {
            type = type.ToLower();
            switch (type)
            {
                case "bool":
                    writer.Write(value.AsBool());
                    break;
                case "int":
                    writer.Write(value.AsInt());
                    break;
                case "float":
                    writer.Write(value.AsFloat());
                    break;
                case "double":
                    writer.Write(value.AsDouble());
                    break;
                case "long":
                    writer.Write(value.AsLong());
                    break;
                case "decimal":
                    writer.Write(value.AsDecimal());
                    break;
                case "byte":
                    writer.Write(value.AsByte());
                    break;
                case "short":
                    writer.Write(value.AsShort());
                    break;
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
                case "bool":
                    writer.Write(value.AsBool());
                    break;
                case "int":
                    writer.Write(value.AsInt());
                    break;
                case "float":
                    writer.Write(value.AsFloat());
                    break;
                case "double":
                    writer.Write(value.AsDouble());
                    break;
                case "long":
                    writer.Write(value.AsLong());
                    break;
                case "decimal":
                    writer.Write(value.AsDecimal());
                    break;
                case "byte":
                    writer.Write(value.AsByte());
                    break;
                case "short":
                    writer.Write(value.AsShort());
                    break;
                case "string":
                case "char":
                    writer.Write(value);
                    break;
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
                    string[] array = value.AsStringArray('|');
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

        #region Cell

        public static string Write(this Cell cell)
        {
            if (string.IsNullOrEmpty(cell.value))
            {
                return "";
            }
            string name = cell.name;
            string value = cell.value;
            string type = cell.type.ToLower();
            switch (type)
            {
                case "bool":
                case "int":
                case "float":
                case "double":
                case "long":
                case "decimal":
                case "byte":
                case "short":
                    return string.Format("{0} = {1}", name, value);
                    break;
                case "string":
                case "char":
                    return string.Format("{0} = \"{1}\"", name, value);
                    break;
                case "bool[]":
                case "int[]":
                case "float[]":
                case "double[]":
                case "long[]":
                case "decimal[]":
                case "byte[]":
                case "short[]":
                case "list<bool>":
                case "list<int>":
                case "list<float>":
                case "list<double>":
                case "list<long>":
                case "list<decimal>":
                case "list<byte>":
                case "list<short>":
                {
                    type = type.Replace("[]", "").Replace("list<", "").Replace(">", "");
                    return string.Format("{0} = new {1}[]{2}{3}{4}", name, type, "{", value.Replace("|", ","), "}");
                }
                case "string[]":
                case "list<string>":
                {
                    type = type.Replace("[]", "").Replace("list<", "").Replace(">", "");
                    return string.Format("{0} = new {1}[]{2}{3}{4}", name, type, "{", value.Replace("|", ","), "}");
                }
                    return "";
                default:
                    return string.Format("{0} {1} = {2}", type, name, value);
            }
            return "";
        }

        #endregion

        /// <summary>
        /// 分割字符串(返回前缀)
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="suffix">后缀</param>
        internal static string Get(string str, out string suffix)
        {
            suffix = "";
            for (var i = str.Length - 1; i >= 0; i--)
            {
                if (char.IsDigit(str, i))
                {
                    suffix = str[i] + suffix;
                }
                else
                {
                    break;
                }
            }

            return string.IsNullOrEmpty(suffix) ? str : str.Replace(suffix, "");
        }

        #region 写入为XML属性表

        /// <summary>
        /// 写入为XML属性表
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        internal static XmlDocument ToXmlAttribute(string savePath, Dictionary<int, List<Cell>> dic)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement rootElement = xmlDoc.CreateElement("Root");
            xmlDoc.AppendChild(rootElement);

            foreach (var pair in dic)
            {
                XmlElement node = xmlDoc.CreateElement("Table");
                foreach (var cell in pair.Value)
                {
                    if (string.IsNullOrEmpty(cell.value))
                        continue;

                    string suffix = "";
                    string prefix = Get(cell.name, out suffix);
                    if (prefix.Equals(cell.name))
                    {
                        //不以数字结尾
                        node.SetAttribute(cell.name, cell.value);
                    }
                    else
                    {
                        //以数字结尾  
                        if (node.HasAttribute(prefix))
                        {
                            string value = node.GetAttribute(prefix);
                            node.SetAttribute(prefix, value + "|" + cell.value);
                        }
                        else
                        {
                            node.SetAttribute(prefix, cell.value);
                        }
                    }
                }
                //有属性才加入
                if (node.HasAttributes)
                {
                    rootElement.AppendChild(node);
                }
            }
            xmlDoc.Save(savePath);
            return xmlDoc;
        }

        #endregion

        #region 写入为XML键值表

        /// <summary>
        /// 写入为XML键值表
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        internal static XmlDocument ToXml(string savePath, Dictionary<int, List<Cell>> dic)
        {
            FileInfo fileInfo = new FileInfo(savePath);
            string info = fileInfo.Name.Replace(fileInfo.Extension, "").Replace("Table", "Info");
            XmlDocument xmlDoc = new XmlDocument();

            XmlElement rootElement = xmlDoc.CreateElement("ArrayOf" + info);
            xmlDoc.AppendChild(rootElement);

            XmlDeclaration xmldecl = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", "");
            xmlDoc.InsertBefore(xmldecl, rootElement);

            foreach (var pair in dic)
            {
                XmlElement node = xmlDoc.CreateElement(info);
                foreach (var cell in pair.Value)
                {
                    if (string.IsNullOrEmpty(cell.value) || cell.value.ToLower().Equals("null"))
                    {
                        continue;
                    }

                    string suffix = "";
                    string prefix = Get(cell.name, out suffix);
                    if (prefix.Equals(cell.name))
                    {
                        //不以数字结尾
                        XmlElement node2 = xmlDoc.CreateElement(cell.name);
                        node2.InnerText = cell.value;
                        if (!string.IsNullOrEmpty(node2.InnerText))
                        {
                            node.AppendChild(node2);
                        }
                    }
                    else
                    {
                        //以数字结尾   
                        XmlNode node2 = node.SelectSingleNode(prefix);
                        if (node2 == null)
                        {
                            node2 = xmlDoc.CreateElement(prefix);
                            node.AppendChild(node2);
                        }
                        XmlNode node3 = xmlDoc.CreateElement(cell.type);
                        node3.InnerText = cell.value;
                        if (!string.IsNullOrEmpty(node3.InnerText))
                        {
                            node2.AppendChild(node3);
                        }

                        //有孩子节点才加入
                        if (node2.ChildNodes.Count == 0)
                        {
                            node.RemoveChild(node2);
                        }
                    }

                    //有孩子节点才加入
                    if (node.ChildNodes.Count != 0)
                    {
                        rootElement.AppendChild(node);
                    }
                }
                //有孩子节点才加入
                if (node.ChildNodes.Count != 0)
                {
                    rootElement.AppendChild(node);
                }
            }

            xmlDoc.Save(savePath);
            return xmlDoc;
        }

        #endregion

        #region  写入为CSV逗号分隔文本

        /// <summary>
        /// 写入为逗号分隔文本
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="dic"></param>
        internal static void ToCsv(string savePath, Dictionary<int, List<Cell>> dic)
        {
            StreamWriter sw = new StreamWriter(savePath);
            bool init = false;
            foreach (var pair in dic)
            {
                if (!init)
                {
                    init = true;
                    sw.WriteLine(string.Join(",", pair.Value.Select(p => p.name).ToArray()));
                    sw.WriteLine(string.Join(",", pair.Value.Select(p => p.type).ToArray()));
                }
                sw.WriteLine(string.Join(",", pair.Value.Select(p => p.value).ToArray()));
            }
            sw.Dispose();
            sw.Close();
        }

        #endregion

        #region  写入换行文本

        /// <summary>
        /// 写入为逗号分隔文本
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="dic"></param>
        internal static void ToTxt(string savePath, Dictionary<int, List<Cell>> dic)
        {
            StreamWriter sw = new StreamWriter(savePath);
            foreach (var pair in dic)
            {
                sw.WriteLine("#BEGIN");
                foreach (var cell in pair.Value)
                {
                    sw.WriteLine(cell.name + ":" + cell.value);
                }
                sw.WriteLine("#END");
            }
            sw.Dispose();
            sw.Close();
        }

        #endregion

        #region  写入为Bytes

        internal static void ToBytes(string savePath, Dictionary<int, List<Cell>> dic)
        {
            FileStream sw = new FileStream(savePath, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(sw);
            bw.Write(dic.Count);
            bool init = false;
            foreach (var pair in dic)
            {
                if (!init)
                {
                    init = true;
                    bw.Write(pair.Value.Count);
                    foreach (var cell in pair.Value)
                    {
                        bw.Write(cell.name);
                        bw.Write(cell.type); //写入是为了读取时发现不存在的字段时跳过以保持偏移
                    }
                }
                foreach (var cell in pair.Value)
                {
                    bw.Write(cell.value, cell.type);
                }
            }

            bw.Close();
            sw.Dispose();
        }

        #endregion

        #region  写入为Json表

        /// <summary>
        /// 写入为Json表
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="dic"></param>
        internal static void ToJson(string savePath, Dictionary<int, List<Cell>> dic)
        {
            StringWriter sw = new StringWriter();
            JsonWriter writer = new JsonWriter(sw);

            writer.WriteArrayStart();
            foreach (var pair in dic)
            {
                writer.WriteObjectStart();
                foreach (var cell in pair.Value)
                {
                    writer.WritePropertyName(cell.name);
                    writer.Write(cell.value, cell.type);
                }
                writer.WriteObjectEnd();
            }
            writer.WriteArrayEnd();

            sw.Dispose();
            sw.Close();
            File.WriteAllText(savePath, writer.TextWriter.ToString(), Encoding.UTF8);
        }

        #endregion

        #region  写入为Asset

        internal static void ToAsset(string savePath, Dictionary<int, List<Cell>> dic)
        {
            string tableName = new FileInfo(savePath).Name.Replace(".asset", "");
            ScriptableObject table = EditorUtils.CreateAsset<ScriptableObject>(tableName,
                savePath.Replace(EditorUtils.DataPath, ""));

            IDataCollection db = table as IDataCollection;
            if (db == null) return;
            db.Clear();
            var type = EditorUtils.GetType(tableName.Replace("Table", "Info"));

            foreach (var pair in dic)
            {
                var classInstance = type.Assembly.CreateInstance(type.FullName);
                foreach (var cell in pair.Value)
                {
                    var fileInfo = type.GetField(cell.name);
                    if (fileInfo == null) continue;
                    if (string.IsNullOrEmpty(cell.value)) continue;
                    fileInfo.SetValue(classInstance, cell.value, fileInfo.FieldType);
                }
                db.Add(classInstance);
            }

            EditorUtility.SetDirty(table);
            AssetDatabase.SaveAssets();
        }

        #endregion

        #region  写入Lua文本

        /// <summary>
        /// 写入为逗号分隔文本
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="dic"></param>
        internal static void ToLua(string savePath, Dictionary<int, List<Cell>> dic)
        {
            StreamWriter sw = new StreamWriter(savePath);
            foreach (var pair in dic)
            {
                sw.WriteLine("#BEGIN");
                foreach (var cell in pair.Value)
                {
                    sw.WriteLine(cell.name + ":" + cell.value);
                }
                sw.WriteLine("#END");
            }
            sw.Dispose();
            sw.Close();
        }

        #endregion

        internal static bool To(ExportType type, string savePath, Dictionary<int, List<Cell>> dic)
        {
            var first =
                typeof (ExportType).GetField(type.ToString())
                    .GetCustomAttributes(false)
                    .OfType<DescriptionAttribute>()
                    .FirstOrDefault();
            if (first != null)
            {
                savePath = Path.ChangeExtension(savePath, first.Description);
            }
            else
            {
                Debug.LogError("获取路径扩展名出错");
                return false;
            }

            switch (type)
            {
                case ExportType.ToXml:
                    ToXml(savePath, dic);
                    break;
                case ExportType.ToXmlAttribute:
                    ToXmlAttribute(savePath, dic);
                    break;
                case ExportType.ToJson:
                    ToJson(savePath, dic);
                    break;
                case ExportType.ToCsv:
                    ToCsv(savePath, dic);
                    break;
                case ExportType.ToTxt:
                    ToTxt(savePath, dic);
                    break;
                case ExportType.ToBytes:
                    ToBytes(savePath, dic);
                    break;
                case ExportType.ToAsset:
                    ToAsset(savePath, dic);
                    break;
                case ExportType.ToLua:
                    ToLua(savePath, dic);
                    break;
                default:
                    Debug.Log(Path.GetFileName(savePath) + "导表失败!");
                    return false;
            }
            return true;
        }
    }
}