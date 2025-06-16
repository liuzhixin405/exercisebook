using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AiAgent.Services
{
    public interface IAIService
    {
        IAsyncEnumerable<string> GetAnswerStreamAsync(string question,
            [EnumeratorCancellation] CancellationToken cancellationToken = default, IEnumerable<string> relatedFiles = null, bool isDeepMode = false);
        Task<bool> CheckHealthAsync();
    }
}