using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotnetAgent
{
    public class AgentRunner
    {
        private readonly OllamaClient _ollama;

        public AgentRunner()
        {
            _ollama = new OllamaClient();
        }

        public async Task<string> RunAsync(AgentDefinition agent, string query, int? timeoutMinutes = null)
        {
            var prompt = (agent.SystemPrompt ?? string.Empty) + "\n\n" + query;

            using var cts = timeoutMinutes.HasValue
                ? new CancellationTokenSource(TimeSpan.FromMinutes(timeoutMinutes.Value))
                : new CancellationTokenSource();

            try
            {
                var task = _ollama.GenerateAsync(agent.Model ?? "llama2", prompt);
                using (cts.Token.Register(() => { /* no-op */ }))
                {
                    var completed = await Task.WhenAny(task, Task.Delay(Timeout.Infinite, cts.Token));
                    if (completed != task)
                    {
                        throw new TimeoutException("Agent run timed out");
                    }
                    return await task;
                }
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("Agent run cancelled or timed out");
            }
        }
    }
}
