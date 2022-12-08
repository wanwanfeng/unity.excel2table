using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

namespace Excel
{
	/// <summary>
	/// 跨平台插件
	/// https://archive.codeplex.com/?p=exceldatareader
	/// https://github.com/ExcelDataReader/ExcelDataReader
	/// </summary>
	public class TT_GetTable : GetTable, IDisposable
	{
		DataSet dataSet;
		public TT_GetTable(string path)
		{
			try
			{
				IExcelDataReader excelReader = null;
				using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read))
				{
					switch (Path.GetExtension(path))
					{
						case ".xls":
							//Excel2003的连接字符串
							excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
							break;
						case ".xlsx":
							//Excel2007的连接字符串  
							excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
							break;
						default:
							Debug.Log("文件类型错误！！！");
							return;
					}
					dataSet = excelReader.AsDataSet();
				}
			}
			catch (Exception err)
			{
				Debug.LogError(Path.GetFileName(path) + ":数据绑定Excel失败!失败原因：" + err.Message);
			}
		}

		public override IEnumerable<DataRowCollection> GetDataRowCollection()
		{
			if (dataSet == null)
			{
				Debug.Log(null);
				yield break;
			}

			for (int i = 0; i < dataSet.Tables.Count; i++)
			{
				var table = dataSet.Tables[i];

				DataRowCollection rowCollection = table.Rows;

				Debug.Log(string.Format("行:{0}\n列:{1}", table.Rows.Count, table.Columns.Count));

				yield return rowCollection;
			}
		}

        void IDisposable.Dispose()
		{
			
		}
	}
}