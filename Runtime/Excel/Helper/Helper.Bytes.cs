using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Excel
{
    public abstract partial class Helper
    {
        public partial class Bytes : ImpHelper
        {
            public int count { get; set; }
            public string Extensions { get { return ".bytes"; } }
            IEnumerable ImpHelper.ProcessData<T>(object obj)
            {
                if (obj is byte[] bytes)
                {
                    if (bytes.Length == 0) yield return null;

                    using (var ms = new MemoryStream(bytes))
                    {
                        using (var br = new BinaryReader(ms))
                        {
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
                                T classInstance = Activator.CreateInstance<T>();
                                for (var j = 0; j < length; j++)
                                {
                                    var field = fieldList[j];
                                    var type = typeList[j];

                                    var fileInfo = typeof(T).GetField(field);
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
                                yield return classInstance;
                            }
                        }
                    }
                }
            }

            void ImpHelper.Export(string savePath, Dictionary<int, List<Cell>> dic, string tableName)
            {
                using (FileStream sw = new FileStream(savePath, FileMode.Create))
                using (BinaryWriter bw = new BinaryWriter(sw))
                {
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
                }
            }

            void ImpHelper.Export(string savePath, IEnumerable enumerable, string tableName)
            {
                using (FileStream sw = new FileStream(savePath, FileMode.Create))
                using (BinaryWriter bw = new BinaryWriter(sw))
                {
                    var result = enumerable.OfType<List<Cell>>().ToList();
                    bw.Write(result.Count);
                    bool init = false;
                    foreach (var list in result)
                    {
                        if (!init)
                        {
                            init = true;
                            bw.Write(list.Count);
                            foreach (var cell in list)
                            {
                                bw.Write(cell.name);
                                bw.Write(cell.type); //写入是为了读取时发现不存在的字段时跳过以保持偏移
                            }
                        }
                        foreach (var cell in list)
                        {
                            bw.Write(cell.value, cell.type);
                        }
                    }
                }
            }
        }
    }
}
