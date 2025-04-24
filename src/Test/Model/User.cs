using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Test.Model
{
    internal class User
    {
        // 整数类型
        public int Id { get; set; }

        // 字符串类型
        public string Name { get; set; }

        // 布尔类型
        public bool IsActive { get; set; }

        // 日期时间类型
        public DateTime BirthDate { get; set; }

        // 浮点数类型
        public double Height { get; set; }

        // 枚举类型
        public SexEnum Sex { get; set; }

        // 可为空的整数类型
        public int? Age { get; set; }

    }

    // 自定义枚举类型
    public enum SexEnum
    {
        Boy,
        Gril
    }
}
