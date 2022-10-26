using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Excel
{
    public abstract partial class Helper
    {
        public partial class Txt : ImpHelper
        {
            public int count { get; set; }
            public string Extensions { get { return ".txt"; } }
            IEnumerable ImpHelper.ProcessData<T>(object obj)
            {
                if (obj is byte[] bytes)
                {
                    var content = System.Text.Encoding.UTF8.GetString(bytes);
                    if (string.IsNullOrEmpty(content)) yield return null;

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
                            yield return classInstance;
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
                }
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

            void ImpHelper.Export(string savePath, IEnumerable enumerable, string tableName)
            {
                using (StreamWriter sw = new StreamWriter(savePath))
                {
                    foreach (var list in enumerable.OfType<List<Cell>>())
                    {
                        sw.WriteLine("#BEGIN");
                        foreach (var cell in list)
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
