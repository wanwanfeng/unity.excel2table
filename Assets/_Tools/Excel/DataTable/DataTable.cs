using UnityEngine;
using System.Collections.Generic;
using Library.Helper;
using Library.LitJson;
using Object = UnityEngine.Object;

namespace Excel
{
    /// <summary>
    /// Json方式解析
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TK"></typeparam>
    public class DataTableJson<TK, T> : DataCollection<TK, T> where T : class, new()
    {
        public override string DataSuffix
        {
            get { return ".json"; }
        }

        /// <summary>
        /// 解析表
        /// </summary>
        protected override void ProcessData(Object obj)
        {
            var table = obj as TextAsset;
            if (table == null) return;

            var content = table.text.Trim();
            if (string.IsNullOrEmpty(content)) return;

            Clear();
            Elements = new List<T>(LitJsonHelper.ToObject<T[]>(content));
        }
    }

    /// <summary>
    /// Xml键值对解析
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TK"></typeparam>
    public class DataTableXml<TK, T> : DataCollection<TK, T> where T : class, new()
    {
        public override string DataSuffix
        {
            get { return ".xml"; }
        }

        /// <summary>
        /// 解析表
        /// </summary>
        protected override void ProcessData(Object obj)
        {
            var table = obj as TextAsset;
            if (table == null) return;

            var content = table.text.Trim();
            if (string.IsNullOrEmpty(content)) return;

            Clear();
            Elements = new List<T>(XmlHelper.ToObject<T[]>(content));
        }
    }

    /// <summary>
    /// Xml属性解析
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TK"></typeparam>
    public class DataTableXmlAttribute<TK, T> : DataCollection<TK, T> where T : class, new()
    {
        public override string DataSuffix
        {
            get { return ".xml"; }
        }

        /// <summary>
        /// 解析表
        /// </summary>
        protected override void ProcessData(Object obj)
        {
            var table = obj as TextAsset;

            if (table == null) return;
            var content = table.text.Trim();
            if (string.IsNullOrEmpty(content)) return;

            Clear();
            Elements = EditorExcelRead.FromXmlAttribute<T>(content);
        }
    }

    /// <summary>
    /// Csv方式解析
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TK"></typeparam>
    public class DataTableCsv<TK, T> : DataCollection<TK, T> where T : class, new()
    {
        public override string DataSuffix
        {
            get { return ".csv"; }
        }

        /// <summary>
        /// 解析表
        /// </summary>
        protected override void ProcessData(Object obj)
        {
            var table = obj as TextAsset;
            if (table == null) return;

            var content = table.text.Trim();
            if (string.IsNullOrEmpty(content)) return;

            Clear();
            Elements = EditorExcelRead.FromCsv<T>(content);
        }
    }

    /// <summary>
    /// Txt方式解析
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TK"></typeparam>
    public class DataTableTxt<TK, T> : DataCollection<TK, T> where T : class, new()
    {
        public override string DataSuffix
        {
            get { return ".txt"; }
        }

        /// <summary>
        /// 解析表
        /// </summary>
        protected override void ProcessData(Object obj)
        {
            var table = obj as TextAsset;
            if (table == null) return;

            var content = table.text.Trim();
            if (string.IsNullOrEmpty(content)) return;

            Clear();
            Elements = EditorExcelRead.FromTxt<T>(content);
        }
    }

    /// <summary>
    /// Bytes方式解析
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TK"></typeparam>
    public class DataTableBytes<TK, T> : DataCollection<TK, T> where T : class, new()
    {
        public override string DataSuffix
        {
            get { return ".bytes"; }
        }

        /// <summary>
        /// 解析表
        /// </summary>
        protected override void ProcessData(Object obj)
        {
            var table = obj as TextAsset;
            if (table == null) return;

            Clear();
            Elements = EditorExcelRead.FromBytes<T>(table.bytes);
        }
    }

    /// <summary>
    /// Asset方式解析
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TK"></typeparam>
    public class DataTableAsset<TK, T> : DataCollection<TK, T> where T : class, new()
    {
        public override string DataSuffix
        {
            get { return ".asset"; }
        }

        /// <summary>
        /// 解析表
        /// </summary>
        protected override void ProcessData(UnityEngine.Object obj)
        {
            var table = obj as DataTableAsset<TK,T>;
            if (table == null) return;

            Elements = table.Elements;
        }
    }
}