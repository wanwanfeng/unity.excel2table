using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Excel
{
    public class ExcelWindowConfirm
    {
        public void Init(ExcelInfo info, Action<ExcelInfo> callback, Action closeAction)
        {
            this.excelInfo = info;
            if (info.excelFullPath != "" || oldTarget != selectTarget)
            {
                selectTarget = AssetDatabase.LoadAssetAtPath(info.excelFullPath, typeof (Object));
            }
            this.callback = callback;
            this.closeAction = closeAction;
        }

        private ExcelInfo excelInfo;
        private Action<ExcelInfo> callback;
        private Action closeAction;

        private Object oldTarget = null;
        private Object selectTarget = null;
        private Object outPut = null;

        private bool? rename = null;
        private bool? check = null;

        private int offset = 10;

        public void DrawGUI(int limit = 0)
        {
            GUILayout.BeginVertical(GUILayout.MaxWidth(limit));
            GUILayout.Space(offset);
            selectTarget = EditorGUILayout.ObjectField("Excel来源文件：", selectTarget, typeof (Object), false);

            if (oldTarget != selectTarget)
            {
                oldTarget = selectTarget;
                check = null;
                if (excelInfo != null && selectTarget != null)
                {
                    rename = excelInfo.SetData(AssetDatabase.GetAssetPath(selectTarget));
                }
            }

            if (excelInfo != null && selectTarget != null)
            {
                outPut = EditorGUILayout.ObjectField("Excel输出文件：", outPut, typeof (Object), false);
                EditorGUILayout.TextField("名称：", excelInfo.excelName);
                EditorGUILayout.TextField("来源路径：", excelInfo.excelFullPath);
                EditorGUILayout.TextField("输出路径：", excelInfo.tableName);
                EditorGUILayout.TextField("Sheet：", excelInfo.sheet.ToString());
                EditorGUILayout.TextField("Table：", excelInfo.classTableName);
                EditorGUILayout.TextField("Info：", excelInfo.classInfoName);

                GUILayout.Space(offset);
                if (rename == true)
                {
                    if (EditorUtility.DisplayDialog("提示", "名称不符合规范！是否继续重命名？", "确定", "取消"))
                    {
                        string sourcePath = AssetDatabase.GetAssetPath(selectTarget);
                        FileInfo fileInfo = new FileInfo(sourcePath);
                        string temp = fileInfo.Name.Replace(fileInfo.Extension, "");
                        string newPath = sourcePath.Replace(temp, temp + "Table");
                        File.Move(EditorUtils.DataPath + sourcePath, EditorUtils.DataPath + newPath);
                        AssetDatabase.Refresh();
                        Debug.LogError("名字不符和规范！已经重命名！");
                        selectTarget = AssetDatabase.LoadAssetAtPath(newPath, typeof (Object));
                        rename = excelInfo.SetData(newPath);
                    }
                    else
                    {
                        this.CloseWindow();
                    }
                }
                else if (rename == false)
                {
                    if (GUILayout.Button("Check"))
                    {
                        check = EditorUtils.GetClassByName(excelInfo.classTableName);
                        check = EditorUtils.GetClassByName(excelInfo.classInfoName);
                    }
                }
            }

            GUILayout.Space(offset);

            GUILayout.BeginHorizontal();
            if (check == true)
            {
                if (GUILayout.Button("Yes"))
                {
                    if (callback != null)
                    {
                        callback(excelInfo);
                    }
                    CloseWindow();
                }
                if (GUILayout.Button("Not"))
                {
                    CloseWindow();
                }
            }
            else if (check == false)
            {
                if (GUILayout.Button("CreateClass"))
                {
                    if (excelInfo != null)
                    {
                        string csPath = EditorUtils.DataPath + ExcelWindow.ClassPath;
                        if (!Directory.Exists(csPath))
                        {
                            Directory.CreateDirectory(csPath);
                        }
                        EditorExcelTools.CreateClass(excelInfo.excelFullPath,excelInfo.classInfoName,excelInfo.classTableName,string.Format(csPath + "{0}.cs", excelInfo.classTableName));
                    }
                    AssetDatabase.Refresh();
                    check = true;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(offset);
            if (GUILayout.Button("Close"))
            {
                CloseWindow();
            }

            GUILayout.EndVertical();
        }

        private void CloseWindow()
        {
            excelInfo = null;
            callback = null;
            oldTarget = null;
            selectTarget = null;
            check = false;
            if (closeAction != null)
                closeAction();
        }
    }
}

