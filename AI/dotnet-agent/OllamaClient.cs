using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DotnetAgent
{
    /// <summary>
    /// Minimal Ollama client with HTTP fallback and CLI fallback.
    /// - Tries HTTP at http://localhost:11434 (common default) if available.
    /// - Otherwise falls back to invoking the `ollama` CLI if present.
    /// Adjust as needed for your local Ollama setup.
    /// </summary>
    public class OllamaClient
    {
        private readonly HttpClient _http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        private readonly string _httpBase = "http://localhost:11434";

        public async Task<string> GenerateAsync(string model, string prompt)
        {
            // Try HTTP endpoint(s)
            try
            {
                foreach (var path in new[] { "/api/generate", "/api/predict" })
                {
                    var url = new Uri(new Uri(_httpBase), path);
                    var payload = new { model = model, prompt = prompt };
                    var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                    var resp = await _http.PostAsync(url, content);
                    if (resp.IsSuccessStatusCode)
                    {
                        var txt = await resp.Content.ReadAsStringAsync();
                        try
                        {
                            using var doc = JsonDocument.Parse(txt);
                            if (doc.RootElement.TryGetProperty("text", out var t))
                                return t.GetString() ?? txt;
                        }
                        catch { }

                        return txt;
                    }
                }
            }
            catch
            {
                // swallow and fallback to CLI
            }

            // Fallback to CLI: `ollama generate <model> --prompt "..."`
            // Fallback to CLI: try several possible subcommands depending on Ollama version
            var cliCommands = new[]
            {
                $"run {EscapeArg(model)} {EscapeArg(prompt)}",
                $"generate {EscapeArg(model)} --prompt {EscapeArg(prompt)}",
                $"predict {EscapeArg(model)} --prompt {EscapeArg(prompt)}",
                $"chat {EscapeArg(model)} --prompt {EscapeArg(prompt)}",
            };

            Exception? lastEx = null;
            foreach (var cmd in cliCommands)
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "ollama",
                    Arguments = cmd,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                using var proc = Process.Start(psi);
                if (proc == null) continue;
                var outText = await proc.StandardOutput.ReadToEndAsync();
                var errText = await proc.StandardError.ReadToEndAsync();
                await proc.WaitForExitAsync();
                if (proc.ExitCode == 0)
                {
                    return outText;
                }

                lastEx = new Exception($"Ollama CLI ({cmd}) error: {errText}");

                // If the error indicates unknown command, try next fallback
                if (errText?.Contains("unknown command", StringComparison.OrdinalIgnoreCase) == true)
                {
                    continue;
                }
                // otherwise keep trying other commands
            }

            throw lastEx ?? new Exception("Failed to invoke Ollama CLI");
        }

        private static string EscapeArg(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return '"' + s.Replace("\"", "\\\"") + '"';
        }
    }
}
