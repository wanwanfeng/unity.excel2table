﻿using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Excel
{
	/// </summary>
	public class EPPlus_GetTable : GetTable, IDisposable
	{
		string path = null;

		public EPPlus_GetTable(string path)
		{
			this.path = path;
		}

		//此方法默认从第一行输出
		//第一行为描述
		//遍历行的集合，取得每一行的DataRow对象
		//默认表中第一行为表头，输出
		//人为规定第二行为变量名称
		//人为规定第三行为变量类型
		//共有多少列由第二行第三行列数来决定
		public override IEnumerable<DataRowCollection> GetDataRowCollection()
		{
			if (File.Exists(path))
			{
				using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(path)))
				{
					foreach (var worksheet in excelPackage.Workbook.Worksheets)
					{
						var dataTable = new DataTable();

						dataTable.TableName = worksheet.Name;

						for (int i = 0; i < worksheet.Dimension.Columns; i++)
						{
							dataTable.Columns.Add();
						}

						for (int j = 0; j < worksheet.Dimension.Rows; j++)
						{
							dataTable.Rows.Add(worksheet.Cells.Skip(j * worksheet.Dimension.Columns).Take(worksheet.Dimension.Columns).Select(p => p.GetValue<object>()).ToArray());
						}

						DataRowCollection rowCollection = dataTable.Rows;

						Debug.Log(string.Format("行:{0}\n列:{1}", dataTable.Rows.Count, dataTable.Columns.Count));

						yield return rowCollection;
					}
				}
			}
		}

		void IDisposable.Dispose()
		{

		}
	}
}