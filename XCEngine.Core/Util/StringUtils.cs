using System.Text;
using System.Text.RegularExpressions;

namespace XCEngine.Core
{
    /// <summary>
    /// 字符串助手函数
    /// </summary>
    public class StringUtils
    {
        /// <summary>
        /// 字符串转成驼峰命名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToCamelCase(string str)
        {
            var words = str.Split(new[] { "_", " " }, StringSplitOptions.RemoveEmptyEntries);
            words = words
                .Select(word => char.ToUpper(word[0]) + word.Substring(1))
                .ToArray();
            return string.Join(string.Empty, words);
        }

        /// <summary>
        /// 字符串种是否有中文
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool HasChinese(string str)
        {
            return Regex.IsMatch(str, @"[\u4e00-\u9fa5]");
        }

        /// <summary>
        /// 字典转成字符串
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dic"></param>
        /// <param name="delimiter1"></param>
        /// <param name="delimiter2"></param>
        /// <returns></returns>
        public static string DictionaryToString<K, V>(IReadOnlyDictionary<K, V> dic, string delimiter1 = ";", string delimiter2 = ",") where K : notnull
        {
            if (dic == null)
            {
                return String.Empty;
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var iter in dic)
            {
                stringBuilder.Append(iter.Key == null ? String.Empty : iter.Key.ToString()).Append(delimiter2)
                    .Append(iter.Value == null ? String.Empty : iter.Value.ToString()).Append(delimiter1);
            }

            if (stringBuilder.Length > 0)
            {
                return stringBuilder.ToString(0, stringBuilder.Length - 1);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 列表转字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string EnumerableToString<T>(IEnumerable<T> enumerable, string delimiter = ",")
        {
            if (enumerable == null)
            {
                return String.Empty;
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var str in enumerable)
            {
                stringBuilder.Append(str == null ? String.Empty : str.ToString()).Append(delimiter);
            }

            if (stringBuilder.Length > 0)
            {
                return stringBuilder.ToString(0, stringBuilder.Length - 1);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 版本字符串传整数（最多支持3位版本，每位999个）
        /// </summary>
        /// <param name="versionStr"></param>
        /// <returns></returns>
        public static int VersionStringToInt(string versionStr)
        {
            int intVersion = 0;
            string[] versionList = versionStr.Split('.');
            for (int i = 0; i < versionList.Length; ++i)
            {
                intVersion = intVersion * 1000 + int.Parse(versionList[i]);
            }
            return intVersion;
        }

        /// <summary>
        /// 路径转为文件夹和名字
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dir"></param>
        /// <param name="name"></param>
        public static void SplitPathToDirAndName(string path, out string dir, out string name)
        {
            int index = path.LastIndexOf('/');
            if (index == -1)
            {
                dir = string.Empty;
                name = path;
            }
            else
            {
                dir = path.Substring(0, index);
                name = path.Substring(index + 1);
            }
        }

        /// <summary>
        /// 字符串反转
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        /// <summary>
        /// 字符串hash
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int Hash(string s)
        {
            int hash = 5381;
            int index = 0;


            while (index < s.Length)
            {
                hash += (hash << 5) + (s[index]);
                index++;
            }

            return (hash & 0x7FFFFFFF);
        }
    }
}
