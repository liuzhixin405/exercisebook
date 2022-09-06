using IObservableDemo.subs;
using IObservableDemo.Temp;
using System;
using System.Threading.Tasks;

namespace IObservableDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {

            {
                TemperatureMonitor monitor = new TemperatureMonitor();
                TemperatureReporter reporter = new TemperatureReporter();
                reporter.Subscribe(monitor);             
                await monitor.GetTemperature();
            }

            {
                LocationTracker provider = new(); //发布者
                LocationReporter reporter1 = new("FixedGPS");  //订阅者

                reporter1.Subscribe(provider);  //订阅

                LocationReporter reporter2 = new("mobileGPS");     //订阅者
                reporter2.Subscribe(provider); //订阅

                provider.TrackLocation(new Location(47.6456, -122.1312)); //消费
                reporter1.Unsubscribe();         
                provider.TrackLocation(new Location(47.6677, -122.1199));  //消费
                provider.TrackLocation(null);
                provider.EndTransmission();
            }

            {
                BaggageHandler provider = new BaggageHandler(); //发布者
                ArrivalsMonitor observer1 = new ArrivalsMonitor("BaggageClaimMonitor1");   //订阅者
                ArrivalsMonitor observer2 = new ArrivalsMonitor("SecurityExit");  //订阅者

                provider.BaggageStatus(712, "Detroit", 3);
                observer1.Subscribe(provider);
                provider.BaggageStatus(712, "Kalamazoo", 3);
                provider.BaggageStatus(400, "New York-Kennedy", 1);
                provider.BaggageStatus(712, "Detroit", 3);
                observer2.Subscribe(provider);
                provider.BaggageStatus(511, "San Francisco", 2);
                provider.BaggageStatus(712);
                observer2.Unsubscribe();
                provider.BaggageStatus(400);
                provider.LastBaggageClaimed();

            }
            Console.Read();
        }
    }
}
