using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonFilter.Helper
{
    public class TypeConvertHelper
    {
        /// <summary>
        /// 转换成GUID数组
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Guid[] AsGuidArray(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return Array.Empty<Guid>();
            }

            string[] parts = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<Guid> guids = new List<Guid>();

            foreach (string part in parts)
            {
                if (Guid.TryParse(part.Trim(), out Guid guid))
                {
                    guids.Add(guid);
                }
            }

            return guids.ToArray();
        }

        private static readonly Dictionary<Type, Func<string, object>> _tryParseMethods =
        new Dictionary<Type, Func<string, object>>()
    {
        { typeof(int), str => int.TryParse(str, out var result) ? (object)result : null },
        { typeof(double), str => double.TryParse(str, out var result) ? (object)result : null },
        { typeof(bool), str => bool.TryParse(str, out var result) ? (object)result : null },
        { typeof(DateTime), str => DateTime.TryParse(str, out var result) ? (object)result : null },
        { typeof(decimal), str => decimal.TryParse(str, out var result) ? (object)result : null },
        { typeof(long), str => long.TryParse(str, out var result) ? (object)result : null },
        { typeof(float), str => float.TryParse(str, out var result) ? (object)result : null },
        { typeof(short), str => short.TryParse(str, out var result) ? (object)result : null },
        { typeof(byte), str => byte.TryParse(str, out var result) ? (object)result : null },
        { typeof(Guid), str => Guid.TryParse(str, out var result) ? (object)result : null },
        { typeof(TimeSpan), str => TimeSpan.TryParse(str, out var result) ? (object)result : null }
    };

        public static object ConvertType(object obj, Type targetType)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return GetDefaultValue(targetType);
            }

            // 如果目标类型是字符串，直接调用 ToString()
            if (targetType == typeof(string))
                return obj.ToString();

            // bool 类型
            if (targetType == typeof(bool))
            {
                if (obj is Boolean b)
                {
                    return b;
                }

                var s = obj.ToString();

                if (s == "1" || string.Compare(s, "true", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return true;
                }

                return false;
            }

            // 如果目标类型是枚举，尝试从字符串或数字转换
            if (targetType.IsEnum)
            {
                if (obj is string str)
                {
                    // 尝试从字符串解析为枚举
                    if (Enum.TryParse(targetType, str, true, out var result))
                    {
                        return result;
                    }
                    else
                    {
                        throw new InvalidOperationException($"无法将字符串 '{str}' 转换为枚举类型 {targetType}。");
                    }
                }
                else if (obj is int || obj is long || obj is short || obj is byte)
                {
                    // 如果是数字，直接转换为枚举
                    return Enum.ToObject(targetType, obj);
                }
                else
                {
                    throw new InvalidOperationException($"无法将类型 {obj.GetType()} 转换为枚举类型 {targetType}。");
                }
            }

            // 如果是时间类型
            if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
            {
                if (obj is long l || long.TryParse(obj.ToString(), out l))
                {
                    var time = DateTimeOffset.FromUnixTimeMilliseconds(l).LocalDateTime;
                    return time;
                }

                // 尝试使用 DateTime.TryParse 解析
                if (DateTime.TryParse(obj.ToString(), out var time2))
                {
                    return time2;
                };
            }

            // 如果源类型可以直接赋值给目标类型，直接返回
            if (targetType.IsAssignableFrom(obj.GetType()))
                return obj;

            // 如果是常见类型，使用 TryParse 方法
            if (_tryParseMethods.TryGetValue(targetType, out var tryParseFunc))
            {
                if (obj is string str)
                    return tryParseFunc(str);
                else
                    return tryParseFunc(obj.ToString());
            }

            // 如果目标类型是可空类型，递归调用 ConvertType 处理基础类型
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(targetType);
                return ConvertType(obj, underlyingType);
            }

            // 如果以上都不适用，尝试使用 Convert.ChangeType
            try
            {
                return System.Convert.ChangeType(obj, targetType);
            }
            catch
            {
                throw new InvalidOperationException($"Cannot convert {obj.GetType()} to {targetType}.");
            }
        }

        public static T ConvertType<T>(object obj)
        {
            return (T)ConvertType(obj, typeof(T));
        }


        static object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                if (Nullable.GetUnderlyingType(type) != null)
                {
                    return null;
                }
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}
