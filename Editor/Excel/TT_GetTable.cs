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

		//此方法默认从第一行输出
		//第一行为描述
		//遍历行的集合，取得每一行的DataRow对象
		//默认表中第一行为表头，输出
		//人为规定第二行为变量名称
		//人为规定第三行为变量类型
		//共有多少列由第二行第三行列数来决定
		public override IEnumerable<DataTable> GetDataTable()
		{
			if (dataSet == null)
			{
				Debug.Log(null);
				yield break;
			}

			for (int i = 0; i < dataSet.Tables.Count; i++)
			{
				yield return dataSet.Tables[i];
			}
		}

		void IDisposable.Dispose()
		{
			
		}
	}
}