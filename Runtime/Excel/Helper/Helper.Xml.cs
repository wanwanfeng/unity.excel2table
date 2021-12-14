using Library.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Excel
{
    public abstract partial class Helper
    {
        public partial class Xml : ImpHelper
        {
            public string Extensions { get { return ".xml"; } }
            object ImpHelper.ProcessData<T>(object obj)
            {
                if (!(obj is byte[] bytes)) return null;
                var content = System.Text.Encoding.UTF8.GetString(bytes);
                return XmlHelper.ToObject<List<T>>(content);
            }
            void ImpHelper.Export(string savePath, Dictionary<int, List<Cell>> dic, string tableName)
            {
                var array = dic.Select(item =>
                {
                    return new XElement("Table",
                           item.Value.Select(p =>
                           {
                               return new XAttribute(p.name, p.value);
                           }).ToArray());
                }).ToArray();

                var doc = new XDocument(
                    new XDeclaration("1.0", "UTF-8", ""),
                    new XElement("Root", array)
                );
                doc.Save(savePath);
                return;
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
            }

        }
    }
}
