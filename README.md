# JsonFilter：一款强大的动态查询表达式引擎

在.NET开发中，我们经常需要根据用户输入的查询条件动态构建查询表达式。本文将深入介绍一个优雅的解决方案 - JsonFilter，它能让你通过简单的JSON配置，轻松实现复杂的查询逻辑。

## 核心特性

1. 支持丰富的操作符
   - 比较运算：等于(eq/=)、大于(gt/>)、小于(lt/<)等
   - 字符串匹配：like、左匹配(llike)、右匹配(rlike)
   - 空值判断：isnull/null、isnotnull/notnull
   - 范围查询：in、between

2. 支持多种数据类型
   - 字符串(string)
   - 数值(int/decimal)
   - 日期时间(DateTime)
   - 枚举(Enum)
   - GUID

3. 灵活的条件组合
   - 支持AND/OR逻辑组合
   - 支持多层嵌套条件

## 实现原理

### 1. 表达式树动态构建

JsonFilter的核心是通过Expression<Func<T, bool>>动态构建Lambda表达式。这种方式比字符串拼接SQL更安全、更优雅。

示例JSON格式：
```json
{
    "field": "Age",
    "op": "gt",
    "value": "18"
}
```

### 2. 类型转换处理

系统会根据属性的实际类型自动进行类型转换：

```csharp
// 示例：处理不同类型的查询条件
var testCases = new[]
{
    // 字符串类型
    new { field = "Name", op = "like", value = "J" },
    // 数值类型
    new { field = "Age", op = "gt", value = "18" },
    // 日期类型
    new { field = "BirthDate", op = "between", value = "1990-01-01,1995-01-01" },
    // 枚举类型
    new { field = "Sex", op = "eq", value = "Boy" }
}
```

### 3. 复杂条件组合

支持通过Type属性指定条件组合方式：

```json
{
    "type": "and",
    "filters": [
        { "field": "Age", "op": ">=", "value": "18" },
        { "field": "IsActive", "op": "=", "value": "true" }
    ]
}
```

## 性能优化

1. 表达式树缓存
   - 对常用查询条件的表达式树进行缓存
   - 避免重复构建相同的表达式

2. 参数化查询
   - 所有查询条件都通过参数化方式处理
   - 防止SQL注入风险

## 使用场景

1. 前端动态查询
   - 配合前端查询界面，实现灵活的查询功能
   - 支持多条件组合筛选

2. API查询接口
   - 提供统一的查询接口
   - 支持复杂的查询条件组合

3. 报表查询
   - 动态构建报表查询条件
   - 支持多维度数据筛选

## 最佳实践

1. 查询条件定义
```csharp
public class Filter
{
    public string Field { get; set; }    // 查询字段
    public string Op { get; set; }       // 操作符
    public string Value { get; set; }    // 查询值
    public string Type { get; set; }     // 组合类型(and/or)
    public List<Filter> Filters { get; set; } // 子条件
}
```

2. 使用示例
```csharp
// 构建查询条件
var filter = new Filter
{
    Field = "Age",
    Op = "gt",
    Value = "18"
};

// 生成表达式
var express = FiterExpressHelper.Parse<User>(new List<Filter> { filter });

// 执行查询
var result = list.AsQueryable().Where(express).ToList();
```

## 更多JSON示例
### 单条件示例
```plain
[
    // Id = 1
    [
        {
            "field": "Id",
            "value": "1",
            "op": "="
        }
    ],
    // Name like 'John'
    [
        {
            "field": "Name",
            "value": "John",
            "op": "like"
        }
    ],
    // IsActive = true
    [
        {
            "field": "IsActive",
            "value": "true",
            "op": "="
        }
    ],
    // BirthDate < '2000-01-01'
    [
        {
            "field": "BirthDate",
            "value": "2000-01-01",
            "op": "<"
        }
    ],
    // Height >= 170.5
    [
        {
            "field": "Height",
            "value": "170.5",
            "op": ">="
        }
    ],
    // Sex = 1
    [
        {
            "field": "Sex",
            "value": "1",
            "op": "="
        }
    ],
    // Age is not null
    [
        {
            "field": "Age",
            "value": "null",
            "op": "!="
        }
    ],
    // Age between 18 and 30
    [
        {
            "field": "Age",
            "value": "18",
            "op": ">="
        },
        {
            "field": "Age",
            "value": "30",
            "op": "<="
        }
    ],
    // Id in (1, 2, 3)
    [
        {
            "field": "Id",
            "value": "1,2,3",
            "op": "in"
        }
    ],
    // Name != 'Alice' and IsActive = true
    [
        {
            "type": "and",
            "filters": [
                {
                    "field": "Name",
                    "value": "Alice",
                    "op": "!="
                },
                {
                    "field": "IsActive",
                    "value": "true",
                    "op": "="
                }
            ]
        }
    ]
]
```

