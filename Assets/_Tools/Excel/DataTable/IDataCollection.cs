//#define IS_DATATABLEXML 
//#define IS_DATATABLEXMLATTRIBUTE
//#define IS_DATATABLEJSON
//#define IS_DATATABLETXT 
//#define IS_DATATABLECSV 
//#define IS_DATATABLEBYTES 
#define IS_DATATABLEASSET 
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace Excel
{
    /// <summary>
    /// 接口
    /// </summary>
    public interface IDataCollection
    {
        void Add(object data);
        void Clear();
        void Trim();
        void OnLoaded();
        string DataPath { get; set; }
        string DataSuffix { get; set; }
        void Load(bool isRunTime = false);
    }

    /// <summary>
    /// table
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TK"></typeparam>
# if IS_DATATABLEASSET
    public abstract class DataCollection<TK, T> : ScriptableObject, IDataCollection where T : class, new()
#else
    public abstract class DataCollection<TK,T> : IDataCollection where T : class, new()
#endif
    {
        public List<T> Elements = new List<T>();
        public Dictionary<TK, T> Cache = new Dictionary<TK, T>();
        public Dictionary<string, string> CacheDesc = new Dictionary<string, string>();

        public T this[int index]
        {
            get { return Elements[index]; }
        }

        public void Add(object data)
        {
            Elements.Add(data as T);
        }

        public void Clear()
        {
            if (Elements.Count > 0)
            {
                Elements.Clear();
            }
        }

        public void Trim()
        {
            Elements.TrimExcess();
        }

        public virtual T GetItem(TK id)
        {
            if (id.Equals(default(TK)))
            {
                return default(T);
            }
            if (typeof (T).GetField("id") != null)
            {
                T t = Elements.FirstOrDefault(item => item.GetType().GetField("id").GetValue(item).Equals(id));
                if (t == null)
                {
                    Debug.LogError(DataPath + " 的id：" + id + " 不存在！");
                    return default(T);
                }
                return t;
            }
            Debug.LogError("id is not exist : " + id);
            return default(T);
        }

        public virtual string GetItemDesc(string fieldName)
        {
            string value = "";
            CacheDesc.TryGetValue(fieldName, out value);
            return value;
        }

        public void OnLoaded()
        {
            if (typeof (T).GetField("id") == null)
            {
                Debug.LogError("id is not exist!");
            }
            try
            {
                Cache = Elements.ToDictionary(p => (TK) (p.GetType().GetField("id").GetValue(p)));
            }
            catch (Exception e)
            {
                Debug.LogError("转换为字典出错：" + DataPath);
            }
            finally
            {
                CacheDesc = typeof (T).GetFields().ToDictionary(p => p.Name, q =>
                {
                    var att = q.GetCustomAttributes(false).OfType<DescriptionAttribute>().FirstOrDefault();
                    return att == null ? "" : att.Description;
                });
            }
        }

        public List<T> Gets()
        {
            return Elements;
        }

        public virtual string DataPath { get; set; }
        public virtual string DataSuffix { get; set; }

        public virtual void Init()
        {
        }

        public void Load(bool isRunTime = false)
        {
            if (isRunTime)
            {
                UnityEngine.Object obj = Resources.Load<UnityEngine.Object>(DataPath);
                if (obj)
                {
                    ProcessData(obj);
                    Trim();
                    if (Elements.Count != 0)
                    {
                        Debug.Log("表加载并解析成功！ " + DataPath);
                        OnLoaded();
                        Init();
                    }
                    else
                    {
                        Debug.LogError("表加载成功但解析出错！ " + DataPath);
                    }
                }
                else
                {
                    Debug.LogError("表加载出错！" + DataPath);
                }
            }
            else
            {
#if UNITY_EDITOR

#endif
            }
        }

        protected abstract void ProcessData(UnityEngine.Object table);
    }

    ///  <summary>
    /// 可以修改继承类来适应不同的加载方式
    ///  </summary>
    ///  <typeparam name="T"></typeparam>
    /// <typeparam name="TK"></typeparam>
#if IS_DATATABLEASSET
    public class DataTable<TK, T> : DataTableAsset<TK, T> where T : class, new()
#elif IS_DATATABLEBYTES
    public class DataTable<TK,T> : DataTableBytes<TK,T> where T : class, new()
#elif IS_DATATABLECSV
    public class DataTable<TK,T> : DataTableCsv<TK,T> where T : class, new()
#elif IS_DATATABLEJSON
    public class DataTable<TK,T> : DataTableJson<TK,T> where T : class, new()
#elif IS_DATATABLETXT
    public class DataTable<TK,T> : DataTableTxt<TK,T> where T : class, new()
#elif IS_DATATABLEXMLATTRIBUTE
    public class DataTable<TK,T> : DataTableXmlAttribute<TK,T> where T : class, new()
#else
    public class DataTable<TK,T> : DataTableXml<TK,T> where T : class, new()
#endif
    {
        //DataTableXml 
        //DataTableXmlAttribute 
        //DataTableTxt 
        //DataTableJson
        //DataTableCsv 
        //DataTableBytes 
        //DataTableAsset 
        public override string DataPath
        {
            get { return "Table/" + GetType().Name; }
        }
    }
}
