using JsonFilter.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JsonFilter
{
    internal class QueryExpression<T>
    {
        private ParameterExpression parameter;

        public QueryExpression()
        {
            parameter = Expression.Parameter(typeof(T));
        }

        private Expression ParseExpressionBody(List<Filter> conditions, string type = "or")
        {
            if (conditions == null || conditions.Count() == 0)
            {
                return Expression.Constant(true, typeof(bool));
            }

            Expression lasestExpress = null;
            foreach (var condition in conditions)
            {
                Expression express = null;
                // 有嵌套条件，传递了type时，则忽略其他字段值
                if (!string.IsNullOrEmpty(condition.Type))
                {
                    if (!condition.Type.Equals("and", StringComparison.OrdinalIgnoreCase) && !condition.Type.Equals("or", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new Exception($"{condition.Type}传值不正确，期望值范围是and,or");
                    }
                    if (condition.Filters != null && condition.Filters.Any())
                    {
                        express = ParseExpressionBody(condition.Filters, condition.Type);
                        if (express == null)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        // 嵌套条件下又没有任何条件，不能拼接
                        continue;
                    }
                }
                else if (!string.IsNullOrEmpty(condition.Field)) // 传递了字段，则是字段过滤
                {
                    express = ParseCondition(condition);
                }
                else
                {
                    continue;
                }

                // 表达式拼接
                if (lasestExpress is null)
                {
                    lasestExpress = express;
                }
                else
                {
                    lasestExpress = type == "and" ? Expression.And(lasestExpress, express) : Expression.Or(lasestExpress, express);
                }
            }

            // var expresses = new List<FiterExpress>();
            //conditions.ForEach(condition =>
            //{
            //    var filter = new FiterExpress
            //    {
            //        field = condition.field,
            //        isAnd = condition.isAnd,
            //        op = condition.op,
            //        value = condition.value
            //    };
            //    var express = ParseCondition(filter);
            //    filter.Expression = express;
            //    expresses.Add(filter);
            //});

            //Expression lasestExpress = null;
            //for (int i = 1; i < expresses.Count; i++)
            //{
            //    if (lasestExpress is null)
            //        lasestExpress = expresses[i - 1].Expression;

            //    var now = expresses[i];
            //    lasestExpress = now.isAnd ? Expression.And(lasestExpress, now.Expression)
            //        : Expression.Or(lasestExpress, now.Expression);
            //}
            return lasestExpress;
        }

        private Expression ParseCondition(Filter condition)
        {
            ParameterExpression p = parameter;
            Expression key = null;

            try
            {
                key = Expression.Property(p, StringHelper.ToCamelCase(condition.Field));
            }
            catch (Exception ex)
            {
                throw new Exception($"模型{p.Type}中不存在字段{condition.Field}", ex);
            }

            // 条件类型
            var keyType = StringToType(condition.ValueType) ?? key.Type;

            // 条件运算符默认值
            if (string.IsNullOrEmpty(condition.Op))
            {
                condition.Op = OpEnumHelper.GetDefaultOp(keyType);
            }

            var opEnum = OpEnumHelper.ParaseOp(condition.Op);

            object convertValue = condition.Value;
            if (opEnum != OpEnum.In || opEnum != OpEnum.between)
            {
                convertValue = TypeConvertHelper.ConvertType(condition.Value, keyType);
            }
            Expression value = Expression.Constant(convertValue);

            switch (opEnum)
            {
                case OpEnum.startsWith:
                    // Expression stringValue = Expression.Constant(convertValue.ToString());
                    var notNullCheckStartsWithCall = Expression.NotEqual(key, Expression.Constant(null, key.Type));
                    var startsWithCall = Expression.Call(key, typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), value);
                    return Expression.AndAlso(notNullCheckStartsWithCall, startsWithCall);

                case OpEnum.endsWith:
                    // Expression stringValue = Expression.Constant(convertValue.ToString());
                    var notNullCheckEndsWithCall = Expression.NotEqual(key, Expression.Constant(null, key.Type));
                    var endsWithCall = Expression.Call(key, typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }), value);
                    return Expression.AndAlso(notNullCheckEndsWithCall, endsWithCall);

                case OpEnum.contains:
                    // Expression stringValue = Expression.Constant(convertValue.ToString());
                    var notNullCheckContainsCall = Expression.NotEqual(key, Expression.Constant(null, key.Type));
                    var containsCall = Expression.Call(key, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), value);
                    return Expression.AndAlso(notNullCheckContainsCall, containsCall);

                case OpEnum.equal:
                    return Expression.Equal(key, Expression.Convert(value, key.Type));

                case OpEnum.greater:
                    return Expression.GreaterThan(key, Expression.Convert(value, key.Type));

                case OpEnum.greaterorequal:
                    return Expression.GreaterThanOrEqual(key, Expression.Convert(value, key.Type));

                case OpEnum.less:
                    return Expression.LessThan(key, Expression.Convert(value, key.Type));

                case OpEnum.lessorequal:
                    return Expression.LessThanOrEqual(key, Expression.Convert(value, key.Type));

                case OpEnum.notequal:
                    return Expression.NotEqual(key, Expression.Convert(value, key.Type));

                case OpEnum.isnull:
                    return Expression.Equal(key, Expression.Constant(null, key.Type));

                case OpEnum.notnull:
                    return Expression.NotEqual(key, Expression.Constant(null, key.Type));

                case OpEnum.In:
                    return ParaseIn(condition, keyType);

                case OpEnum.between:
                    return ParaseBetween(condition, keyType);

                default:
                    throw new NotImplementedException("不支持此操作");
            }
        }

        /// <summary>Between 转换</summary>
        /// <param name="conditions"></param>
        /// <param name="keyType"></param>
        /// <returns></returns>
        private Expression ParaseBetween(Filter conditions, Type keyType)
        {
            ParameterExpression p = parameter;
            Expression key = Expression.Property(p, StringHelper.ToCamelCase(conditions.Field));

            // 将值分割为起始和结束值
            var value = conditions.Value.ToString();
            var valueArr = value.Split(',');

            if (valueArr.Length != 2)
            {
                throw new ArgumentException("ParaseBetween参数错误，必须包含起始值和结束值");
            }

            // 转换起始值和结束值为字段类型
            object startValue;
            object endValue;
            try
            {
                startValue = TypeConvertHelper.ConvertType(valueArr[0], keyType);
                endValue = TypeConvertHelper.ConvertType(valueArr[1], keyType);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("ParaseBetween参数转换失败，请检查值的类型是否与字段类型匹配", ex);
            }

            // 构建表达式
            Expression start = Expression.GreaterThanOrEqual(key, Expression.Constant(startValue, keyType));
            Expression end = Expression.LessThanOrEqual(key, Expression.Constant(endValue, keyType));

            return Expression.AndAlso(start, end);
        }

        /// <summary>In 转换 多个值之间 ， 间隔</summary>
        /// <param name="conditions"></param>
        /// <param name="keyType"></param>
        /// <returns></returns>
        private Expression ParaseIn(Filter conditions, Type keyType)
        {
            ParameterExpression p = parameter;
            var fieldvalue = conditions.Value.ToString();
            Expression key = Expression.Property(p, StringHelper.ToCamelCase(conditions.Field));

            // 将值分割并转换为目标类型的列表
            var valueArr = fieldvalue.Split(',');

            var typeValueArr = Array.CreateInstance(keyType, valueArr.Length);

            for (int i = 0; i < valueArr.Length; i++)
            {
                var convertedValue = TypeConvertHelper.ConvertType(valueArr[i], keyType);
                typeValueArr.SetValue(convertedValue, i);
            }
            // 构建常量表达式表示数组
            var arrayExpression = Expression.Constant(typeValueArr);

            // 调用 Enumerable.Contains 方法
            var containsMethod = typeof(Enumerable)
                .GetMethods()
                .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                .MakeGenericMethod(keyType);

            // 生成 key 在 typeValueArr 中的表达式
            return Expression.Call(containsMethod, arrayExpression, key);
            // typeValueArr 包含key
            // Expression expression = Expression.Constant(true, typeof(bool));
            //foreach (var itemVal in valueArr)
            //{
            //    var itemTypeVal = TypeConvertHelper.ConvertType(itemVal, keyType);
            //    Expression value = Expression.Constant(itemTypeVal);
            //    Expression right = Expression.Equal(key, Expression.Convert(value, keyType));
            //    expression = Expression.Or(expression, right);
            //}
            //return expression;

        }


        public static Type StringToType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }
            switch (typeName.ToLower())
            {
                case "int":
                    return typeof(int);
                case "string":
                    return typeof(string);
                case "bool":
                    return typeof(bool);
                case "datetime":
                    return typeof(DateTime);
                case "double":
                    return typeof(double);
                case "enum":
                    // 枚举是抽象概念，不能直接返回一个具体类型，这里简单返回 object 表示可用于后续枚举类型处理
                    return typeof(object);
                case "byte":
                    return typeof(byte);
                case "decimal":
                    return typeof(decimal);
                case "float":
                    return typeof(float);
                case "long":
                    return typeof(long);
                case "short":
                    return typeof(short);
                case "ushort":
                    return typeof(ushort);
                case "uint":
                    return typeof(uint);
                case "ulong":
                    return typeof(ulong);
                case "char":
                    return typeof(char);
                case "timespan":
                    return typeof(TimeSpan);
                case "guid":
                    return typeof(Guid);
                default:
                    throw new ArgumentException($"不支持的类型名称: {typeName}");
            }
        }


        /// <summary>根据Filter获取查询表达式</summary>
        /// <returns></returns>
        public Expression<Func<T, bool>> ParserWhere(List<Filter> filters)
        {
            //将条件转化成表达是的Body
            var query = ParseExpressionBody(filters);
            if (query == null)
            {
                return null;
            }

            return Expression.Lambda<Func<T, bool>>(query, parameter);
        }
    }
}
