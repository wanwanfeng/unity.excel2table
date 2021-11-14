using Library.Helper;
using PLitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Excel
{
    public abstract partial class Helper
    {
        public partial class Json : ImpHelper
        {
            public string Extensions { get { return ".json"; } }
            object ImpHelper.ProcessData<T>(object obj)
            {
                if (!(obj is byte[] bytes)) return null;
                var content = System.Text.Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
                //Debug.Log(content);
                return Library.Helper.JsonHelper.ToObject<List<T>>(content.Trim('\r', '\n'));
            }

            void ImpHelper.Export(string savePath, Dictionary<int, List<Cell>> dic, string tableName)
            {
                using (StringWriter sw = new StringWriter())
                {
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

                    File.WriteAllText(savePath, writer.TextWriter.ToString(), Encoding.UTF8);
                }
            }
        }
    }
}