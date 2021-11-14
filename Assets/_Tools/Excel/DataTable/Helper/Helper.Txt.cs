using Library.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Excel
{
    public abstract partial class Helper
    {
        public partial class Txt : ImpHelper
        {
            public string Extensions { get { return ".txt"; } }
            object ImpHelper.ProcessData<T>(object obj)
            {
                if (!(obj is byte[] bytes)) return null;
                var content = System.Text.Encoding.UTF8.GetString(bytes);
                return Import<T>(content);
            }
            List<T> Import<T>(string content)
            {
                List<T> list = new List<T>();
                if (string.IsNullOrEmpty(content)) return list;
                string[] results = content.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                T classInstance = default(T);
                foreach (var line in results)
                {
                    if (line.StartsWith("#BEGIN"))
                    {
                        classInstance = Activator.CreateInstance<T>();
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
                    var fileInfo = typeof(T).GetField(result[0]);
                    if (fileInfo == null)
                        continue;
                    fileInfo.SetValue(classInstance, result[1], fileInfo.FieldType);
                }
                return list;
            }

            void ImpHelper.Export(string savePath, Dictionary<int, List<Cell>> dic, string tableName)
            {
                using (StreamWriter sw = new StreamWriter(savePath))
                {
                    foreach (var pair in dic)
                    {
                        sw.WriteLine("#BEGIN");
                        foreach (var cell in pair.Value)
                        {
                            sw.WriteLine(cell.name + ":" + cell.value);
                        }
                        sw.WriteLine("#END");
                    }
                }
            }
        }
    }
}
