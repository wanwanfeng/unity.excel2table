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
        public partial class Asset : ImpHelper
        {
            public int count { get; set; }
            public string Extensions { get { return ".asset"; } }

            IEnumerable ImpHelper.ProcessData<T>(object obj)
            {
                //var table = obj as DataTableAsset<TK, T>;
                //return table == null ? null : table.List;
                return null;
            }

            void ImpHelper.Export(string savePath, Dictionary<int, List<Cell>> dic, string tableName)
            {
                string className = tableName ?? new FileInfo(savePath).Name.Replace(".asset", "");

                List<object> db = new List<object>();
                var type = typeof(IDataCollection).Assembly.GetTypes().FirstOrDefault(p => p.Name == tableName.Replace("Table", "Info"));

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

#if UNITY_EDITOR

                //todo: ´ýÑéÖ¤
                var table = UnityEditor.AssetDatabase.LoadAssetAtPath(savePath, type) as UnityEngine.ScriptableObject;
                if (table == null)
                {
                    table = UnityEngine.ScriptableObject.CreateInstance(type);
                    UnityEditor.AssetDatabase.CreateAsset(table, savePath);
                }
                table.GetType().GetField("List").SetValue(table, db);
                UnityEditor.EditorUtility.SetDirty(table);
                UnityEditor.AssetDatabase.SaveAssets();
#endif
            }
        }
    }
}