### 嵌套数据示例
```plain
[
    // (Id = 1 or Id = 2) and (Name like 'John' or Name like 'Doe')
    [
        {
            "type": "and",
            "filters": [
                {
                    "type": "or",
                    "filters": [
                        {
                            "field": "Id",
                            "value": "1",
                            "op": "="
                        },
                        {
                            "field": "Id",
                            "value": "2",
                            "op": "="
                        }
                    ]
                },
                {
                    "type": "or",
                    "filters": [
                        {
                            "field": "Name",
                            "value": "John",
                            "op": "like"
                        },
                        {
                            "field": "Name",
                            "value": "Doe",
                            "op": "like"
                        }
                    ]
                }
            ]
        }
    ],
    // (IsActive = true and Age >= 18) or (IsActive = false and Age < 18)
    [
        {
            "type": "or",
            "filters": [
                {
                    "type": "and",
                    "filters": [
                        {
                            "field": "IsActive",
                            "value": "true",
                            "op": "="
                        },
                        {
                            "field": "Age",
                            "value": "18",
                            "op": ">="
                        }
                    ]
                },
                {
                    "type": "and",
                    "filters": [
                        {
                            "field": "IsActive",
                            "value": "false",
                            "op": "="
                        },
                        {
                            "field": "Age",
                            "value": "18",
                            "op": "<"
                        }
                    ]
                }
            ]
        }
    ],
    // (BirthDate < '2000-01-01' and Height >= 170.5) or Sex = 1
    [
        {
            "type": "or",
            "filters": [
                {
                    "type": "and",
                    "filters": [
                        {
                            "field": "BirthDate",
                            "value": "2000-01-01",
                            "op": "<"
                        },
                        {
                            "field": "Height",
                            "value": "170.5",
                            "op": ">="
                        }
                    ]
                },
                {
                    "field": "Sex",
                    "value": "1",
                    "op": "="
                }
            ]
        }
    ]
]
```

## 数据结构说明
### 定义
```plain
// 定义 Filter 类型
interface Filter {
    Type?: string;  // 关系运算符
    Field: string;  // 字段名称
    Op: string;   // 运算符类型
    Value: any;  // 条件值
    ValueType: ValueType; // 条件值数据类型
    Filters?: Filter[]; // 嵌套条件
}    

// 定义 C# 常见数据类型枚举
enum ValueType {
    Int = 'int',
    String = 'string',
    Bool = 'bool',
    DateTime = 'DateTime',
    Double = 'double',
    Enum = 'enum',
    Byte = 'byte',
    Decimal = 'decimal',
    Float = 'float',
    BigInt = 'long', // C# 里 bigint 对应 long 类型
    Short = 'short',
    UByte = 'byte', // 无符号 byte，C# 里是 byte 本身（它是无符号的）
    UShort = 'ushort',
    UInt = 'uint',
    ULong = 'ulong',
    Char = 'char',
    TimeSpan = 'TimeSpan',
    Guid = 'Guid'
}
```

### 属性缺省值
+ type: 有值时，只允许填写and或者or，并且忽略其他的filters属性。
+ valueType:  不传递时，尝试将值转换成field在实体定义的类型，转换失败则报错
+ op：没有传递时，会自动根据valueType得到默认的op运算符

| 类型 | 默认值 |
| --- | --- |
| string | like |
| 其他 | = |


## 总结

JsonFilter通过优雅的设计，解决了动态查询的复杂性问题。它不仅提供了丰富的查询功能，还保证了类型安全和查询性能。无论是简单的单条件查询，还是复杂的多条件组合，都能轻松应对。

这个框架的实现充分展示了.NET中表达式树的强大功能，也为我们在设计类似功能时提供了很好的参考。通过学习其源码，你会对.NET的表达式树、反射、泛型等特性有更深入的理解。

希望这篇文章能帮助你更好地理解和使用JsonFilter，也欢迎在实践中不断探索和改进这个框架。