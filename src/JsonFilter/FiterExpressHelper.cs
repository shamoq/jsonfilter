using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JsonFilter
{
    public static class FiterExpressHelper
    {
        /// <summary>获取条件表达式</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Parse<T>(List<Filter> filters)
        {
            var param = new QueryExpression<T>();
            return param.ParserWhere(filters);
        }
    }
}
