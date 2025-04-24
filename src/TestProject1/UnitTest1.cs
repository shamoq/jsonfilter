using JsonFilter;
using Newtonsoft.Json;
using TestProject1.TestCase;

namespace TestProject1
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }


        List<User> GetList()
        {
            var list = new List<User>
            {
                new User { Id = 1, Name = "John", IsActive = true, BirthDate = new DateTime(1990, 1, 1), Height = 180.5, Sex = SexEnum.Boy, Age = 25, Salary = 1000, UserGUID= new Guid("00000000-0000-0000-0000-000000000001") },
                new User { Id = 2, Name = "Jane", IsActive = false, BirthDate = new DateTime(1995, 5, 15), Height = 165.3, Sex = SexEnum.Boy, Age = 30, Salary = 2000, UserGUID= new Guid("00000000-0000-0000-0000-000000000002")  },
                new User { Id = 3, Name = "Jack", IsActive = true, BirthDate = new DateTime(1985, 3, 20), Height = 175.0, Sex = SexEnum.Gril, Age = 35 , Salary = 3000, UserGUID= new Guid("00000000-0000-0000-0000-000000000003") },
                new User { Id = 4, Name = null, IsActive = false, BirthDate = new DateTime(2000, 7, 10), Height = 160.0, Sex = SexEnum.Gril, Age = null, Salary = 4000, UserGUID= new Guid("00000000-0000-0000-0000-000000000004")  },
                new User { Id = 5, Name = "Alice", IsActive = true, BirthDate = new DateTime(1992, 8, 25), Height = 170.2, Sex = SexEnum.Gril, Age = 28 , Salary = 5000, UserGUID= new Guid("00000000-0000-0000-0000-000000000005") },
                new User { Id = 6, Name = "Bob", IsActive = false, BirthDate = new DateTime(1980, 12, 5), Height = 185.0, Sex = SexEnum.Boy, Age = 42, Salary = 6000, UserGUID= new Guid("00000000-0000-0000-0000-000000000006")  },
                new User { Id = 7, Name = "Charlie", IsActive = true, BirthDate = new DateTime(2001, 3, 15), Height = 178.4, Sex = SexEnum.Boy, Age = 22, Salary = 7000, UserGUID= new Guid("00000000-0000-0000-0000-000000000007")  },
                new User { Id = 8, Name = "Diana", IsActive = false, BirthDate = new DateTime(1998, 6, 10), Height = 162.5, Sex = SexEnum.Gril, Age = 25, Salary = 8000, UserGUID= new Guid("00000000-0000-0000-0000-000000000008")  },
                new User { Id = 9, Name = "Eve", IsActive = true, BirthDate = new DateTime(1987, 11, 30), Height = 168.0, Sex = SexEnum.Gril, Age = 35 , Salary = 9000, UserGUID= new Guid("00000000-0000-0000-0000-000000000009") },
                new User { Id = 10, Name = "Frank", IsActive = false, BirthDate = new DateTime(1975, 4, 20), Height = 190.0, Sex = SexEnum.Boy, Age = 48, Salary = 10000, UserGUID= new Guid("00000000-0000-0000-0000-00000000000A")  }
            };

            return list;
        }

        [Test(Description = "单个条件逐一测试，覆盖更多运算符")]
        public void TestAllOperators()
        {
            // 测试数据
            var testCases = new[]
            {
                // 测试等于操作
                new { field = "Id", op = "eq", value = "1", expectedCount = 1 },
                new { field = "Id", op = "=", value = "1", expectedCount = 1 },
                // 测试大于操作
                new { field = "Age", op = "gt", value = "18", expectedCount = 9 },
                new { field = "Age", op = ">", value = "18", expectedCount = 9 },
                // 测试大于等于操作
                new { field = "Age", op = "gte", value = "18", expectedCount = 9 },
                new { field = "Age", op = ">=", value = "18", expectedCount = 9 },
                // 测试不等于操作
                new { field = "IsActive", op = "ne", value = "true", expectedCount = 5 },
                new { field = "IsActive", op = "!=", value = "true", expectedCount = 5 },
                // 测试小于操作
                new { field = "Age", op = "lt", value = "60", expectedCount = 9 },
                new { field = "Age", op = "<", value = "60", expectedCount = 9 },
                // 测试小于等于操作
                new { field = "Age", op = "lte", value = "60", expectedCount = 9 },
                new { field = "Age", op = "<=", value = "60", expectedCount = 9 },
                // 测试包含操作
                new { field = "Name", op = "like", value = "John", expectedCount = 1 },
                // 测试以某个值开头
                new { field = "Name", op = "llike", value = "J", expectedCount = 3 },
                // 测试以某个值结尾
                new { field = "Name", op = "rlike", value = "n", expectedCount = 2 },
                // 测试 In 操作
                new { field = "Id", op = "in", value = "1,2,3", expectedCount = 3 },
                // 测试 Between 操作
                new { field = "Age", op = "between", value = "18,30", expectedCount = 5 },
                // 测试 IsNull 操作
                new { field = "Name", op = "isnull", value = "", expectedCount = 1 },
                new { field = "Name", op = "null", value = "", expectedCount = 1 },
                // 测试 IsNotNull 操作
                new { field = "Name", op = "isnotnull", value = "", expectedCount = 9 },
                new { field = "Name", op = "notnull", value = "", expectedCount = 9 }
            };

            // 模拟数据
            var list = GetList();

            foreach (var caseData in testCases)
            {
                // 将测试数据序列化为 JSON
                var json = JsonConvert.SerializeObject(caseData);

                // 反序列化为 Filter 列表
                var filter = JsonConvert.DeserializeObject<Filter>(json);

                // 解析表达式
                var express = FiterExpressHelper.Parse<User>(new List<Filter>() { filter });

                Console.Write(express);

                // 执行查询
                var result = list.AsQueryable().Where(express).ToList();

                // 输出查询结果
                Console.WriteLine($"  查询结果：{result.Count}");

                // 断言实际结果是否符合预期
                Assert.AreEqual(caseData.expectedCount, result.Count, $"测试用例失败: {caseData.field} {caseData.op} {caseData.value}");
            }
        }

        /// <summary>
        /// 测试条件组合和嵌套
        /// </summary>
        [Test(Description = "测试条件组合和嵌套")]
        public void TestCombinedAndNestedConditions()
        {
            // 测试数据
            var testCases = new[]
            {
        // (Age >= 18 and IsActive = true)
        new {
            filter = new Filter
            {
                Type = "and",
                Filters = new List<Filter>
                {
                    new Filter { Field = "Age", Op = ">=", Value = "18" },
                    new Filter { Field = "IsActive", Op = "=", Value = "true" }
                }
            },
            expectedCount = 5
        },
        // (Age < 30 or Name like 'J')
        new {
            filter = new Filter
            {
                Type = "or",
                Filters = new List<Filter>
                {
                    new Filter { Field = "Age", Op = "<", Value = "30" },
                    new Filter { Field = "Name", Op = "like", Value = "J" }
                }
            },
            expectedCount = 6
        },
        // ((Age >= 20 and Age <= 35) or (Name startsWith 'A')) and IsActive = true
        new {
            filter = new Filter
            {
                Type = "and",
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        Type = "or",
                        Filters = new List<Filter>
                        {
                            new Filter
                            {
                                Type = "and",
                                Filters = new List<Filter>
                                {
                                    new Filter { Field = "Age", Op = ">=", Value = "20" },
                                    new Filter { Field = "Age", Op = "<=", Value = "35" }
                                }
                            },
                            new Filter { Field = "Name", Op = "llike", Value = "A" }
                        }
                    },
                    new Filter { Field = "IsActive", Op = "=", Value = "true" }
                }
            },
            expectedCount = 5
        }
    };

            // 模拟数据
            var list = GetList();

            for (int i = 0; i < testCases.Length; i++)
            {
                var caseData = testCases[i];

                // 解析表达式
                var express = FiterExpressHelper.Parse<User>(new List<Filter> { caseData.filter });

                Console.Write(express);

                // 执行查询
                var result = list.AsQueryable().Where(express).ToList();

                // 输出查询结果
                Console.WriteLine($"  查询结果：{result.Count}");

                // 断言实际结果是否符合预期
                Assert.AreEqual(caseData.expectedCount, result.Count, $"测试用例失败{i}: {JsonConvert.SerializeObject(caseData.filter)}");
            }
        }


        [Test(Description = "所有类型和所有运算符组合")]
        public void TestAllTypeOperators()
        {
            var list = GetList();
            var testCases = new[]
            {
        // 字符串类型测试
        new { field = "Name", op = "eq", value = "John", expectedCount = 1 },
        new { field = "Name", op = "ne", value = "John", expectedCount = 9 },
        new { field = "Name", op = "like", value = "J", expectedCount = 3 },
        new { field = "Name", op = "llike", value = "J", expectedCount = 3 },
        new { field = "Name", op = "rlike", value = "n", expectedCount = 1 },
        new { field = "Name", op = "isnull", value = "", expectedCount = 1 },
        new { field = "Name", op = "notnull", value = "", expectedCount = 9 },

        // 整数类型测试
        new { field = "Id", op = "eq", value = "1", expectedCount = 1,  },
        new { field = "Id", op = "gt", value = "5", expectedCount = 5,  },
        new { field = "Id", op = "gte", value = "5", expectedCount = 6,  },
        new { field = "Id", op = "lt", value = "5", expectedCount = 4,  },
        new { field = "Id", op = "lte", value = "5", expectedCount = 5,  },
        new { field = "Id", op = "ne", value = "1", expectedCount = 9,  },
        new { field = "Id", op = "in", value = "1,2,3", expectedCount = 3,  },

        // 日期类型测试
        new { field = "BirthDate", op = "eq", value = "1990-01-01", expectedCount = 1,  },
        new { field = "BirthDate", op = "gt", value = "1995-01-01", expectedCount = 4,  },
        new { field = "BirthDate", op = "gte", value = "1995-01-01", expectedCount = 4,  },
        new { field = "BirthDate", op = "lt", value = "1995-01-01", expectedCount = 6,  },
        new { field = "BirthDate", op = "lte", value = "1995-01-01", expectedCount = 6,  },
        new { field = "BirthDate", op = "ne", value = "1990-01-01", expectedCount = 9,  },
        new { field = "BirthDate", op = "between", value = "1990-01-01,1995-01-01", expectedCount = 2,  },

        // 枚举类型测试
        new { field = "Sex", op = "eq", value = SexEnum.Boy.ToString(), expectedCount = 5,  },
        new { field = "Sex", op = "eq", value = "0", expectedCount = 5,  },
        new { field = "Sex", op = "ne", value = SexEnum.Boy.ToString(), expectedCount = 5,  },

        // 假设 User 类添加了 decimal 类型的 Salary 字段和 Guid 类型的 UserGuid 字段
        // 小数类型测试
        new { field = "Salary", op = "eq", value = "5000.00", expectedCount = 1, },
        new { field = "Salary", op = "gt", value = "4000.00", expectedCount = 6, },
        new { field = "Salary", op = "gte", value = "4000.00", expectedCount = 7, },
        new { field = "Salary", op = "lt", value = "6000.00", expectedCount = 5, },
        new { field = "Salary", op = "lte", value = "6000.00", expectedCount = 6, },
        new { field = "Salary", op = "ne", value = "5000.00", expectedCount = 9, },
        new { field = "Salary", op = "between", value = "4000.00,6000.00", expectedCount = 3, },

        // Guid 类型测试
        new { field = "UserGuid", op = "eq", value = Guid.NewGuid().ToString(), expectedCount = 0, },
        new { field = "UserGuid", op = "ne", value = Guid.NewGuid().ToString(), expectedCount = 10, },
        new { field = "UserGuid", op = "eq", value = "00000000-0000-0000-0000-000000000001", expectedCount = 1, },
        new { field = "UserGuid", op = "ne", value = "00000000-0000-0000-0000-000000000001", expectedCount = 9, },
        new { field = "UserGuid", op = "in", value = "00000000-0000-0000-0000-000000000001,00000000-0000-0000-0000-000000000002,00000000-0000-0000-0000-000000000003", expectedCount = 3, }
    };

            for (int i = 0; i < testCases.Length; i++)
            {
                var caseData = testCases[i];
                var json = JsonConvert.SerializeObject(new { Field = caseData.field, Op = caseData.op, Value = caseData.value });
                var filter = JsonConvert.DeserializeObject<Filter>(json);
                var express = FiterExpressHelper.Parse<User>(new List<Filter> { filter });
                Console.Write(express);
                var result = list.AsQueryable().Where(express).ToList();
                Console.WriteLine($"  查询结果：{result.Count}");
                Assert.AreEqual(caseData.expectedCount, result.Count, $"测试用例失败{i}: {caseData.field} {caseData.op} {caseData.value}");
            }
        }
    }
}