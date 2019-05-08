using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Library.Extensions;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Excel
{
    public class ExcelWindow : EditorWindow
    {

        [MenuItem("Tools/ExcelTools/ExportExcel2")]
        private static void ExportExcel()
        {
            if (!Directory.Exists(ExcelPath))
                Directory.CreateDirectory(ExcelPath);

            var files = Directory.GetFiles(ExcelPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(p => p.EndsWith(".xlsx") || p.EndsWith(".xls"))
                .Select(p => new
                {
                    path = p,
                    name = Path.GetFileNameWithoutExtension(p),
                    list = Path.GetFileNameWithoutExtension(p).Split('#')
                })
                .Where(p => !p.name.StartsWith("."))
                .ToLookup(p => p.list.First())
                .ToDictionary(p => p.Key, p => p.ToList().Select(q => q.path).ToArray());


            Debug.Log(files.Join("\n"));
            Debug.Log(JsonHelper.ToJson(files));

            string csPath = EditorUtils.DataPath + ClassPath;
            if (!Directory.Exists(csPath))
                Directory.CreateDirectory(csPath);
            foreach (KeyValuePair<string, string[]> pair in files)
            {
                foreach (string excelFullPath in pair.Value)
                {
                    string classInfoName = pair.Key.Replace("Table", "Info");
                    string classTableName = pair.Key;
                    EditorExcelTools.CreateClass(excelFullPath, classInfoName, classTableName,
                        string.Format(csPath + "{0}.cs", classTableName));

                    string path = EditorUtils.DataPath + excelFullPath;
                    string savePath = EditorUtils.DataPath + TablePath;
                    Debug.Log(path + "\n" + savePath);
                    EditorExcelTools.ExportTo(_tableType, excelFullPath, new[] {savePath});
                }
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/ExcelTools/Window")]
        private static void ShowWindow()
        {
            GetWindow<ExcelWindow>(false, "ExcelWindow", true);
        }

        public ExcelWindowConfirm excelWindowConfirm;

        private void ConfirmInit(ExcelInfo info, Action<ExcelInfo> callback)
        {
            isExcelDrawInfo = true;
            excelWindowConfirm = new ExcelWindowConfirm();
            excelWindowConfirm.Init(info, callback, () =>
            {
                isExcelDrawInfo = false;
                excelWindowConfirm = null;
            });
        }

        private static string AssetPath;
        private static string ExcelPath = "Assets/_Table/Excel/"; //Excel根目录
        public static string ClassPath = "Assets/Scripts/Data/Table/Info/"; //Cs根目录
        private static string TablePath = "Assets/Resources/Table/"; //Table根目录

        private ExcelData db = null;
        private int selectIndex;

        private static ExportType _tableType
        {
            get { return (ExportType)EditorPrefs.GetInt("tableType", 0); }
            set { EditorPrefs.SetInt("tableType", (int)value); }
        }

        private int limitWidth = Screen.width;

        private Vector2 _mScroll;
        private void Awake()
        {
            TraceMessage();
            AssetPath = RootDir + "ExcelDB.asset";

            if (!Directory.Exists(ExcelPath))
                Directory.CreateDirectory(ExcelPath);
            if (!Directory.Exists(ClassPath))
                Directory.CreateDirectory(ClassPath);
            if (!Directory.Exists(TablePath))
                Directory.CreateDirectory(TablePath);
            AssetDatabase.Refresh();
        }

        private static string RootDir = "";

        private static void TraceMessage()
        {
            RootDir = Path.GetDirectoryName(new StackTrace(true).GetFrame(1).GetFileName());
            if (RootDir == null) return;
            RootDir = Path.GetDirectoryName(RootDir);
            if (RootDir == null) return;
            RootDir = "Assets" + RootDir.Replace("\\", "/").Replace(Application.dataPath, "") + "/DB/";
            Debug.Log(RootDir);
        }

        private bool isDrawInfo;
        public Rect windowRect = new Rect(100, 100, 350, 200);

        private void DrawInfoWindow(int windowID)
        {
            this.Vertical(() =>
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("表总数目: " + db.content.Count);
                GUILayout.Label("AssetPath: " + AssetPath);
                GUILayout.Label("表来源路径: " + ExcelPath);
                GUILayout.Label("表输出路径: " + TablePath);
                GUILayout.Label("类文件路径: " + ClassPath);
                GUILayout.FlexibleSpace();
            });
        }

        private bool isExcelDrawInfo;
        public Rect excelWindowRect = new Rect(Screen.width - 280, 100, 360, 300);

        private void DrawExcelInfoWindow(int windowID)
        {
            this.Vertical(() =>
            {
                GUILayout.FlexibleSpace();
                if (excelWindowConfirm != null)
                {
                    excelWindowConfirm.DrawGUI(300);
                }
                GUILayout.FlexibleSpace();
            });
            GUI.DragWindow();
        }

        private void LoadFile()
        {
            //加载asset
            if (db != null) return;
            db = EditorUtils.CreateAsset<ExcelData>(AssetPath);
        }

        private void OnGUI()
        {
            LoadFile();
            GUILayout.BeginHorizontal();
            limitWidth = Screen.width;
            GUILayout.BeginVertical(GUILayout.MaxWidth(Screen.width));
            {
                LineMain();

                SplitHorizontal(15);

                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical();
                _mScroll = GUILayout.BeginScrollView(_mScroll, GUILayout.MaxWidth(limitWidth));
                ShowFolder();
                GUILayout.EndScrollView();
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            GUILayout.FlexibleSpace();

            BeginWindows();
            if (isDrawInfo)
            {
                windowRect = GUILayout.Window(1, windowRect, DrawInfoWindow, "Stats");
            }
            if (isExcelDrawInfo)
            {
                excelWindowRect = GUILayout.Window(2, excelWindowRect, DrawExcelInfoWindow, "Stats");
            }
            EndWindows();
        }

        private void LineMain()
        {
            GUILayout.BeginHorizontal("toolbar");

            isDrawInfo = GUILayout.Toggle(isDrawInfo, " Stats ", "toolbarbutton");

            _tableType = (ExportType)SetEnumPopup(_tableType, type =>
            {
                foreach (var t in db.content)
                {
                    t.SetData((ExportType)type);
                }

                var path = AssetDatabase.GetAllAssetPaths().FirstOrDefault(p => Path.GetFileNameWithoutExtension(p) == typeof(IDataCollection).Name);

                if (path != null)
                {
                    Debug.Log(path);
                    var content = File.ReadAllLines(path);
                    var array = Enum.GetValues(typeof(ExportType));
                    for (int i = 0; i < array.Length; i++)
                    {
                        content[i] = content[i].Replace("//", "");
                        if ((int)((ExportType)type) != i)
                        {
                            content[i] = content[i].Insert(0, "//");
                        }
                    }
                    File.WriteAllLines(path, content);
                }

                Debug.Log("宽度:" + Screen.width + "高度:" + Screen.height);
            }, 100, "ToolbarDropDown");

            GUILayout.FlexibleSpace();

            GUI.backgroundColor = Color.white;
            SetButton("+", () =>
            {
                ConfirmInit(new ExcelInfo(), (obj) =>
                {
                    db.content.Add(obj);
                    foreach (var t in db.content)
                    {
                        t.SetData(_tableType);
                    }
                    SaveAssets(true);
                });
            });

            GUI.backgroundColor = Color.white;
            SetButton("-", () =>
            {
                if (!DisplayDialog("是否删除选中项？")) return;
                selectIndex = 0;
                db.content.RemoveAll(p => p.isSelected);
            });

            GUI.backgroundColor = Color.white;
            SetButton("Sort", () =>
            {
                if (!DisplayDialog("是否排序？")) return;
                db.content.Sort((p1, p2) => string.Compare(p1.excelName, p2.excelName, StringComparison.Ordinal));
            });

            SetButton("OpenFolder", () =>
            {
                if (!DisplayDialog("是否打开表输出文件夹？")) return;
                EditorUtility.RevealInFinder(TablePath);
            }, null, false);

            SetButton("CancelSelect", () =>
            {
                if (!DisplayDialog("是否取消选中？")) return;
                Selection.activeObject = null;
                foreach (var info in db.content)
                {
                    info.isSelected = false;
                }
            });

            GUI.backgroundColor = Color.green;

            SetButton("ExportSelect", () =>
            {
                if (!DisplayDialog("是否批量导出表？未有选中项时将全部导出！")) return;
                try
                {
                    bool isAll = db.SelectedList.Contains(true) == false;
                    for (var i = 0; i < db.content.Count; i++)
                    {
                        var excelInfo = db.content[i];
                        if (isAll == false && excelInfo.isSelected == false) continue;
                        if (EditorUtility.DisplayCancelableProgressBar("导表", "操作中，请稍后..." + excelInfo.excelFullPath,
                            (float)(i + 1) / db.content.Count)) break;
                        ExportTable(excelInfo);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                    DisplayDialog("导表已完成！");
                }
            }, null, false);

            GUI.backgroundColor = Color.white;

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 显示一级
        /// </summary>
        private void ShowFolder()
        {
            for (var i = 0; i < db.content.Count; i++)
            {
                GUI.backgroundColor = i == selectIndex ? Color.cyan : Color.white;
                GUI.SetNextControlName(i.ToString());
                GUILayout.BeginHorizontal("AS TextArea", GUILayout.Height(20f));
                {
                    GUI.backgroundColor = Color.white;
                    var excelInfo = db.content[i];
                    bool focus = GUILayout.Toggle(excelInfo.isSelected, " " + (i + 1) + ". " + excelInfo.excelName,
                        GUILayout.Width(170));
                    if (excelInfo.isSelected != focus)
                    {
                        excelInfo.isSelected = focus;
                        selectIndex = i;
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath(excelInfo.excelFullPath, typeof(Object));
                        GUI.FocusControl(i.ToString());
                        Debug.Log("选中了" + excelInfo.excelName);
                        if (excelWindowConfirm != null)
                        {
                            ConfirmInit(excelInfo, (obj) => { });
                        }
                    }

                    GUILayout.FlexibleSpace();

                    SetButton(excelInfo.tableName, () =>
                    {
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath(TablePath + excelInfo.tableName,
                            typeof(Object));
                    }, guiStyle, false);

                    SetButton("类：" + excelInfo.classTableName, () =>
                    {
                        Selection.activeObject =
                            AssetDatabase.LoadAssetAtPath(ClassPath + excelInfo.classTableName + ".cs", typeof(Object));
                    }, 140, guiStyle, false);

                    SetButton("类：" + excelInfo.classInfoName, () =>
                    {
                        Selection.activeObject =
                            AssetDatabase.LoadAssetAtPath(ClassPath + excelInfo.classTableName + ".cs", typeof(Object));
                    }, 140, guiStyle, false);

                    SetButton("Open", () =>
                    {
                        ConfirmInit(excelInfo, (obj) => { });
                    }, 50, null, false);

                    SetButton("CreateClass", () =>
                    {
                        if (!DisplayDialog("是否创建选中文件的类？")) return;
                        CreateClassCheck(excelInfo);
                    }, 90);

                    GUI.backgroundColor = Color.red;
                    SetButton("Del", () =>
                    {
                        if (DisplayDialog("是否删除此条目？"))
                        {
                            selectIndex = 0;
                            db.content.RemoveAt(i);
                            SaveAssets(true);
                        }
                    }, 50, null, false);

                    GUI.backgroundColor = Color.green;
                    SetButton("Export", () =>
                    {
                        ExportTable(excelInfo);
                    }, 60, null, false);
                }
                GUILayout.EndHorizontal();
                GUI.backgroundColor = Color.white;
            }
        }
        private void CreateClassCheck(ExcelInfo excelInfo)
        {
            bool check = EditorUtils.GetClassByName(excelInfo.classInfoName);
            if (check)
            {
                if (DisplayDialog("已有此类！是否继续创建!"))
                {
                    CreateClass(excelInfo);
                }
            }
            else
            {
                CreateClass(excelInfo);
            }
        }

        private void CreateClass(ExcelInfo excelInfo)
        {
            string csPath = EditorUtils.DataPath + ClassPath;
            if (!Directory.Exists(csPath))
            {
                Directory.CreateDirectory(csPath);
            }
            EditorExcelTools.CreateClass(excelInfo.excelFullPath, excelInfo.classInfoName, excelInfo.classTableName,
                string.Format(csPath + "{0}.cs", excelInfo.classTableName));
        }

        public static void ExportTable(ExcelInfo excelInfo)
        {
            string path = EditorUtils.DataPath + excelInfo.excelFullPath;
            string savePath = EditorUtils.DataPath + TablePath;
            Debug.Log(path + "\n" + savePath);
            if (EditorExcelTools.ExportTo(_tableType, excelInfo.excelFullPath, new[] { savePath }))
            {
                AssetDatabase.Refresh();
            }
        }

        private bool DisplayDialog(string msg)
        {
            return EditorUtility.DisplayDialog("提示", msg, "确认", "取消");
        }

        private void OnInspectorUpdate()
        {
            this.Repaint();
        }

        private void OnSelectionChange()
        {
            Repaint();
        }

        void SplitHorizontal(int height)
        {
            GUILayout.BeginHorizontal("AS TextArea", GUILayout.Height(height));
            GUILayout.EndHorizontal();
        }

        void SplitVertical(int width)
        {
            GUILayout.BeginVertical("AS TextArea", GUILayout.Width(width));
            GUILayout.EndVertical();
        }

        #region 基本控件封装

        private GUIStyle guiStyle = null;

        private void OnEnable()
        {
            if (guiStyle == null)
            {
                GUIStyleState guiStyleState = new GUIStyleState()
                {
                    background = null,
                    textColor = new Color(180.0f / 255, 180.0f / 255, 180.0f / 255, 1)
                };
                guiStyle = new GUIStyle("button")
                {
                    alignment = TextAnchor.LowerLeft,
                    normal = guiStyleState,
                    hover = guiStyleState,
                    active = guiStyleState,
                    focused = guiStyleState,
                    onNormal = guiStyleState,
                    onHover = guiStyleState,
                    onActive = guiStyleState,
                    onFocused = guiStyleState,
                };
            }
        }

        protected void SetButton(string msg, Action action, int width, GUIStyle style = null, bool isSave = true)
        {
            if (style == null)
            {
                if (!GUILayout.Button(msg, "toolbarbutton", GUILayout.Width(width))) return;
            }
            else
            {
                if (!GUILayout.Button(msg, style, GUILayout.Width(width))) return;
            }
            if (action != null)
                action();
            SaveAssets(isSave);
            Focus();
        }

        protected void SetButton(string msg, Action action, GUIStyle style = null, bool isSave = true)
        {
            if (style == null)
            {
                if (!GUILayout.Button(msg, "toolbarbutton")) return;
            }
            else
            {
                if (!GUILayout.Button(msg, style)) return;
            }
            if (action != null)
                action();
            SaveAssets(isSave);
            Focus();
        }

        protected Enum SetEnumPopup(Enum t, Action<Enum> action, int width, GUIStyle style = null, bool isSave = true)
        {
            Enum selectedNew = t;
            selectedNew = style == null
                ? EditorGUILayout.EnumPopup(selectedNew, GUILayout.MaxWidth(width))
                : EditorGUILayout.EnumPopup(selectedNew, style, GUILayout.MaxWidth(width));
            if (selectedNew.Equals(t)) return t;
            t = selectedNew;
            if (action != null)
                action(t);
            SaveAssets(isSave);
            Focus();
            return t;
        }

        protected bool SetToggle(string msg, bool t, Action<bool> action, int width, bool isSave = true)
        {
            bool toggle = GUILayout.Toggle(t, msg, GUILayout.MaxWidth(width));
            if (toggle == t) return t;
            t = toggle;
            if (action != null)
                action(t);
            SaveAssets(isSave);
            Focus();
            return t;
        }

        protected virtual void SaveAssets(bool isSave = false)
        {
            if (!isSave) return;
            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void Vertical(Action callAction, int spaceH = 10)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(spaceH);
                EditorGUILayout.BeginVertical();
                {
                    callAction();
                }
                EditorGUILayout.EndVertical();
                GUILayout.Space(spaceH);
            }
            EditorGUILayout.EndHorizontal();
        }

        #endregion

    }
}