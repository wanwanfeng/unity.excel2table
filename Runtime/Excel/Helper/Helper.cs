using System;

namespace Excel
{
    public abstract partial class Helper : FileHelper
    {
        /// <summary>
        /// �ָ��ַ���(����ǰ׺)
        /// </summary>
        /// <param name="str">Դ�ַ���</param>
        /// <param name="suffix">��׺</param>
        internal static string Get(string str, out string suffix)
        {
            suffix = "";
            for (var i = str.Length - 1; i >= 0; i--)
            {
                if (char.IsDigit(str, i))
                {
                    suffix = str[i] + suffix;
                }
                else
                {
                    break;
                }
            }

            return string.IsNullOrEmpty(suffix) ? str : str.Replace(suffix, "");
        }
    }
}
