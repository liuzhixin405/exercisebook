using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AttributeDemo
{
    #region test frombody
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        var message = new MesageData
    //        {
    //            Header = "header...",
    //            Body = "body...",
    //            Footer = "footer..."
    //        };
    //        Type objT = typeof(Program);

    //        Type fromBodyT = typeof(FromBodyAttribute);
    //        MethodInfo method = objT.GetMethod("Test");
    //        ParameterInfo[] paramsInfo = method.GetParameters();
    //        var parameters = new List<object>(paramsInfo.Length);
    //        foreach (var parameterInfo in paramsInfo)
    //        {
    //            var parameter = new object();
    //            if (parameterInfo.CustomAttributes.Any(i => i.AttributeType == fromBodyT))
    //                parameter = message.Body;
    //            parameters.Add(parameter);
    //        }
    //        object result = method.Invoke(null, parameters.ToArray());
    //        Console.WriteLine(result);

    //        Console.Read();
    //    }

    //    public static string Test([FromBody] string body)
    //    {
    //        return body;
    //    }
    //}

    //public class FromBodyAttribute : Attribute
    //{

    //}
    //class MesageData
    //{
    //    public string Body { get; set; }
    //    public string Header { get; set; }
    //    public string Footer { get; set; }

    //}

    #endregion

    #region
    class Test
    {
        static void Three_Main(string[] args)
        {
            //People people = new People() { Id = 1, Name = "jack", Age = 20 ,QQ=1023456};
            ////people.Study();
            ////people.Answer("123");
            //InvokeCenter.ManagerStudent<People>(people);
            //Console.WriteLine(AttributeExtend.Validate<People>(people));
            float expected = 65.9F;
            Dashboard dashboard = new Dashboard();
            float actual = dashboard[(data) => data > 63F];
            Console.WriteLine(expected == actual);
            expected = 56.7F;
            actual = dashboard[data => data < 63F && data > 56.5F];
            Console.WriteLine(actual == expected);
            Console.Read();
        }
    }

    public class Dashboard
    {
        float[] temps = new float[10] { 56.2F, 56.7F, 56.5F, 56.9F, 58.8F, 61.3F, 65.9F, 62.1F, 59.2F, 57.5F };

        public float this[Predicate<float> predicate]
        {
            get
            {
                return Array.FindAll<float>(temps, predicate)[0];
            }
        }
    }
    public static class AttributeExtend
    {
        public static bool Validate<T>(this T t)
        {
            Type type = t.GetType();

            foreach (var prop in type.GetProperties())
            {
                if (prop.IsDefined(typeof(AbstractValidateAttribute), true))
                {
                    object oValue = prop.GetValue(t);
                    foreach (AbstractValidateAttribute attr in prop.GetCustomAttributes(typeof(AbstractValidateAttribute), true))
                    {
                        if (!attr.Validate(oValue))
                            return false;
                    }
                    
                }
            }
            return true;
        }
    }
    public abstract class AbstractValidateAttribute : Attribute
    {
        public abstract bool Validate(object oValue);
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class LongAttribute: AbstractValidateAttribute
    {
        private long _Min = 0;
        private long _Max = 0;
        public LongAttribute(long min,long max)
        {
            this._Max = max;
            this._Min = min;
        }

        public override bool Validate(object oValue)
        {
            return oValue != null && long.TryParse(oValue.ToString(), out long lValue)
                && lValue >= this._Min && lValue <= this._Max;
        }
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredAttribute : AbstractValidateAttribute
    {
        public override bool Validate(object oValue)
        {
            return oValue != null && !string.IsNullOrWhiteSpace(oValue.ToString());
        }
    }

    [Custom("message")]
    [Custom]
    public class People
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        [Required]
        [Long(10000,99999999)]
        public long QQ { get; set; }

        [Custom("message")]
        [Custom]
        public void Study()
        {
            Console.WriteLine("Study");
        }
        [Custom("message")]
        [Custom]
        public void Answer(string question)
        {
            Console.WriteLine(question);
        }
    }

    public class InvokeCenter
    {
        public static void Valaudet()
        {

        }
        public static void ManagerStudent<T>(T people) where T : People
        {
            Console.WriteLine($"{people.Id}_{people.Name}");
            people.Study();
            people.Answer("question");

            Type type = people.GetType();

            if (type.IsDefined(typeof(CustomAttribute), true))
            {
                object[] attributes = type.GetCustomAttributes(typeof(CustomAttribute), true);
                foreach (CustomAttribute attribute in attributes)
                {
                    attribute.Show();
                }

                foreach (var prop in type.GetProperties())
                {
                    if (prop.IsDefined(typeof(CustomAttribute), true))
                    {
                        object[] propAttributes = prop.GetCustomAttributes(typeof(CustomAttribute), true);
                        foreach (CustomAttribute attribute in propAttributes)
                        {
                            attribute.Show();
                        }
                    }
                }

                foreach (var method in type.GetProperties())
                {
                    if (method.IsDefined(typeof(CustomAttribute), true))
                    {
                        object[] emthodAttributes = method.GetCustomAttributes(typeof(CustomAttribute), true);
                        foreach (CustomAttribute attribute in emthodAttributes)
                        {
                            attribute.Show();
                        }
                    }
                }
            }
        }
    }
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class CustomAttribute : Attribute
    {
        public CustomAttribute()
        {

        }
        public CustomAttribute(string message)
        {
            Console.WriteLine($"ctor pars message={message}");
        }
        public void Show()
        {
            Console.WriteLine("Show");
        }
    }
    #endregion

    #region
    //class TestEnum
    //{
    //    static void Main(string[] args)
    //    {
    //        UserState userState = UserState.Frozen;
    //        Console.WriteLine(AttributeExtend.GetRemark(userState));
    //        Console.WriteLine(userState.GetRemark());
    //        Console.Read();
    //    }
    //}

    //public enum UserState
    //{
    //   [Remark("正常")]
    //    Normal =0,
    //   [Remark("已冻结")]
    //    Frozen =1,
    //   [Remark("已删除")]
    //    Deleted=2
    //}
    //[AttributeUsage(AttributeTargets.Field)]
    //public class RemarkAttribute : Attribute
    //{
    //    public string Remark { get; private set; }
    //    public RemarkAttribute(string remark)
    //    {
    //        this.Remark = remark;
    //    }
    //}

    //public static class AttributeExtend
    //{
    //    public static string GetRemark(this Enum value)
    //    {
    //        Type type = value.GetType();
    //        var filed = type.GetField(value.ToString());
    //        if (filed.IsDefined(typeof(RemarkAttribute), true))
    //        {
    //            RemarkAttribute attr = (RemarkAttribute)filed.GetCustomAttribute(typeof(RemarkAttribute), true);
    //            return attr.Remark;
    //        }
    //        else
    //        {
    //            return value.ToString();
    //        }
    //    }
    //}
    #endregion
}
