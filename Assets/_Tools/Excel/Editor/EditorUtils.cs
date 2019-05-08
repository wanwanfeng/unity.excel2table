using System;
using System.IO;
using UnityEngine;
using System.Reflection;
using Library.Helper;
using UnityEditor;

namespace Excel
{
    public partial class EditorUtils
    {
        /// <summary>
        /// 获取本地Application.dataPath路径
        /// </summary>
        public static string DataPath
        {
            get { return Application.dataPath.Replace("Assets", ""); }
        }

        /// <summary>
        /// 创建Asset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        internal static T CreateAsset<T>(string assetPath) where T : ScriptableObject
        {
            FileHelper.CreateDirectory(assetPath);
            var db = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)) as T;
            if (db != null) return db;
            db = ScriptableObject.CreateInstance(typeof (T)) as T;
            AssetDatabase.CreateAsset(db, assetPath);
            return db;
        }

        /// <summary>
        /// 创建Asset
        /// </summary>
        /// <param name="className"></param>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        internal static T CreateAsset<T>(string className, string assetPath) where T : ScriptableObject
        {
            FileHelper.CreateDirectory(assetPath);
            var db = AssetDatabase.LoadAssetAtPath(assetPath, GetType(className)) as T;
            if (db != null) return db;
            db = ScriptableObject.CreateInstance(className) as T;
            AssetDatabase.CreateAsset(db, assetPath);
            return db;
        }


        private static string Assembly_CSharp_Path
        {
            get { return Environment.CurrentDirectory + "/Library/ScriptAssemblies/Assembly-CSharp.dll"; }
        }

        public static Type GetType(string className)
        {
            //因为在不同项目下
            return Assembly.LoadFrom(Assembly_CSharp_Path).GetType(className);
        }

        public static bool GetClassByName(string className)
        {
            Type type = GetType(className);
            if (type == null)
            {
                Debug.LogError("不存在类：" + className);
                return false;
            }
            Debug.Log("存在类：" + className);
            return true;
        }
    }
}
