using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonFilter
{
    public enum OpEnum
    {
        /// <summary>!= 不等于</summary>
        notequal,

        /// <summary>= 等于</summary>
        equal,

        /// <summary>大于 &gt;</summary>
        greater,

        /// <summary>大于等于 &gt;=</summary>
        greaterorequal,

        /// <summary>/* 小于 */</summary>
        less,

        /// <summary>/* 小于等于 */</summary>
        lessorequal,

        /// <summary>包含 like</summary>
        contains,

        /// <summary> startsWith </summary>
        startsWith,

        /// <summary> endsWith </summary>
        endsWith,

        /// <summary>在 、、之中</summary>
        In,

        /// <summary>在 、、之间</summary>
        between,


        /// <summary> is null </summary>
        isnull,

        /// <summary> not null </summary>
        notnull,
    }

    public class OpEnumHelper
    {
        /// <summary>转换操作符</summary>
        /// <param name="op"></param>
        /// <returns></returns>
        /// <exception cref="FatalException"></exception>
        public static OpEnum ParaseOp(string op)
        {
            switch (op.ToLower())
            {
                case "=":
                case "eq":
                    return OpEnum.equal;

                case ">":
                case "gt":
                    return OpEnum.greater;

                case ">=":
                case "gte":
                    return OpEnum.greaterorequal;

                case "!=":
                case "ne":
                    return OpEnum.notequal;

                case "<":
                case "lt":
                    return OpEnum.less;

                case "<=":
                case "lte":
                    return OpEnum.lessorequal;

                case "like":
                    return OpEnum.contains;
                case "llike":
                    return OpEnum.startsWith;
                case "rlike":
                    return OpEnum.endsWith;


                case "in":
                    return OpEnum.In;

                case "between":
                    return OpEnum.between;

                case "null":
                case "isnull":
                    return OpEnum.isnull;

                case "notnull":
                case "isnotnull":
                    return OpEnum.notnull;

                default:
                    throw new Exception("操作类型 Op 未定义");
            }
        }

        public static string GetDefaultOp(Type type)
        {
            if (type == typeof(string))
            {
               return "like";
            }
            {
                return "=";
            }
        }

    }
}
