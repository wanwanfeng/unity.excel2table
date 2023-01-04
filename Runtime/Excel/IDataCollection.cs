using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
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
		uint priority { get; }

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

		uint IDataCollection.priority { get { return GetPriority(); } }

		protected virtual uint GetPriority()
		{
			return 999;
		}

		IEnumerator IDataCollection.Load(ImpHelper helper, object obj)
		{
			foreach (var item in helper.ProcessData<T>(obj).OfType<T>())
			{
				Add(item);
				helper.count++;
				if (helper.count % 5000 == 0) Thread.Sleep(0);
				if (helper.count % 3000 == 0) yield return null;
			}

			ReviseFunc?.Invoke();
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
