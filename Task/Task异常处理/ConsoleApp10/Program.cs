namespace ConsoleApp10
{
    internal class Program
    {
        /// <summary>
        /// 异常可控
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            try
            {
                var srv = await MyService.CreateAsync();
                Console.WriteLine("Loaded!");
            }
            catch(Exception ex)
            {
                    Console.WriteLine($"拿到异常了: {ex.Message}");
            }
           
        }
    }

    class MyService
    {
        private MyService()
        {

        }

        async Task InitAsync()
        {
            await Task.Delay(1000);
            throw new Exception("Init failed!");
        }

        public static async Task<MyService> CreateAsync()
        {
            var service = new MyService();
            await service.InitAsync();
            return service;
        }
    }
}
