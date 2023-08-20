using System.Diagnostics;

namespace webapi
{
    public class MyDiagnosticListener : DiagnosticListener
    {
        public const string EventName = "MyCompany.MyEvent";

        public MyDiagnosticListener() : base("MyCompany")
        {
        }

        public void OnMyEvent(string message)
        {
            if (IsEnabled(EventName))
            {
                Write(EventName, new { Message = message });
            }
        }
    }
}
