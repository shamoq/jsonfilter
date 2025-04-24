using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonFilter
{
    /// <summary>查询条件 And Or 查询 不支持嵌套</summary>
    public class Filter : IEqualityComparer<Filter>
    {
        /// <summary>关系运算符，and or</summary>
        public string Type { get; set; }

        /// <summary>字段名称</summary>
        public string Field { get; set; }

        /// <summary>
        /// 操作符
        /// </summary>
        /// <![CDATA[
        /// like 、 = 、！= 、> 、< 、>= 、<= 、in 、 between 
        /// ]]>
        public string Op { get; set; }

        /// <summary>值</summary>
        public string Value { get; set; }

        /// <summary>
        /// 值的类型
        /// </summary>
        public string ValueType { get; set; }

        /// <summary>
        /// 表达式
        /// </summary>
        public List<Filter> Filters { get; set; }

        public bool Equals(Filter me, Filter other)
        {
            var result = me.Field == other.Field
                && me.Value.ToString() == other.Value.ToString()
                && me.Op == other.Op
                && me.Type == other.Type;
            return result;
        }

        public int GetHashCode(Filter me)
        {
            return me.ToString().GetHashCode();
        }
    }

}
