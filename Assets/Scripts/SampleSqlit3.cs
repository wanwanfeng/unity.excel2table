using Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public partial class SampleSqlit3 : Sample
{
	private Dictionary<Type, IDataCollection> dictionary;

    public IDataCollection this[Type key] => dictionary.TryGetValue(key, out IDataCollection data) ? data : null;

	public IEnumerator Start()
	{
		dictionary = typeof(IDataCollection).Assembly.GetTypes()
			.Where(p => typeof(IDataCollection).IsAssignableFrom(p))
			.Where(p => typeof(IData).IsAssignableFrom(p))
			.Where(p => !p.IsAbstract)
			.ToDictionary(p => p, p => (IDataCollection)Activator.CreateInstance(p));

		helper.count = 0;
		foreach (var pair in dictionary)
		{
			yield return pair.Value.Load(helper, pair.Key.Name);

			//var info = GetType().GetProperty(pair.Key.Name, BindingFlags.Public | BindingFlags.Instance);
			//if (info == null) continue;
			//info.SetValue(this, pair.Value);
		}
	}
}