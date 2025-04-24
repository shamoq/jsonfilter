using JsonFilter;
using Newtonsoft.Json;
using Test.Model;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TestFilters();
            Console.ReadKey();
        }

        /// <summary>
        /// 测试所有运算符
        /// </summary>
        static void TestFilters()
        {
            // 测试数据
            var testCases = new[]
            {
                // 测试等于操作
                new { field = "Id", op = "=", value = "1" },
                // 测试大于操作
                new { field = "Age", op = ">=", value = "18" },
                // 测试小于操作
                new { field = "Age", op = "<=", value = "60" },
                // 测试包含操作
                new { field = "Name", op = "like", value = "John" },
                // 测试以某个值开头
                new { field = "Name", op = "llike", value = "J" },
                // 测试以某个值结尾
                new { field = "Name", op = "rlike", value = "n" },
                // 测试不等于操作
                new { field = "IsActive", op = "!=", value = "true" },
                // 测试大于等于操作
                new { field = "Age", op = ">", value = "18" },
                // 测试小于等于操作
                new { field = "Age", op = "<", value = "60" },
                // 测试 In 操作
                new { field = "Id", op = "in", value = "1,2,3" },
                // 测试 Between 操作
                new { field = "Age", op = "between", value = "18,30" },
                // 测试 IsNull 操作
                new { field = "Name", op = "isnull", value = "" },
                // 测试 IsNotNull 操作
                new { field = "Name", op = "notnull", value = "" }
            };

            // 模拟数据
            var list = new List<User>
            {
                new User { Id = 1, Name = "John", IsActive = true, BirthDate = new DateTime(1990, 1, 1), Height = 180.5, Sex = SexEnum.Boy, Age = 25 },
                new User { Id = 2, Name = "Jane", IsActive = false, BirthDate = new DateTime(1995, 5, 15), Height = 165.3, Sex = SexEnum.Boy, Age = 30 },
                new User { Id = 3, Name = "Jack", IsActive = true, BirthDate = new DateTime(1985, 3, 20), Height = 175.0, Sex = SexEnum.Gril, Age = 35 },
                new User { Id = 4, Name = null, IsActive = false, BirthDate = new DateTime(2000, 7, 10), Height = 160.0, Sex = SexEnum.Gril, Age = null },
                new User { Id = 5, Name = "Alice", IsActive = true, BirthDate = new DateTime(1992, 8, 25), Height = 170.2, Sex = SexEnum.Gril, Age = 28 },
                new User { Id = 6, Name = "Bob", IsActive = false, BirthDate = new DateTime(1980, 12, 5), Height = 185.0, Sex = SexEnum.Boy, Age = 42 },
                new User { Id = 7, Name = "Charlie", IsActive = true, BirthDate = new DateTime(2001, 3, 15), Height = 178.4, Sex = SexEnum.Boy, Age = 22 },
                new User { Id = 8, Name = "Diana", IsActive = false, BirthDate = new DateTime(1998, 6, 10), Height = 162.5, Sex = SexEnum.Gril, Age = 25 },
                new User { Id = 9, Name = "Eve", IsActive = true, BirthDate = new DateTime(1987, 11, 30), Height = 168.0, Sex = SexEnum.Gril, Age = 35 },
                new User { Id = 10, Name = "Frank", IsActive = false, BirthDate = new DateTime(1975, 4, 20), Height = 190.0, Sex = SexEnum.Boy, Age = 48 }
            };
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
            }
        }
    }
}
