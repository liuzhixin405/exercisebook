using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OllamaContext7Api.Services
{
    public interface IAIService
    {
        IAsyncEnumerable<string> GetAnswerStreamAsync(string question,
            [EnumeratorCancellation] CancellationToken cancellationToken = default, IEnumerable<string> relatedFiles = null, bool isDeepMode = false);
        Task<bool> CheckHealthAsync();
    }
}