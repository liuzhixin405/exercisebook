using System.Runtime.CompilerServices;

namespace OllamaContext7Api.Services
{
    public interface IAIService
    {
        Task<string> GetAnswerAsync(string question);
        IAsyncEnumerable<string> GetAnswerStreamAsync(string question,
            [EnumeratorCancellation] CancellationToken cancellationToken = default);
        Task<bool> CheckHealthAsync();
    }
}