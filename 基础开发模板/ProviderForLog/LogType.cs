using System.Security;

namespace ProviderForLog
{
    public record struct LogType
    {
        public static LogType  Exception =new ("Error");
        public static LogType ApplicationTrack =new ("Track");
        private readonly string _value;
        private LogType(string value)
        {
            _value = value;
        }
        public override string ToString()
        {
            return _value;
        }
    }
}