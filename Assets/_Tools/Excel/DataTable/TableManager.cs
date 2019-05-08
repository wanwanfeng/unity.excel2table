using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Excel;
using Library.Extensions;
using UnityEngine;

public partial class TableManager
{
    #region 管理

    private Dictionary<Type, IDataCollection> dictionary;

    public List<string> Init(int index)
    {
        dictionary = new Dictionary<Type, IDataCollection>();
        var fields = GetType().GetFields().Where(p => p.FieldType.GetInterfaces().Contains(typeof (IDataCollection)));
        foreach (var info in fields)
        {
            var att = info.GetFirstCustomAttribute<CategoryAttribute>() ?? new CategoryAttribute("2");
            if (index.ToString() != att.Category) continue;
            var collection = info.FieldType.IsSubclassOf(typeof (ScriptableObject))
                ? (IDataCollection) ScriptableObject.CreateInstance(info.FieldType)
                : (IDataCollection) Activator.CreateInstance(info.FieldType);
            info.SetValue(this, collection);
            dictionary[info.FieldType] = collection;
        }
        return dictionary.Values.SelectMany(p => p.DataPaths).ToList();
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

    public IDataCollection this[Type key]
    {
        get
        {
            IDataCollection iDataCollection;
            if (!dictionary.TryGetValue(key, out iDataCollection))
				return null;
            return iDataCollection;
        }
    }

    #endregion
}
