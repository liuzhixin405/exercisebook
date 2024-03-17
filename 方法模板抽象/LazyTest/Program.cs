namespace LazyTest
{
    internal class Program
    {
        static Lazy<IUser> _user = new Lazy<IUser>(()=>
        {
            return new User();
        });
        static void Main(string[] args)
        {
            IUser user = _user.Value;
            Console.WriteLine("over");
        }
    }

    interface IUser
    {
        // 用户接口定义
    }

    class User : IUser
    {
        // 用户类实现
    }
}
