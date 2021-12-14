using Library.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Excel
{
    public abstract partial class Helper
    {
        public partial class Bytes : ImpHelper
        {
            public string Extensions { get { return ".bytes"; } }

            object ImpHelper.ProcessData<T>(object obj)
            {
                if (!(obj is byte[] bytes)) return null;
                return Import<T>(bytes);
            }

            List<T> Import<T>(byte[] content)
            {
                List<T> list = new List<T>();
                if (content.Length == 0) return list;

                using (var ms = new MemoryStream(content))
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
                            list.Add(classInstance);
                        }
                    }
                }
                return list;
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
        }
    }
}
