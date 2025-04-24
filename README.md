# 基于JSON 的动态查询描述器
它能够支持任意逻辑条件的组合或嵌套，组合使用可以动态生成Expression<Func<TEntity,bool>>表达式树，供给EFCore 等ORM框架使用，查询条件只需要构造一个Json,无需编写任何代码即可以实现任意的查询。

## JSON数据
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

### 缺省值
+ type: 有值时，只允许填写and或者or，并且忽略其他的filters属性。
+ valueType:  不传递时，尝试将值转换成field在实体定义的类型，转换失败则报错
+ op：没有传递时，会自动根据valueType得到默认的op运算符

| 类型 | 默认值 |
| --- | --- |
| string | like |
| 其他 | = |