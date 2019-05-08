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
using Library.Extensions;
using UnityEngine;

namespace Excel
{

    /// <summary>
    /// 接口
    /// </summary>
    public interface IDataCollection
    {
        void Add(params object[] data);
        void Clear();
        void Trim();
        void OnLoaded();
        string[] DataPaths { get; set; }
        string DataSuffix { get; set; }
        void Load(bool isRunTime = false);
    }

    /// <summary>
    /// 接口
    /// </summary>
    public interface IDataCollection<in T> : IDataCollection where T : class, new()
    {
        void Add(params T[] data);
    }

    /// <summary>
    /// list table （列表）
    /// </summary>
    /// <typeparam name="T"></typeparam>
# if IS_DATATABLEASSET
    public abstract class DataCollection<T> : ScriptableObject, IDataCollection<T>, IDataCollection
        where T : class, new()
#else
    public abstract class DataCollection<T> :  IDataCollection<T>, IDataCollection where T : class, new()
#endif
    {
        [SerializeField] public List<T> List; //{ get; private set; }
        public Dictionary<string, string> CacheDesc { get; private set; }


        protected DataCollection()
        {
            List = new List<T>();
            CacheDesc = new Dictionary<string, string>();
        }

        public virtual T this[int index]
        {
            get { return List[index]; }
        }

        public virtual void Add(params T[] data)
        {
            List.AddRange(data);
        }

        public virtual void Add(params object[] data)
        {
            List.AddRange(data.OfType<T>().ToArray());
        }

        public virtual void Clear()
        {
            if (List.Count > 0)
                List.Clear();
        }

        public virtual void Trim()
        {
            List.TrimExcess();
        }

        public virtual string GetFieldDesc(string fieldName)
        {
            string value = "";
            CacheDesc.TryGetValue(fieldName, out value);
            return value;
        }

        public virtual void OnLoaded()
        {
            CacheDesc = typeof(T).GetFields().ToDictionary(p => p.Name, q =>
            {
                var att = q.GetFirstCustomAttribute<DescriptionAttribute>();
                return att == null ? "" : att.Description;
            });
        }

        public string[] DataPaths { get; set; }
        public string DataSuffix { get; set; }

        public virtual void Init()
        {
        }

        public void Load(bool isRunTime = false)
        {
            if (isRunTime)
            {
                Clear();
                foreach (var path in DataPaths)
                {
                    UnityEngine.Object obj = Resources.Load<UnityEngine.Object>(path);
                    if (obj == null)
                    {
                        Debug.LogError("表加载出错！" + path);
                    }
                    else
                    {
                        var list = (List<T>) ProcessData(obj);
                        if (list == null)
                        {
                            Debug.LogError("表加载成功但解析出错！ " + path);
                        }
                        else
                        {
                            Add(list.ToArray());
                            Debug.Log("表加载并解析成功！ " + path);
                        }
                    }
                }

                Trim();
                OnLoaded();
                Init();
            }
            else
            {
#if UNITY_EDITOR

#endif
            }
        }

        protected virtual object ProcessData(object obj)
        {
            var table = obj as TextAsset;
            if (table == null) return null;

            var content = table.text.Trim();
            if (string.IsNullOrEmpty(content)) return null;

            return content;
        }
    }

    /// <summary>
    /// dic table (键值对)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TK"></typeparam>
    public abstract class DataCollection<TK, T> : DataCollection<T> where T : class, new()
    {
        public Dictionary<TK, T> Cache = new Dictionary<TK, T>();

        public virtual T GetItem(TK id)
        {
            {
                T t;
                if (Cache.TryGetValue(id, out t))
                    return t;
            }
            /*{
                if (typeof (T).GetField("id") != null)
                {
                    T t = List.FirstOrDefault(item => item.GetType().GetField("id").GetValue(item).Equals(id));
                    if (t == null)
                    {
                        Debug.LogError(DataPaths + " 的id：" + id + " 不存在！");
                        return default(T);
                    }
                    return t;
                }
            }*/
            //Debug.LogError("id is not exist : " + id);
            return default(T);
        }

        public override void OnLoaded()
        {
            if (typeof(T).GetField("id") == null)
            {
                Debug.LogError("id is not exist!");
            }
            try
            {
                Cache = List.ToDictionary(p => (TK) (p.GetType().GetField("id").GetValue(p)));
            }
            catch (Exception e)
            {
                Debug.LogError("转换为字典出错：" + DataPaths);
            }
            finally
            {
                base.OnLoaded();
            }
        }
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

        protected DataTable()
        {
            var descriptionAttribute = GetType().GetFirstCustomAttribute<DescriptionAttribute>();
            if (descriptionAttribute != null)
                DataSuffix = descriptionAttribute.Description;

            var defaultValueAttribute = GetType().GetFirstCustomAttribute<DefaultValueAttribute>();
            if (defaultValueAttribute != null)
                DataPaths = defaultValueAttribute.Value.ToString().Split('|', ',').Distinct()
                    .Where(p => !string.IsNullOrEmpty(p)).Select(p => "Table/" + p).ToArray();
            else
                DataPaths = new[] {"Table/" + GetType().Name};
        }
    }
}
