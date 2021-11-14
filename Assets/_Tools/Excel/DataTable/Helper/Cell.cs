using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Library.Extensions;
using PLitJson;

namespace Excel
{
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

	public static partial class HelperExtensions
    {
        public static string Write(this Cell cell)
        {
            if (string.IsNullOrEmpty(cell.value))
            {
                return "";
            }
            string name = cell.name;
            string value = cell.value;
            string type = cell.type.ToLower();
            switch (type)
            {
                case "bool":
                case "int":
                case "float":
                case "double":
                case "long":
                case "decimal":
                case "byte":
                case "short":
                    return string.Format("{0} = {1}", name, value);
                    break;
                case "string":
                case "char":
                    return string.Format("{0} = \"{1}\"", name, value);
                    break;
                case "bool[]":
                case "int[]":
                case "float[]":
                case "double[]":
                case "long[]":
                case "decimal[]":
                case "byte[]":
                case "short[]":
                case "list<bool>":
                case "list<int>":
                case "list<float>":
                case "list<double>":
                case "list<long>":
                case "list<decimal>":
                case "list<byte>":
                case "list<short>":
                    {
                        type = type.Replace("[]", "").Replace("list<", "").Replace(">", "");
                        return string.Format("{0} = new {1}[]{2}{3}{4}", name, type, "{", value.Replace("|", ","), "}");
                    }
                case "string[]":
                case "list<string>":
                    {
                        type = type.Replace("[]", "").Replace("list<", "").Replace(">", "");
                        return string.Format("{0} = new {1}[]{2}{3}{4}", name, type, "{", value.Replace("|", ","), "}");
                    }
                    return "";
                default:
                    return string.Format("{0} {1} = {2}", type, name, value);
            }
            return "";
        }
    }
}