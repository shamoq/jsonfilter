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

        [Test(Description = "����������һ���ԣ����Ǹ��������")]
        public void TestAllOperators()
        {
            // ��������
            var testCases = new[]
            {
                // ���Ե��ڲ���
                new { field = "Id", op = "eq", value = "1", expectedCount = 1 },
                new { field = "Id", op = "=", value = "1", expectedCount = 1 },
                // ���Դ��ڲ���
                new { field = "Age", op = "gt", value = "18", expectedCount = 9 },
                new { field = "Age", op = ">", value = "18", expectedCount = 9 },
                // ���Դ��ڵ��ڲ���
                new { field = "Age", op = "gte", value = "18", expectedCount = 9 },
                new { field = "Age", op = ">=", value = "18", expectedCount = 9 },
                // ���Բ����ڲ���
                new { field = "IsActive", op = "ne", value = "true", expectedCount = 5 },
                new { field = "IsActive", op = "!=", value = "true", expectedCount = 5 },
                // ����С�ڲ���
                new { field = "Age", op = "lt", value = "60", expectedCount = 9 },
                new { field = "Age", op = "<", value = "60", expectedCount = 9 },
                // ����С�ڵ��ڲ���
                new { field = "Age", op = "lte", value = "60", expectedCount = 9 },
                new { field = "Age", op = "<=", value = "60", expectedCount = 9 },
                // ���԰�������
                new { field = "Name", op = "like", value = "John", expectedCount = 1 },
                // ������ĳ��ֵ��ͷ
                new { field = "Name", op = "llike", value = "J", expectedCount = 3 },
                // ������ĳ��ֵ��β
                new { field = "Name", op = "rlike", value = "n", expectedCount = 2 },
                // ���� In ����
                new { field = "Id", op = "in", value = "1,2,3", expectedCount = 3 },
                // ���� Between ����
                new { field = "Age", op = "between", value = "18,30", expectedCount = 5 },
                // ���� IsNull ����
                new { field = "Name", op = "isnull", value = "", expectedCount = 1 },
                new { field = "Name", op = "null", value = "", expectedCount = 1 },
                // ���� IsNotNull ����
                new { field = "Name", op = "isnotnull", value = "", expectedCount = 9 },
                new { field = "Name", op = "notnull", value = "", expectedCount = 9 }
            };

            // ģ������
            var list = GetList();

            foreach (var caseData in testCases)
            {
                // �������������л�Ϊ JSON
                var json = JsonConvert.SerializeObject(caseData);

                // �����л�Ϊ Filter �б�
                var filter = JsonConvert.DeserializeObject<Filter>(json);

                // �������ʽ
                var express = FiterExpressHelper.Parse<User>(new List<Filter>() { filter });

                Console.Write(express);

                // ִ�в�ѯ
                var result = list.AsQueryable().Where(express).ToList();

                // �����ѯ���
                Console.WriteLine($"  ��ѯ�����{result.Count}");

                // ����ʵ�ʽ���Ƿ����Ԥ��
                Assert.AreEqual(caseData.expectedCount, result.Count, $"��������ʧ��: {caseData.field} {caseData.op} {caseData.value}");
            }
        }

        /// <summary>
        /// ����������Ϻ�Ƕ��
        /// </summary>
        [Test(Description = "����������Ϻ�Ƕ��")]
        public void TestCombinedAndNestedConditions()
        {
            // ��������
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

            // ģ������
            var list = GetList();

            for (int i = 0; i < testCases.Length; i++)
            {
                var caseData = testCases[i];

                // �������ʽ
                var express = FiterExpressHelper.Parse<User>(new List<Filter> { caseData.filter });

                Console.Write(express);

                // ִ�в�ѯ
                var result = list.AsQueryable().Where(express).ToList();

                // �����ѯ���
                Console.WriteLine($"  ��ѯ�����{result.Count}");

                // ����ʵ�ʽ���Ƿ����Ԥ��
                Assert.AreEqual(caseData.expectedCount, result.Count, $"��������ʧ��{i}: {JsonConvert.SerializeObject(caseData.filter)}");
            }
        }


        [Test(Description = "�������ͺ�������������")]
        public void TestAllTypeOperators()
        {
            var list = GetList();
            var testCases = new[]
            {
        // �ַ������Ͳ���
        new { field = "Name", op = "eq", value = "John", expectedCount = 1 },
        new { field = "Name", op = "ne", value = "John", expectedCount = 9 },
        new { field = "Name", op = "like", value = "J", expectedCount = 3 },
        new { field = "Name", op = "llike", value = "J", expectedCount = 3 },
        new { field = "Name", op = "rlike", value = "n", expectedCount = 1 },
        new { field = "Name", op = "isnull", value = "", expectedCount = 1 },
        new { field = "Name", op = "notnull", value = "", expectedCount = 9 },

        // �������Ͳ���
        new { field = "Id", op = "eq", value = "1", expectedCount = 1,  },
        new { field = "Id", op = "gt", value = "5", expectedCount = 5,  },
        new { field = "Id", op = "gte", value = "5", expectedCount = 6,  },
        new { field = "Id", op = "lt", value = "5", expectedCount = 4,  },
        new { field = "Id", op = "lte", value = "5", expectedCount = 5,  },
        new { field = "Id", op = "ne", value = "1", expectedCount = 9,  },
        new { field = "Id", op = "in", value = "1,2,3", expectedCount = 3,  },

        // �������Ͳ���
        new { field = "BirthDate", op = "eq", value = "1990-01-01", expectedCount = 1,  },
        new { field = "BirthDate", op = "gt", value = "1995-01-01", expectedCount = 4,  },
        new { field = "BirthDate", op = "gte", value = "1995-01-01", expectedCount = 4,  },
        new { field = "BirthDate", op = "lt", value = "1995-01-01", expectedCount = 6,  },
        new { field = "BirthDate", op = "lte", value = "1995-01-01", expectedCount = 6,  },
        new { field = "BirthDate", op = "ne", value = "1990-01-01", expectedCount = 9,  },
        new { field = "BirthDate", op = "between", value = "1990-01-01,1995-01-01", expectedCount = 2,  },

        // ö�����Ͳ���
        new { field = "Sex", op = "eq", value = SexEnum.Boy.ToString(), expectedCount = 5,  },
        new { field = "Sex", op = "eq", value = "0", expectedCount = 5,  },
        new { field = "Sex", op = "ne", value = SexEnum.Boy.ToString(), expectedCount = 5,  },

        // ���� User ������� decimal ���͵� Salary �ֶκ� Guid ���͵� UserGuid �ֶ�
        // С�����Ͳ���
        new { field = "Salary", op = "eq", value = "5000.00", expectedCount = 1, },
        new { field = "Salary", op = "gt", value = "4000.00", expectedCount = 6, },
        new { field = "Salary", op = "gte", value = "4000.00", expectedCount = 7, },
        new { field = "Salary", op = "lt", value = "6000.00", expectedCount = 5, },
        new { field = "Salary", op = "lte", value = "6000.00", expectedCount = 6, },
        new { field = "Salary", op = "ne", value = "5000.00", expectedCount = 9, },
        new { field = "Salary", op = "between", value = "4000.00,6000.00", expectedCount = 3, },

        // Guid ���Ͳ���
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
                Console.WriteLine($"  ��ѯ�����{result.Count}");
                Assert.AreEqual(caseData.expectedCount, result.Count, $"��������ʧ��{i}: {caseData.field} {caseData.op} {caseData.value}");
            }
        }
    }
}