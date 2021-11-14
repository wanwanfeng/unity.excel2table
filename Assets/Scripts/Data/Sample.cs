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
	public static Excel.ImpHelper helper => new Excel.Helper.Json();

	private Dictionary<Type, IDataCollection> dictionary;

    public IDataCollection this[Type key] => dictionary.TryGetValue(key, out IDataCollection data) ? data : null;

    public IEnumerator Start()
	{
		dictionary = typeof(IDataCollection).Assembly.GetTypes()
			.Where(p => typeof(IDataCollection).IsAssignableFrom(p))
			.Where(p => typeof(IData).IsAssignableFrom(p))
			.Where(p => !p.IsAbstract)
			.ToDictionary(p => p, p => (IDataCollection)Activator.CreateInstance(p));

		var path = Path.Combine(Application.persistentDataPath, "data.bin");
		var textAsset = Resources.Load<TextAsset>("data.bin");
		File.WriteAllBytes(path, textAsset.bytes);
		Resources.UnloadAsset(textAsset);

		if (FileHelper.GZIP.Deserialize(path, out Dictionary<string, byte[]> dict))
		{
			dict = dict.ToDictionary(p => Path.GetFileNameWithoutExtension(p.Key), p => p.Value);

			foreach (var pair in dictionary)
			{
				if (dict.TryGetValue(pair.Key.Name, out byte[] bytes))
				{
					Debug.Log("this table is now ! " + pair.Key.Name);
					yield return pair.Value.Load(helper, bytes);

					var info = GetType().GetProperty(pair.Key.Name, BindingFlags.Public | BindingFlags.Instance);
					if (info == null) continue;
					info.SetValue(this, pair.Value);
                }
                else
                {
					Debug.LogError("this table is not exist! " + pair.Key.Name);
                }
			}
		}

		File.Delete(path);
	}
}