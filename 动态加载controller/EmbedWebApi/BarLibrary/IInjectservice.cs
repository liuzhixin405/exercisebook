namespace BarLibrary
{
    public interface IInjectService
    {
        string Get();
    }

    public class InjectService : IInjectService
    {
        public string Get()
        {
            return "已拿到";
        }
    }
}
