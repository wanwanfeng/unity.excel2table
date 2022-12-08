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
		public override IEnumerable<DataRowCollection> GetDataRowCollection(string path)
		{
			if (File.Exists(path))
			{
				using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(path)))
				{
					foreach (var worksheet in excelPackage.Workbook.Worksheets)
					{
						var dataTable = new DataTable();

						dataTable.TableName = worksheet.Name;

						for (int i = worksheet.Dimension.Start.Column; i <= worksheet.Dimension.End.Column; i++)
						{
							dataTable.Columns.Add();
						}

						for (int j = 1; j <= worksheet.Dimension.End.Row; j++)
						{
							List<object> result = new List<object>();
							for (int i = 1; i <= worksheet.Dimension.End.Column; i++)
							{
								result.Add(worksheet.Cells[j, i].GetValue<object>());
							}
							dataTable.Rows.Add(result.ToArray());
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