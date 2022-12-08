using Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public partial class Sample : MonoBehaviour
{
	//设定配表方式
	public static ImpHelper helper => Activator.CreateInstance(Helper.Setting) as ImpHelper;

	private Dictionary<Type, IDataCollection> dictionary;

    public IDataCollection this[Type key] => dictionary.TryGetValue(key, out IDataCollection data) ? data : null;

    public IEnumerator Start()
	{
		dictionary = typeof(IDataCollection).Assembly.GetTypes()
			.Where(p => typeof(IDataCollection).IsAssignableFrom(p))
			.Where(p => typeof(IData).IsAssignableFrom(p))
			.Where(p => !p.IsAbstract)
			.ToDictionary(p => p, p => (IDataCollection)Activator.CreateInstance(p));

		using (var loader = new LoadBin())
		{
			if (FileHelper.GZIP.Deserialize(loader.path, out Dictionary<string, byte[]> dict))
			{
				dict = dict.ToDictionary(p => Path.GetFileNameWithoutExtension(p.Key), p => p.Value);

				helper.count = 0;
				foreach (var pair in dictionary)
				{
					if (dict.TryGetValue(pair.Key.Name, out byte[] bytes))
					{
						Debug.Log("this table is now ! " + pair.Key.Name);
						yield return pair.Value.Load(helper, bytes);

						//var info = GetType().GetProperty(pair.Key.Name, BindingFlags.Public | BindingFlags.Instance);
						//if (info == null) continue;
						//info.SetValue(this, pair.Value);
					}
					else
					{
						Debug.LogError("this table is not exist! " + pair.Key.Name);
					}
				}
			}
		}
	}
}

public partial class LoadBin : IDisposable
{
	public string path { get; set; }

	public LoadBin()
	{
		path = Path.Combine(Application.persistentDataPath, "data.bin");
	}
	void IDisposable.Dispose()
	{
		if (File.Exists(path))
			File.Delete(path);
	}
}