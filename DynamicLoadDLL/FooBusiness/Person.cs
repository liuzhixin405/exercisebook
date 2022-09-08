namespace FooBusiness
{
    public class Person
    {
        public async Task<string> Drink(string name, int water)
        {
            await Console.Out.WriteLineAsync($"姓名:{name},喝水{water}ml");
            return "喝饱了";
        }
    }
}