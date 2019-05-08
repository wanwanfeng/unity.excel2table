using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Library.Helper;

namespace Excel
{
    /// <summary>
    /// Json方式解析
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TK"></typeparam>
    [Description(".json")]
    public class DataTableJson<TK, T> : DataCollection<TK, T> where T : class, new()
    {
        /// <summary>
        /// 解析表
        /// </summary>
        protected override object ProcessData(object obj)
        {
            var content = (string) base.ProcessData(obj);
            return Library.Helper.JsonHelper.ToObject<List<T>>(content);
        }
    }

    /// <summary>
    /// Xml键值对解析
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TK"></typeparam>
    [Description(".xml")]
    public class DataTableXml<TK, T> : DataCollection<TK, T> where T : class, new()
    {
        /// <summary>
        /// 解析表
        /// </summary>
        protected override object ProcessData(object obj)
        {
            var content = (string) base.ProcessData(obj);
            return XmlHelper.ToObject<List<T>>(content);
        }
    }

    /// <summary>
    /// Xml属性解析
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TK"></typeparam>
    [Description(".xml")]
    public class DataTableXmlAttribute<TK, T> : DataCollection<TK, T> where T : class, new()
    {
        /// <summary>
        /// 解析表
        /// </summary>
        protected override object ProcessData(object obj)
        {
            var content = (string) base.ProcessData(obj);
            return EditorExcelRead.FromXmlAttribute<T>(content);
        }
    }

    /// <summary>
    /// Csv方式解析
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TK"></typeparam>
    [Description(".csv")]
    public class DataTableCsv<TK, T> : DataCollection<TK, T> where T : class, new()
    {
        /// <summary>
        /// 解析表
        /// </summary>
        protected override object ProcessData(object obj)
        {
            var content = (string)base.ProcessData(obj);
            return EditorExcelRead.FromCsv<T>(content);
        }
    }

    /// <summary>
    /// Txt方式解析
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TK"></typeparam>
    [Description(".txt")]
    public class DataTableTxt<TK, T> : DataCollection<TK, T> where T : class, new()
    {
        /// <summary>
        /// 解析表
        /// </summary>
        protected override object ProcessData(object obj)
        {
            var content = (string)base.ProcessData(obj);
            return EditorExcelRead.FromTxt<T>(content);
        }
    }

    /// <summary>
    /// Bytes方式解析
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TK"></typeparam>
    [Description(".bytes")]
    public class DataTableBytes<TK, T> : DataCollection<TK, T> where T : class, new()
    {
        /// <summary>
        /// 解析表
        /// </summary>
        protected override object ProcessData(object obj)
        {
            var content = (string)base.ProcessData(obj);
            return EditorExcelRead.FromBytes<T>(Encoding.UTF8.GetBytes(content));
        }
    }

    /// <summary>
    /// Asset方式解析
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TK"></typeparam>
    [Description(".asset")]
    public class DataTableAsset<TK, T> : DataCollection<TK, T> where T : class, new()
    {
        /// <summary>
        /// 解析表
        /// </summary>
        protected override object ProcessData(object obj)
        {
            var table = obj as DataTableAsset<TK, T>;
            return table == null ? null : table.List;
        }
    }
}