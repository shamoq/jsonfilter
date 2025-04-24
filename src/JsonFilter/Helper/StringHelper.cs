using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonFilter.Helper
{
    public class StringHelper
    {

        /// <summary>
        /// 驼峰命名和蛇形命名转换成帕斯卡命名
        /// helloWorld → HelloWorld
        /// hello_world → HelloWorld
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            StringBuilder result = new StringBuilder();
            bool shouldCapitalize = true;

            foreach (char c in input)
            {
                if (c == '_')
                {
                    shouldCapitalize = true;
                    continue;
                }

                if (shouldCapitalize)
                {
                    result.Append(char.ToUpper(c));
                    shouldCapitalize = false;
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// 将蛇形命名和帕斯卡命名转换为驼峰命名
        /// hello_world → helloWorld
        /// HelloWorld → helloWorld
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToCamelCase(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            StringBuilder result = new StringBuilder();
            bool shouldCapitalize = true;

            foreach (char c in input)
            {
                if (c == '_')
                {
                    shouldCapitalize = true;
                    continue;
                }

                if (shouldCapitalize)
                {
                    result.Append(char.ToLower(c));
                    shouldCapitalize = false;
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }
    }
}
