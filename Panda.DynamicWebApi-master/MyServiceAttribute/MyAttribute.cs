namespace MyServiceAttribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MyAttribute : Attribute
    {
        public MyAttribute()
        {
            ServiceName = string.Empty;
        }

        public MyAttribute(string serviceName)
        {
            ServiceName = serviceName;
        }

        public string ServiceName { get; }
    }
}
