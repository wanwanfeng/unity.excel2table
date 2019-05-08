using Excel;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(DefaultAsset))]
public class ExcelInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var path = AssetDatabase.GetAssetPath(target);
        GUI.enabled = true;
        if (path.EndsWith(".xls") || path.EndsWith(".xlsx"))
        {
            if (GUILayout.Button("Export"))
            {
                var excelInfo  = new ExcelInfo();
                excelInfo.SetData(path);
                ExcelWindow.ExportTable(excelInfo);
            }
        }
    }
}
