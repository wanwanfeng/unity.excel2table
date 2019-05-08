using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Excel
{
    [CreateAssetMenu(fileName = "ExcelDB",menuName = "DB/ExcelDB")]
    public class ExcelData : ScriptableObject
    {
        public List<ExcelInfo> content = new List<ExcelInfo>();

        public List<bool> SelectedList
        {
            get { return content.Select(excelInfo => excelInfo.isSelected).ToList(); }
        }
    }

    [Serializable]
    public class ExcelInfo
    {
        public string excelName = "";
        public string excelFullPath = ""; //excel 原路径
        public string tableName = ""; //table 生成路径
        public string classTableName = "";
        public string classInfoName = "";
        public int id = 0;
        public int sheet;
        public bool isSelected;

        public void SetData(ExportType type)
        {
            FileInfo fileInfo = new FileInfo(excelFullPath);

            var first =
                typeof (ExportType).GetField(type.ToString())
                    .GetCustomAttributes(false)
                    .OfType<DescriptionAttribute>()
                    .FirstOrDefault();
            if (first != null)
            {
                tableName = fileInfo.Name.Replace(fileInfo.Extension, first.Description);
            }
            else
            {
                Debug.LogError("获取路径扩展名出错");
            }
        }

        public bool SetData(string sourcePath)
        {
            bool rename = false;

            {
                FileInfo fileInfo = new FileInfo(sourcePath);
                string temp = fileInfo.Name.Replace(fileInfo.Extension, "");
                if (!temp.EndsWith("Table"))
                {
                    rename = true;
                }
            }

            if (!rename)
            {
                excelFullPath = sourcePath;
                FileInfo fileInfo = new FileInfo(excelFullPath);
                excelName = fileInfo.Name;
                classTableName = fileInfo.Name.Replace(fileInfo.Extension, "");
                classInfoName = classTableName.Replace("Table", "Info");
            }
           
            return rename;
        }
    }
}

