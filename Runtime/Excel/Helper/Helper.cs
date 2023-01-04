using System;

namespace Excel
{
    public abstract partial class Helper : FileHelper
    {
        /// <summary>
        /// ·Ö¸î×Ö·û´®(·µ»ØÇ°×º)
        /// </summary>
        /// <param name="str">Ô´×Ö·û´®</param>
        /// <param name="suffix">ºó×º</param>
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
