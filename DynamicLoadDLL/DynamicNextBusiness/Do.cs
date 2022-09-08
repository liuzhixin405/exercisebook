namespace DynamicNextBusiness
{
    public class Do //: IDo
    {
        public async Task Go()
        {
            await Console.Out.WriteLineAsync($"Doing... {DateTimeOffset.UtcNow}");
        }
    }
}