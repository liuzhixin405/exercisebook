namespace IBuyStuff.Infrastructure
{
    public interface IHashingService
    {
        bool Validate(String clearPassword, string hash);
        string Hash(string clearPassword);
    }
}