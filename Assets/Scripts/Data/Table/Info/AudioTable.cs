using System;
using System.ComponentModel;
using Excel;

[Serializable]
 public partial class AudioInfo
{
	[Description("序号")]
	public int id;
	[Description("路径")]
	public string path;
}

[Serializable]
public partial class AudioTable : DataTable<int,AudioInfo>
{
}