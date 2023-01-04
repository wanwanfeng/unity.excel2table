using System;
using System.Linq;
using UnityEngine;

namespace Excel
{

    [CreateAssetMenu(fileName = "ExcelSetting", menuName = "New ExcelSetting", order = 1)]
    public partial class ExcelSetting : ScriptableObject, ISerializationCallbackReceiver
    {
        private static ExcelSetting _instance;
        public static ExcelSetting Instance => _instance ?? (_instance = UnityEngine.Resources.Load<ExcelSetting>("ExcelSetting"));
        public Type Setting => typeof(ImpHelper).Assembly.GetType(SelectList[Selected]);
        [SerializeField] private string[] SelectList;
        [SerializeField] private int Selected;

        public string ExcelPath = "Excel/"; //Excel根目录
        public string TablePath = "Temp/Excel/"; //Table根目录

        public void OnBeforeSerialize()
        {
            SelectList =  typeof(ImpHelper).Assembly.GetTypes().Where(p => typeof(ImpHelper).IsAssignableFrom(p)).Where(p => !p.IsAbstract).Select(p => p.FullName).ToArray();
        }

        public void OnAfterDeserialize()
        {
            
        }
    }
}