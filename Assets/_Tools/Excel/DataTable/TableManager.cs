using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Excel;
using UnityEngine;

public partial class TableManager
{
    #region 管理

    private Dictionary<string, IDataCollection> dictionary;

    public List<string> Init(int index)
    {
        dictionary = new Dictionary<string, IDataCollection>();
        var fields = GetType().GetFields().Where(p => p.FieldType.GetInterfaces().Contains(typeof (IDataCollection)));
        foreach (var info in fields)
        {
            var atts = info.GetCustomAttributes(false).OfType<CategoryAttribute>();
            var att = atts.FirstOrDefault() ?? new CategoryAttribute("2");
            if (index.ToString() != att.Category) continue;
            var collection = info.FieldType.IsSubclassOf(typeof (ScriptableObject))
                ? (IDataCollection) ScriptableObject.CreateInstance(info.FieldType)
                : (IDataCollection) Activator.CreateInstance(info.FieldType);
            info.SetValue(this, collection);
            dictionary[info.Name] = collection;
        }
        return dictionary.Values.Select(p => p.DataPath).ToList();
    }

    /// <summary>
    /// 加载表数据
    /// </summary>
    public IEnumerator Load(bool isIEnumerator = false)
    {
        foreach (var dataCollection in dictionary.Values)
        {
            if (isIEnumerator)
            {
                yield return null;
            }
            dataCollection.Load(true);
        }
    }

    #endregion
}
