namespace DynamicBusiness
{
    public class Test //: ITest
    {
        public async Task Process()
        {
           await Console.Out.WriteLineAsync($"Running... {DateTimeOffset.UtcNow}");
        }
    }
}