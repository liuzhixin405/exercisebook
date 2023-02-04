namespace Clock
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //数据从source经过clockDriver传递到sink
            MockTimeSource source = new MockTimeSource();
            MockTimeSink sink = new MockTimeSink();
            ClockDriver clockDriver = new ClockDriver(sink, source);         
            source.SetTime(3, 4, 5);
            ShowTime(sink.GetHours(), sink.GetMinutes(), sink.GetSecoonds());
            source.SetTime(2, 1, 0);
            ShowTime(sink.GetHours(), sink.GetMinutes(), sink.GetSecoonds());
        }
        private static void ShowTime(int hour,int minute,int second)
        {
            Console.WriteLine($"hour={hour},minute={minute},second={second}");
        }
    }

    public interface TimeSink //目的地
    {
        void SetTime(int hours,int minutes,int seconds);
    }
    public interface TimeSource   //来源
    {
        void SetDriver(ClockDriver driver);
    }
    public class ClockDriver
    {
        private TimeSink timeSink;
        public ClockDriver(TimeSink timeSink,TimeSource timeSource) 
        { 
            this.timeSink = timeSink; timeSource.SetDriver(this); 
        }
        public void Update(int hours,int minutes,int seconds)
        {
            timeSink.SetTime(hours, minutes, seconds);
        }
    }
    public class MockTimeSink : TimeSink
    {
        private int itsHours;
        private int itsMinutes;
        private int itsSeconds;
        public int GetHours()
        {
            return itsHours;
        }
        public int GetMinutes()
        {
            return itsMinutes;
        }
        public int GetSecoonds()
        {
            return itsSeconds;
        }
        public void SetTime(int hours, int minutes, int seconds)
        {
            itsHours = hours;
            itsMinutes = minutes;
            itsSeconds = seconds;
        }
    }
    public class MockTimeSource : TimeSource
    {
        private ClockDriver itsDriver;
        public void SetDriver(ClockDriver itsDriver)
        {
            this.itsDriver = itsDriver;
        }

      public void SetTime(int hours,int minutes,int seconds)
        {
            itsDriver.Update(hours, minutes, seconds);
        }
    } 
}