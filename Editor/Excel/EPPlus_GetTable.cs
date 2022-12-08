using OfficeOpenXml;
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