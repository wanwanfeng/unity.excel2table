using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Excel
{
	/// <summary>
	/// 微软 Excel
	/// http://www.jb51.net/article/52681.htm
	/// </summary>
	public class MS_GetTable : GetTable, IDisposable
	{
		internal System.Data.OleDb.OleDbConnection GetConnection(string path)
		{
			if (File.Exists(path))
			{
				var connectionString = "";
				switch (Path.GetExtension(path))
				{
					case ".xls":
						//Excel2003的连接字符串
						connectionString =
							string.Format(
								"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=NO;IMEX=1\"",
								path); //xls HDR=NO;不忽略第一行 HDR=YES;忽略第一行
						break;
					case ".xlsx":
						//Excel2007的连接字符串  
						connectionString =
							string.Format(
								"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR=NO;IMEX=1\"",
								path); //xlsx HDR=NO;不忽略第一行 HDR=YES;忽略第一行
						break;
					default:
						throw new Exception("文件类型错误！！！");
				}
				return new System.Data.OleDb.OleDbConnection(connectionString);
            }
            else
            {
				throw new Exception($"{path} is not found");
            }
		}

        public override IEnumerable<DataRowCollection> GetDataRowCollection(string path)
        {
			using (var connection = GetConnection(path))
			{
				connection.Open();
				var sql = "SELECT * FROM  [Sheet1$]";
				var adapter = new System.Data.OleDb.OleDbDataAdapter(sql, connection);
				DataSet dataSet = new DataSet();
				adapter.Fill(dataSet);

				for (int i = 0; i < dataSet.Tables.Count; i++)
				{
					var table = dataSet.Tables[i];

					DataRowCollection rowCollection = table.Rows;

					Debug.Log(string.Format("行:{0}\n列:{1}", table.Rows.Count, table.Columns.Count));

					yield return rowCollection;
				}
			}
		}

        void IDisposable.Dispose()
		{
		}


		#region 生成excel

		public static void MS_ExportExcel(string path, Dictionary<int, List<Cell>> content)
		{
			using (var cc = new MS_GetTable())
			{
				using (var connection = cc.GetConnection(path))
				{
					string sqlCreate = "CREATE TABLE TestSheet ([ID] INTEGER,[Username] VarChar,[UserPwd] VarChar)";
					var cmd = new System.Data.OleDb.OleDbCommand(sqlCreate, connection);
					connection.Open();
					cmd.ExecuteNonQuery();
					foreach (KeyValuePair<int, List<Cell>> pair in content)
					{
						cmd.CommandText = string.Format("INSERT INTO TestSheet VALUES({0})",
							string.Join(",", pair.Value.Select(p => p.value).ToArray()));
						cmd.ExecuteNonQuery();
					}
				}
			}
		}

		/// <summary>
		/// 键值对
		/// </summary>
		/// <param name="path"></param>
		/// <param name="content"></param>
		public static void MS_ExportExcel(string path, Dictionary<object, List<object>> content)
		{
			using (var cc = new MS_GetTable())
			{
				using (var connection = cc.GetConnection(path))
				{
					string sqlCreate = "CREATE TABLE TestSheet ([ID] INTEGER,[Username] VarChar,[UserPwd] VarChar)";
					var cmd = new System.Data.OleDb.OleDbCommand(sqlCreate, connection);
					connection.Open();
					cmd.ExecuteNonQuery();
					foreach (var pair in content)
					{
						cmd.CommandText = string.Format("INSERT INTO TestSheet VALUES({0})",
							string.Join(",", pair.Value.Select(p => p.ToString()).ToArray()));
						cmd.ExecuteNonQuery();
					}
				}
			}
		}

		/// <summary>
		/// 键值对
		/// </summary>
		/// <param name="path"></param>
		/// <param name="content"></param>
		public static void MS_ExportExcel(string path, List<object> content)
		{
			if (content == null || content.Count == 0)
				return;

			using (var cc = new MS_GetTable())
			{
				using (var connection = cc.GetConnection(path))
				{
					{
						var pair = content.First();
						if (pair.GetType().IsClass)
						{
							var pair1 = pair;
							var names = pair.GetType().GetFields().Select(p => p.Name.ToString()).ToArray();
							var fieldTypes = pair.GetType().GetFields().Select(p => p.FieldType.ToString()).ToArray();
							Debug.Log(string.Join(",", names));
							Debug.Log(string.Join(",", fieldTypes));
						}
					}

					string sqlCreate = "CREATE TABLE TestSheet ([ID] INTEGER,[Username] VarChar,[UserPwd] VarChar)";
					var cmd = new System.Data.OleDb.OleDbCommand(sqlCreate, connection);
					connection.Open();
					cmd.ExecuteNonQuery();
					foreach (var pair in content)
					{
						if (pair.GetType().IsClass)
						{
							var pair1 = pair;
							var haha = pair.GetType().GetFields().Select(p => p.GetValue(pair1).ToString()).ToArray();
							Debug.Log(string.Join(",", haha));
							cmd.CommandText = string.Format("INSERT INTO TestSheet VALUES({0})", string.Join(",", haha));
							cmd.ExecuteNonQuery();
						}
					}
				}
			}
		}

		#endregion
	}
}