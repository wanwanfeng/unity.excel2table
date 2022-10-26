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

        public partial class Csv : ImpHelper
        {
            public int count { get; set; }
            public string Extensions { get { return ".csv"; } }
            IEnumerable ImpHelper.ProcessData<T>(object obj)
            {
                if (obj is byte[] bytes)
                {
                    var content = System.Text.Encoding.UTF8.GetString(bytes);
                    if (string.IsNullOrEmpty(content)) yield return null;

                    string[] results = content.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    var fieldList = results[0].Split();
                    var typeList = results[1].Split();
                    for (var i = 2; i < results.Length; i++)
                    {
                        string line = results[i];
                        string[] result = line.Split();
                        if (result.Length == 0) continue;
                        T classInstance = Activator.CreateInstance<T>();
                        for (var j = 0; j < result.Length; j++)
                        {
                            var fileInfo = typeof(T).GetField(fieldList[j]);
                            if (fileInfo == null || string.IsNullOrEmpty(result[j]))
                                continue;
                            fileInfo.SetValue(classInstance, result[j], fileInfo.FieldType);
                        }
                        yield return classInstance;
                    }
                }
            }

            void ImpHelper.Export(string savePath, Dictionary<int, List<Cell>> dic, string tableName)
            {
                using (StreamWriter sw = new StreamWriter(savePath))
                {
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
                }
            }

            void ImpHelper.Export(string savePath, IEnumerable enumerable, string tableName)
            {
                using (StreamWriter sw = new StreamWriter(savePath))
                {
                    bool init = false;
                    foreach (var list in enumerable.OfType<List<Cell>>())
                    {
                        if (!init)
                        {
                            init = true;
                            sw.WriteLine(string.Join(",", list.Select(p => p.name).ToArray()));
                            sw.WriteLine(string.Join(",", list.Select(p => p.type).ToArray()));
                        }
                        sw.WriteLine(string.Join(",", list.Select(p => p.value).ToArray()));
                    }
                }
            }
        }
    }
}