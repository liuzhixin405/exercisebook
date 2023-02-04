using System.Collections;
using System.Runtime.CompilerServices;

namespace Clock2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region 改造前
            //MockTimeSink sink = new MockTimeSink();
            //MockTimeSource source = new MockTimeSource();
            //source.SetObserver(sink);
            //source.SetTime(1, 2, 3); 
            #endregion
            MockTimeSource source = new MockTimeSource();
            MockTimeSink sink = new MockTimeSink();
            source.RegisterObserver(sink);
            source.SetTime(1, 2, 3);
            ShowTime(sink.GetHours(),sink.GetMinutes(),sink.GetSecoonds());
            //事件方式最省事
        }
        private static void ShowTime(int hour, int minute, int second)
        {
            Console.WriteLine($"hour={hour},minute={minute},second={second}");
        }
    }
    public interface ClockObserver
    {
        void Update(int hours, int minutes,int seconds);    
    }
    public interface TimeSource
    {
        //void SetObserver(ClockObserver clockObserver);
        void RegisterObserver(ClockObserver clockObserver);
    }
   

    public class MockTimeSink : ClockObserver
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
        public void Update(int hours, int minutes, int seconds)
        {
            itsHours = hours;
            itsMinutes = minutes;
            itsSeconds = seconds;
        }
    }

    #region 改造前
    //public class MockTimeSource : TimeSource
    //{
    //    //private ClockObserver itsObserver; //单个对象通知
    //    private ArrayList itsObservers = new();  //多个对象通知

    //    public void SetTime(int hours,int minutes,int seconds)
    //    {
    //        //itsObserver.Update(hours, minutes, seconds);
    //        foreach (var observer in itsObservers)
    //        {
    //            ((ClockObserver)observer).Update(hours, minutes, seconds);
    //        } 
    //    }
    //    //public void SetObserver(ClockObserver clockObserver)
    //    //{
    //    //    itsObserver= clockObserver;
    //    //}
    //    public void RegisterObserver(ClockObserver clockObserver)
    //    {
    //         itsObservers.Add(clockObserver);
    //    }
    //} 
    #endregion

    public class TimeSourceImplementation : TimeSource
    {
        private ArrayList itsObservers = new();
        public void Notify(int hours,int minutes,int seconds)
        {
            foreach (var observer in itsObservers)
            {
                ((ClockObserver)observer).Update(hours, minutes, seconds);
            }
        }

        public void RegisterObserver(ClockObserver clockObserver)
        {
            itsObservers.Add( itsObservers);
        }
    }
    public class MockTimeSource : TimeSource
    {
        TimeSourceImplementation timeSourceimpl = new TimeSourceImplementation();

        public void RegisterObserver(ClockObserver clockObserver)
        {
            timeSourceimpl.RegisterObserver(clockObserver);
        }

        public void SetTime(int hours,int minutes,int seconds)
        {
            timeSourceimpl.Notify(hours, minutes, seconds);
        }
    }
}