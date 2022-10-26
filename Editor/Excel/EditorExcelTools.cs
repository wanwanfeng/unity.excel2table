using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Excel
{
    public partial class EditorExcelTools
	{
		//设定配表方式
		static Excel.ImpHelper helper => Sample.helper;

		public static string ExcelPath = "Excel/"; //Excel根目录
		public static string TablePath = "Temp/Excel/"; //Table根目录

		[Serializable]
		public class ExcelInfo
		{
			public string excelName { get; set; }
			public string excelFullPath { get; set; }
			public string classTableName { get; set; }
			public string classInfoName { get; set; }
			public int sheet { get; set; } = 0;

			public ExcelInfo(string excelFullPath)
			{
				this.excelFullPath = excelFullPath;
				this.excelName = Path.GetFileName(excelFullPath);
				this.classTableName = Path.GetFileNameWithoutExtension(excelFullPath);
				this.classInfoName = classTableName.Replace("Table", "Info");
			}
		}

		[MenuItem("Tools/ExcelTools/Build")]
		private static void ExportExcel()
		{
			Directory.CreateDirectory(ExcelPath);
			Directory.CreateDirectory(TablePath);

			var files = Directory.GetFiles(ExcelPath, "*.*", SearchOption.TopDirectoryOnly)
				.Where(p => p.EndsWith(".xlsx") || p.EndsWith(".xls"))
				.Select(p => new ExcelInfo(p))
				.Where(p => !p.classTableName.StartsWith("."))
				.ToArray();

			Debug.Log(LitJson.JsonMapper.ToJson(files));

			CreateClass(files);
			ExportTable(files, helper);
			AssetDatabase.Refresh();
		}

		/// <summary>
		/// 创建类文件
		/// </summary>
		/// <param name="excelFullPath"></param>
		public static void CreateClass(ExcelInfo[] content)
		{
			StringBuilder sb = new StringBuilder();
			StringBuilder sb2 = new StringBuilder();

			sb.AppendLine("using System;");
			sb.AppendLine("using System.Collections.Generic;");
			sb.AppendLine("using System.ComponentModel;");
			sb.AppendLine("using Excel;");
			sb.AppendLine("using UnityEngine;");
			sb.AppendLine("");

			sb2.AppendLine("public interface IData {}");

			foreach (ExcelInfo excelInfo in content.OrderBy(p => p.excelName))
			{
				var result = YeildReturnLine(excelInfo.excelFullPath, excelInfo.sheet).OfType<List<Cell>>().ToList();
				if (result.Count == 0) continue;

				string keyType = result[0].FirstOrDefault(p => p.name == "id").type;

				//创建DataInfo
				sb.AppendLine("");
				sb.AppendLine("[Serializable]");
				sb.AppendLine($"public partial class {excelInfo.classInfoName} : DataInfo<{keyType}>");
				sb.AppendLine("{");
				{
					foreach (var cell in result[0])
					{
						sb.Append("\t"); sb.AppendLine($"[Description(\"{cell.remark.Replace("\n", "")}\")]");
						sb.Append("\t"); sb.AppendLine($"public {cell.type} {cell.name};");
					}

					//创建DataInfo.GetPrimaryKey
					sb.Append("\t"); sb.AppendLine($"public override {keyType} GetPrimaryKey()");
					sb.Append("\t"); sb.AppendLine("{");
					sb.Append("\t"); sb.Append("\t"); sb.AppendLine("return id;");
					sb.Append("\t"); sb.AppendLine("}");

					sb.Append("\t"); sb.AppendLine("partial void OnConstruction();");

					//创建DataInfo.Copy
					sb.Append("\t"); sb.AppendLine($"public {excelInfo.classInfoName} Copy({excelInfo.classInfoName} other)");
					sb.Append("\t"); sb.AppendLine("{");
					foreach (var cell in result[0])
					{
						sb.Append("\t"); sb.Append("\t"); sb.AppendLine($"this.{cell.name} = other.{cell.name};");
					}
					sb.Append("\t"); sb.Append("\t"); sb.AppendLine("OnConstruction();");
					sb.Append("\t"); sb.Append("\t"); sb.AppendLine("return other;");
					sb.Append("\t"); sb.AppendLine("}");


					//创建DataInfo.Dict
					sb.Append("\t"); sb.AppendLine($"public static explicit operator {excelInfo.classInfoName}(Dictionary<string, object> other)");
					sb.Append("\t"); sb.AppendLine("{");
					sb.Append("\t"); sb.Append("\t"); sb.AppendLine($"var val_ = new {excelInfo.classInfoName}();");
					foreach (var cell in result[0])
					{
						sb.Append("\t"); sb.Append("\t"); sb.AppendLine($"val_.{cell.name} = other.ContainsKey(\"{cell.name}\") ? ({cell.type})other[\"{cell.name}\"] : default;");
					}
					sb.Append("\t"); sb.Append("\t"); sb.AppendLine("val_.OnConstruction();");
					sb.Append("\t"); sb.Append("\t"); sb.AppendLine("return val_;");
					sb.Append("\t"); sb.AppendLine("}");
				}
				sb.AppendLine("}");

				//创建DataTable
				sb2.AppendLine("[Serializable]");
				sb2.AppendLine($"public partial class {excelInfo.classTableName} : DataTable<{keyType}, {excelInfo.classInfoName}>, IData");
				sb2.AppendLine("{");
				sb2.AppendLine("}");
			}

			sb.AppendLine(sb2.ToString());

			//创建TableManager
			sb.AppendLine($"public partial class TableManager");
			sb.AppendLine("{");
			foreach (ExcelInfo excelInfo in content.OrderBy(p => p.excelName))
			{
				sb.Append("\t"); sb.AppendLine($"public  {excelInfo.classTableName} {excelInfo.classTableName} {{ get; private set; }}");
			}
			sb.AppendLine("}");
			File.WriteAllText(Path.Combine(TablePath, "data.cs"), sb.ToString());
		}

		public static void ExportTable(ExcelInfo[] content, ImpHelper helper)
		{
			var root = Path.Combine(TablePath, "data");
			Directory.CreateDirectory(root);

			foreach (var excelInfo in content)
			{
				string path = excelInfo.excelFullPath;
				string savePath = Path.ChangeExtension(Path.Combine(root, excelInfo.classTableName), helper.Extensions);
				Debug.Log(path + "\n" + savePath);
				helper.Export(savePath, YeildReturnLine(path, excelInfo.sheet), Path.GetFileNameWithoutExtension(savePath));
			}

			var tables = Directory.GetFiles(root, "*.*").ToArray();
			FileHelper.GZIP.Serialize(tables, Path.Combine(Application.persistentDataPath, "data.bin"));
			EditorUtility.RevealInFinder(Application.persistentDataPath);
		}

        /// <summary>
        /// 数据组合成字典
        /// </summary>
        /// <param name="path"></param>
        /// <param name="rowCollection"></param>
        /// <returns></returns>
        private static Dictionary<int, List<Cell>> GetDictionary(string path, int sheet)
        {
            //using (var dd = new MS_GetConnection(path))
            using (var dd = new TT_GetTable(path))
            {
                DataRowCollection rowCollection = dd.GetDataRowCollection(sheet);
                int curHang = 0;

                //描述行信息
                var remarks = rowCollection[curHang++].ItemArray.ToList();
                //名称行信息
                var names = rowCollection[curHang++].ItemArray.ToList();
                for (var i = 0; i < names.Count; i++)
                {
                    if (names[i] != null && !string.IsNullOrEmpty(names[i].ToString())) continue;
                    Debug.LogErrorFormat("{3}:变量名称有空余项！\n错误位置在坐标(行：{0}、列：{1})处！共应该有{2}个变量名称！", 0 + 2, i + 1, names.Count, Path.GetFileName(path));
                    return null;
                }

                //类型行信息
                var types = rowCollection[curHang++].ItemArray.ToList();
                for (var i = 0; i < types.Count; i++)
                {
                    if (types[i] != null && !string.IsNullOrEmpty(types[i].ToString())) continue;
                    Debug.LogErrorFormat("{3}:变量类型有空余项！\n错误位置在坐标(行：{0}、列：{1})处！共应该有{2}个变量名称！", 1 + 2, i + 1, types.Count, Path.GetFileName(path));
                    return null;
                }

                var dic = new Dictionary<int, List<Cell>>();
                //值行信息
                for (var j = curHang++; j < rowCollection.Count; j++)
                {
                    var rows = rowCollection[j].ItemArray.ToList();
                    if (string.IsNullOrEmpty(rows[0].ToString()))
                    {
                        continue; //首格为空时此行跳过
                    }
                    var rowresult = new List<Cell>();
                    for (var i = 0; i < rows.Count; i++)
                    {
                        if (rows[i] == null) continue;
                        if (types[i].ToString().StartsWith("#") || names[i].ToString().StartsWith("#"))
                        {
                            //备注列（不属于有效数据）
                            continue;
                        }
                        rowresult.Add(new Cell
                        {
                            remark = remarks[i].ToString(),
                            name = names[i].ToString(),
                            type = types[i].ToString(),
                            value = rows[i].ToString()
                        });
                    }
                    dic[j - curHang] = rowresult;
                }
                return dic;
            }
        }

		private static IEnumerable YeildReturnLine(string path, int sheet)
		{
			//using (var dd = new MS_GetConnection(path))
			using (var dd = new TT_GetTable(path))
			{
				DataRowCollection rowCollection = dd.GetDataRowCollection(sheet);
				IEnumerator<DataRow> enumerator = rowCollection.GetEnumerator() as IEnumerator<DataRow>;

				while (enumerator.MoveNext())
				{
					var remarks = enumerator.Current.ItemArray;//描述行信息

					enumerator.MoveNext();
					var names = enumerator.Current.ItemArray;//名称行信息
					for (var i = 0; i < names.Length; i++)
					{
						if (names[i] != null && !string.IsNullOrEmpty(names[i].ToString())) continue;
						Debug.LogErrorFormat("{3}:变量名称有空余项！\n错误位置在坐标(行：{0}、列：{1})处！共应该有{2}个变量名称！", 0 + 2, i + 1, names.Length, Path.GetFileName(path));
						yield break;
					}

					enumerator.MoveNext();
					var types = enumerator.Current.ItemArray;//类型行信息
					for (var i = 0; i < types.Length; i++)
					{
						if (types[i] != null && !string.IsNullOrEmpty(types[i].ToString())) continue;
						Debug.LogErrorFormat("{3}:变量类型有空余项！\n错误位置在坐标(行：{0}、列：{1})处！共应该有{2}个变量名称！", 1 + 2, i + 1, types.Length, Path.GetFileName(path));
						yield break;
					}

					while (enumerator.MoveNext())
					{
						var cells = enumerator.Current.ItemArray;//值行信息
						if (string.IsNullOrEmpty(cells[0].ToString()))
						{
							continue; //首格为空时此行跳过
						}
						var rowresult = new List<Cell>();
						for (var i = 0; i < cells.Length; i++)
						{
							if (cells[i] == null) continue;
							if (types[i].ToString().StartsWith("#") || names[i].ToString().StartsWith("#"))
							{
								//备注列（不属于有效数据）
								continue;
							}
							rowresult.Add(new Cell
							{
								remark = remarks[i].ToString(),
								name = names[i].ToString(),
								type = types[i].ToString(),
								value = cells[i].ToString()
							});
						}
						yield return rowresult;

						yield return remarks;
						yield return names;
						yield return types;
						yield return cells;
					}
				}
			}
		}
	}

	public class GetTable
	{
		public virtual DataTable GetDataTable(int sheet)
		{
			return null;
		}

		public virtual DataRowCollection GetDataRowCollection(int sheet)
		{
			DataTable table = GetDataTable(sheet); //返回第一张表

			DataRowCollection rowCollection = table.Rows; //返回一个行的集合

			Debug.Log(string.Format("行:{0}\n列:{1}", table.Rows.Count, table.Columns.Count));

			return rowCollection;
		}
	}
}