using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Excel
{
    public abstract partial class Helper
    {

        public partial class XmlAtt : ImpHelper
        {
            public int count { get; set; }
            public string Extensions { get { return ".xml"; } }
            IEnumerable ImpHelper.ProcessData<T>(object obj)
            {
                if (obj is byte[] bytes)
                {
                    var content = System.Text.Encoding.UTF8.GetString(bytes);
                    if (string.IsNullOrEmpty(content)) yield return null;

                    var document = new XmlDocument();
                    document.LoadXml(content);

                    var root = document.DocumentElement;
                    if (root == null) yield return null;

                    var table = root.SelectNodes("Table");
                    if (table == null) yield return null;

                    foreach (XmlElement node in table)
                    {
                        if (!node.HasAttributes)
                        {
                            continue;
                        }
                        T classInstance = Activator.CreateInstance<T>();
                        var fieldInfos = typeof(T).GetFields();
                        foreach (var fieldInfo in fieldInfos)
                        {
                            if (node.HasAttribute(fieldInfo.Name))
                            {
                                fieldInfo.SetValue(classInstance, node.GetAttribute(fieldInfo.Name), fieldInfo.FieldType);
                            }
                        }
                        yield return classInstance;
                    }
                }
            }

            void ImpHelper.Export(string savePath, Dictionary<int, List<Cell>> dic, string tableName)
            {
                FileInfo fileInfo = new FileInfo(savePath);
                string info = fileInfo.Name.Replace(fileInfo.Extension, "").Replace("Table", "Info");

                var array = dic.Select(item =>
                {
                    return new XElement(info,
                           item.Value.Select(p =>
                           {
                               return new XElement(p.name, p.value);
                           }).ToArray());
                }).ToArray();

                var doc = new XDocument(
                    new XDeclaration("1.0", "UTF-8", ""),
                    new XElement("ArrayOf" + info, array)
                );
                doc.Save(savePath);
                return;

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
            }
        }
    }
}
