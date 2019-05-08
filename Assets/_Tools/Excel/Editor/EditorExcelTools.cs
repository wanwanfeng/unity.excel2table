using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Text;

namespace Excel
{
    public enum ExportType
    {
        [Description(".xml")] ToXml,
        [Description(".xml")] ToXmlAttribute,
        [Description(".json")] ToJson,
        [Description(".txt")] ToTxt,
        [Description(".txt")] ToCsv,
        [Description(".byte")] ToBytes,
        [Description(".asset")] ToAsset,
        [Description(".lua")] ToLua,
    }

    /// <summary>
    /// 每个格子的信息
    /// </summary>
    public struct Cell
    {
        public string remark; //每格备注
        public string name; //每格名
        public string type; //每格类型名
        public string value; //每格值
    }

    public class EditorExcelTools
    {

        #region 创建类文件

        /// <summary>
        /// 创建类文件
        /// </summary>
        /// <param name="info"></param>
        /// <param name="csPath"></param>
        public static void CreateClass(ExcelInfo info, string csPath)
        {
            var dic = ExportTable(info.excelFullPath, info.sheet);
            if (dic == null || dic.Count == 0)
                return;

            string strClass =
                @"using System;
using System.ComponentModel;
using Excel;

[Serializable]
public partial class {0}
{3}{2}
{4}

[Serializable]
public partial class {1} : DataTable<{5},{0}>
{3}
{4}";
            string strField = "\n\t[Description(\"{2}\")]\n\tpublic {0} {1};";
            string fields = "";
            string keyType = "";
            foreach (var cell in dic[0])
            {
                if (cell.name == "id")
                {
                    keyType = cell.type;
                }
                fields += string.Format(strField, cell.type, cell.name, cell.remark.Replace("\n", ""));
            }
            Debug.Log(csPath + "\n" + fields);
            string content = string.Format(strClass, info.classInfoName, info.classTableName, fields,
                "{", "}", keyType);
            Debug.Log(csPath + "\n" + content);
            File.WriteAllText(csPath, content, Encoding.UTF8);
        }

        #endregion

        #region 导出

        /// <summary>
        /// 检测文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool CheckPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.Log("请选择一个Excel文件！");
                return false;
            }

            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Directory != null && !Directory.Exists(fileInfo.Directory.FullName))
            {
                Directory.CreateDirectory(fileInfo.Directory.FullName);
            }
            if (fileInfo.Extension == ".xls" || fileInfo.Extension == ".xlsx")
            {
                return true;
            }
            Debug.LogError("选择的文件类型错误！");
            return false;
        }

        public static bool ExportTo(ExportType type, string path, string[] savePaths, int sheet = 0)
        {
            if (!CheckPath(path))
                return false;
            var dic = ExportTable(path, sheet);
            if (dic == null || dic.Count == 0)
            {
                return false;
            }
            savePaths = savePaths.Select(p => p + Path.GetFileName(path)).ToArray();
            foreach (string save in savePaths)
            {
                var dir = Path.GetDirectoryName(save);
                if (dir != null && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
            string savePath = savePaths[0];
            if (EditorExcelWrite.To(type, savePath, dic))
            {
                for (int i = 1; i < savePaths.Length; i++)
                {
                    if (savePath != null)
                        File.Copy(savePath, Path.ChangeExtension(savePaths[i], Path.GetExtension(savePath)), true);
                }
                Debug.Log(Path.GetFileName(savePath) + " 导表成功!");
                return true;
            }
            return false;
        }

        #endregion

        #region excel 导出核心

        private static Dictionary<int, List<Cell>> ExportTable(string path, int sheet)
        {
            return TT_GetDataRow(path, sheet);
        }

        /// <summary>
        /// 数据组合成字典
        /// </summary>
        /// <param name="path"></param>
        /// <param name="rowCollection"></param>
        /// <returns></returns>
        private static Dictionary<int, List<Cell>> GetDictionary(string path, DataRowCollection rowCollection)
        {
            int curHang = 0;
            var remarks = rowCollection[curHang].ItemArray.ToList();

            curHang++;

            var names = rowCollection[curHang].ItemArray.ToList();
            for (var i = 0; i < names.Count; i++)
            {
                if (names[i] != null && !string.IsNullOrEmpty(names[i].ToString())) continue;
                Debug.LogError(Path.GetFileName(path) +
                               string.Format(":变量名称有空余项！\n错误位置在坐标(行：{0}、列：{1})处！共应该有{2}个变量名称！", 0 + 2, i + 1,
                                   names.Count));
                return null;
            }

            curHang++;

            var types = rowCollection[curHang].ItemArray.ToList();
            for (var i = 0; i < types.Count; i++)
            {
                if (types[i] != null && !string.IsNullOrEmpty(types[i].ToString())) continue;
                Debug.LogError(Path.GetFileName(path) +
                               string.Format(":变量类型有空余项！\n错误位置在坐标(行：{0}、列：{1})处！共应该有{2}个变量名称！", 1 + 2, i + 1,
                                   types.Count));
                return null;
            }

            curHang++;

            var dic = new Dictionary<int, List<Cell>>();
            for (var j = curHang; j < rowCollection.Count; j++)
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

        #region 跨平台插件


        /// <summary>
        /// 获取内容
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sheet"></param>
        private static Dictionary<int, List<Cell>> TT_GetDataRow(string path, int sheet)
        {
            DataSet dataSet = TT_GetDataSet(path);
            if (dataSet == null)
            {
                Debug.Log(null);
                return null;
            }

            DataTable table = dataSet.Tables[sheet]; //返回第一张表

            DataRowCollection rowCollection = table.Rows; //返回一个行的集合

            Debug.Log(string.Format("行:{0}\n列:{1}", table.Rows.Count, table.Columns.Count));

            //此方法默认从第一行输出
            //第一行为描述
            //遍历行的集合，取得每一行的DataRow对象
            //默认表中第一行为表头，输出
            //人为规定第二行为变量名称
            //人为规定第三行为变量类型
            //共有多少列由第二行第三行列数来决定
            return GetDictionary(path, rowCollection);
        }

        /// <summary>
        /// 获取dataset
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static DataSet TT_GetDataSet(string path)
        {
            try
            {
                FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                DataSet result = excelReader.AsDataSet();
                stream.Dispose();
                stream.Close();
                return result;
            }
            catch (Exception err)
            {
                Debug.LogError(Path.GetFileName(path) + ":数据绑定Excel失败!失败原因：" + err.Message);
                return null;
            }
        }

        #endregion

        #endregion

    }
}