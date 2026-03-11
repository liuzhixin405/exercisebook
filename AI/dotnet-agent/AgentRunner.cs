using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotnetAgent
{
    public class AgentRunner
    {
        // Cache API key for the lifetime of the process to avoid repeated prompts
        private static string? _sessionApiKey;

        public AgentRunner()
        {
        }

        public async Task<string> RunAsync(AgentDefinition agent, string query, int? timeoutMinutes = null)
        {
            var prompt = (agent.SystemPrompt ?? string.Empty) + "\n\n" + query;

            using var cts = timeoutMinutes.HasValue
                ? new CancellationTokenSource(TimeSpan.FromMinutes(timeoutMinutes.Value))
                : new CancellationTokenSource();

            try
            {
                string? apiKeyToUse = null;

                if (!string.IsNullOrWhiteSpace(_sessionApiKey))
                {
                    apiKeyToUse = _sessionApiKey;
                }
                else
                {
                    Console.Write("Enter Deepseek API key (press Enter to use agent/frontmatter or DEEPSEEK_API_KEY): ");
                    var inputKey = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(inputKey))
                    {
                        apiKeyToUse = inputKey;
                        _sessionApiKey = inputKey;
                    }
                    else
                    {
                        // fallback to agent frontmatter
                        apiKeyToUse = string.IsNullOrWhiteSpace(agent.DeepseekApiKey) ? null : agent.DeepseekApiKey;

                        // if still null, check environment and cache that for the session
                        if (string.IsNullOrWhiteSpace(apiKeyToUse))
                        {
                            var env = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY");
                            if (!string.IsNullOrWhiteSpace(env))
                            {
                                apiKeyToUse = env;
                                _sessionApiKey = env;
                            }
                        }
                        else
                        {
                            // agent provided a key, cache it
                            _sessionApiKey = apiKeyToUse;
                        }
                    }
                }

                var baseUrl = string.IsNullOrWhiteSpace(agent.DeepseekBaseUrl) ? null : agent.DeepseekBaseUrl;
                var client = new DeepseekClient(apiKeyToUse, baseUrl);
                var task = client.GenerateAsync(agent.Model ?? "deepseek-chat", prompt);
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
