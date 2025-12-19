namespace ProviderForLog
{
    public record struct LogLevel
    {
        public static LogLevel Warning =new ("Warning");
        public static LogLevel Graveness =new ("Graveness");
        private readonly string _value;
        private LogLevel(string value)
        {
            _value = value;
        }
        
        public override string ToString()
        {
            return _value;
        }
    }
}