namespace DecoratorPattern
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IText text = new TextObject();
            Console.WriteLine($"元数据:{text.Content}");
            text = new BoldDecorator(text);
            text = new ColorDecorator(text);
            Console.WriteLine($"装饰后:{text.Content}");
            
            text = new BlockAllDecorator(text);
            Console.WriteLine($"还原后:isnull={text.Content==String.Empty}");
        }
    }
}