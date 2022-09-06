using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Expressions
{
    public class PerformanceTesting
    {
        public void Invoke()
        {
            int count = 1_000_000;
            People people = new People
            {
                Id = 110,
                Name = "test",
                Age = 118
            };
            Console.WriteLine($"-------------直接测试start-------------------");
            Testing("直接复制测试",()=> {
                for (int i = 0; i < count; i++)
                {
                    PeopleCopy pc = new PeopleCopy
                    {
                        Id = people.Id,
                        Name = people.Name,
                        Age = people.Age
                    };
                }
            });
            Console.WriteLine($"---------------直接测试end-----------------");

            Console.WriteLine($"-------------序列化测试start-------------------");
            Testing("直接复制测试", () => {
                for (int i = 0; i < count; i++)
                {
                    Serializable.DeSerializ<People, PeopleCopy>(people);
                }
            });
            Console.WriteLine($"---------------序列化测试end-----------------");

            Console.WriteLine($"-------------反射测试start-------------------");
            Testing("直接复制测试", () => {
                for (int i = 0; i < count; i++)
                {
                    Reflection.Trans<People, PeopleCopy>(people);
                }
            });
            Console.WriteLine($"---------------反射测试end-----------------");

            Console.WriteLine($"-------------字典测试start-------------------");
            Testing("直接复制测试", () => {
                for (int i = 0; i < count; i++)
                {
                    ExpressionMapper.Trans<People, PeopleCopy>(people);
                }
            });
            Console.WriteLine($"---------------字典测试end-----------------");

            Console.WriteLine($"-------------泛型测试start-------------------");
            Testing("直接复制测试", () => {
                for (int i = 0; i < count; i++)
                {
                    ExpressionGenericMapper<People,PeopleCopy>.Trans(people);
                }
            });
            Console.WriteLine($"---------------泛型测试end-----------------");

            /*
             -------------直接测试start-------------------
            ***************直接复制测试**测试开始*****************
            ***************直接复制测试**测试结束**运行时间 25ms***************
            ---------------直接测试end-----------------
            -------------序列化测试start-------------------
            ***************直接复制测试**测试开始*****************
            ***************直接复制测试**测试结束**运行时间 2737ms***************
            ---------------序列化测试end-----------------
            -------------反射测试start-------------------
            ***************直接复制测试**测试开始*****************
            ***************直接复制测试**测试结束**运行时间 995ms***************
            ---------------反射测试end-----------------
            -------------字典测试start-------------------
            ***************直接复制测试**测试开始*****************
            ***************直接复制测试**测试结束**运行时间 234ms***************
            ---------------字典测试end-----------------
            -------------泛型测试start-------------------
            ***************直接复制测试**测试开始*****************
            ***************直接复制测试**测试结束**运行时间 36ms***************
            ---------------泛型测试end-----------------
             */
        }
        public void Testing(string testName,Action action)
        {
            Stopwatch stopwatch = new Stopwatch();
            Console.WriteLine($"***************{testName}**测试开始*****************");
            stopwatch.Start();
            action();
            stopwatch.Stop();
            Console.WriteLine($"***************{testName}**测试结束**运行时间 {stopwatch.ElapsedMilliseconds}ms***************");
        }


        
    }
}
