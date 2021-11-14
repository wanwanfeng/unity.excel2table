using System;
using System.Collections.Generic;
using System.ComponentModel;
using Excel;
using UnityEngine;


[Serializable]
public partial class AudioInfo : DataInfo<int>
{
	[Description("序号")]
	public int id;
	[Description("路径")]
	public string path;
	public override int GetPrimaryKey()
	{
		return id;
	}
	partial void OnConstruction();
	public AudioInfo Copy(AudioInfo other)
	{
		this.id = other.id;
		this.path = other.path;
		OnConstruction();
		return other;
	}
	public static explicit operator AudioInfo(Dictionary<string, object> other)
	{
		var val_ = new AudioInfo();
		val_.id = other.ContainsKey("id") ? (int)other["id"] : default;
		val_.path = other.ContainsKey("path") ? (string)other["path"] : default;
		val_.OnConstruction();
		return val_;
	}
}
public interface IData {}
[Serializable]
public partial class AudioTable : DataTable<int, AudioInfo>, IData
{
}

public partial class TableManager
{
	public  AudioTable AudioTable { get; private set; }
}
