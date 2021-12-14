using Library.Helper;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Excel
{
    public partial class EditorUtils
    {
        /// <summary>
        /// 创建Asset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        internal static T CreateAsset<T>(string assetPath) where T : ScriptableObject
        {
            DirectoryHelper.CreateDirectory(assetPath);
            var db = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)) as T;
            if (db != null) return db;
            db = ScriptableObject.CreateInstance(typeof(T)) as T;
            AssetDatabase.CreateAsset(db, assetPath);
            return db;
        }

        /// <summary>
        /// 创建Asset
        /// </summary>
        /// <param name="className"></param>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        internal static ScriptableObject CreateAsset(string className, string assetPath)
        {
            DirectoryHelper.CreateDirectory(assetPath);
            var type = typeof(IDataCollection).Assembly.GetTypes().FirstOrDefault(p => p.Name == className);
            var db = AssetDatabase.LoadAssetAtPath(assetPath, type) as ScriptableObject;
            if (db != null) return db;
            db = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(db, assetPath);
            return db;
        }
    }
}
