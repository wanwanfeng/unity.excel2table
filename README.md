# unity.excel2table
应用于Unity的Excel导出文件插件，目前了正常导出xml，xmlattribute，json，csv，bytes，txt。加载模块已经预留接口。



## 使用步骤
1. 进入Sample设定配表使用方式。
如：

```
public partial class Sample : MonoBehaviour
{
	//设定配表方式
	public static Excel.ImpHelper helper => new Excel.Helper.Json();
	//public static Excel.ImpHelper helper => new Excel.Helper.Xml();
	//public static Excel.ImpHelper helper => new Excel.Helper.XmlAtt();
	//public static Excel.ImpHelper helper => new Excel.Helper.Csv();
	//public static Excel.ImpHelper helper => new Excel.Helper.Txt();
	//public static Excel.ImpHelper helper => new Excel.Helper.Bytes();
	......
}
```

2. 进入项目根目录Excel文件夹，按照模板文件新加配表或者修改已有配表数据。

3. 进入unity项目，执行菜单“Tools/ExcelTools/Build”。

4. 进入项目根目录Temp/Excel文件夹，查看生成的数据文件以及对应的类文件data.cs，若已知配表字段类型进行了修改或新增，则把data.cs更新至项目内并重新编译代码。Temp/Excel/data为生成的数据中间文件，这些文件会压缩并加密到“Assets/Resources/data.bin.bytes”文件内，此文件为实际使用文件。
5. 文件使用方法见Sample.cs：
```
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
```


