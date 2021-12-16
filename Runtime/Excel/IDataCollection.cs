using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace Excel
{

    public interface IDataInfo<T>
    {
        T GetPrimaryKey();
    }

    public abstract class DataInfo<T> : IDataInfo<T>
    {
        public virtual T GetPrimaryKey()
        {
            throw new NotImplementedException();
        }
    }

	/// <summary>
	/// 接口
	/// </summary>
	public partial interface IDataCollection
	{
		IEnumerator Load(ImpHelper helper, object obj);
	}

	public interface IReviseFunc
	{
		bool OnReviseFunc();
	}

	public abstract partial class DataTable<TK, T>
	{
		public Func<bool> ReviseFunc { get { return this is IReviseFunc temp ? temp.OnReviseFunc : (Func<bool>)null; } }
	}

	public abstract partial class DataTable<TK, T> : KeyedCollection<TK, T>, IDataCollection where T : DataInfo<TK>
    {
		public static Dictionary<string, string> CacheDesc { get; private set; }

		static DataTable()
		{
			CacheDesc = typeof(T).GetFields().ToDictionary(p => p.Name, q =>
			{
				return q.GetCustomAttributes(false).OfType<DescriptionAttribute>().FirstOrDefault()?.Description;
			});
		}

		public virtual string GetFieldDesc(string fieldName)
		{
			CacheDesc.TryGetValue(fieldName, out string value);
			return value;
		}

		protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
        }

		IEnumerator IDataCollection.Load(ImpHelper helper, object obj)
		{
			foreach (var item in helper.ProcessData<T>(obj) as List<T>)
			{
				Add(item);
			}
			ReviseFunc?.Invoke();

			yield break;
		}

        protected new T this[TK id]
        {
            get { return GetItem(id); }
        }

        public virtual T GetItem(TK id)
        {
            if (Contains(id))
                return Dictionary[id];
            return default(T);
        }

        protected override TK GetKeyForItem(T item)
        {
            return item.GetPrimaryKey();
        }
	}
}
