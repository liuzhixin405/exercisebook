namespace OllamaContext7Api.Services
{
    public interface IAIService
    {
        Task<string> GetAnswerAsync(string question);
        IAsyncEnumerable<string> GetAnswerStreamAsync(string question);
        Task<bool> CheckHealthAsync();
    }
}