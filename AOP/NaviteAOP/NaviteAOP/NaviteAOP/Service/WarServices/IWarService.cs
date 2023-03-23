namespace NaviteAOP.Service.WarServices
{
    public interface IWarService
    {
        string WipeOut();
        IWarService Proxy(IWarService warService);
    }
}
