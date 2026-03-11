using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DotnetAgent
{
    /// <summary>
    /// Minimal client that calls Deepseek HTTP API.
    /// - Reads API key from constructor or environment variable DEEPSEEK_API_KEY.
    /// - Base URL can be provided or read from DEEPSEEK_BASE_URL (defaults to https://api.deepseek.com).
    /// - Sends chat-style requests to `/chat/completions` and parses common response shapes.
    /// </summary>
    public class DeepseekClient
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;
        private readonly string _apiKey;

        public DeepseekClient(string? apiKey = null, string? baseUrl = null)
        {
            _apiKey = apiKey ?? Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY") ?? string.Empty;
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new ArgumentException("Deepseek API key is required. Set DEEPSEEK_API_KEY environment variable or pass it to the DeepseekClient constructor.");
            }

            _baseUrl = baseUrl ?? Environment.GetEnvironmentVariable("DEEPSEEK_BASE_URL") ?? "https://api.deepseek.com";
            _http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("dotnet-agent/1.0");
        }

        public async Task<string> GenerateAsync(string model, string prompt)
        {
            if (string.IsNullOrWhiteSpace(model))
            {
                model = "deepseek-chat"; // default model name used by Deepseek examples
            }

            var url = new Uri(new Uri(_baseUrl), "/chat/completions");

            var payload = new
            {
                model = model,
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = prompt }
                },
                stream = false
            };

            var opts = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var content = new StringContent(JsonSerializer.Serialize(payload, opts), Encoding.UTF8, "application/json");

            using var resp = await _http.PostAsync(url, content);
            var txt = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                // Provide a clearer message if the service reports the model does not exist
                if ((int)resp.StatusCode == 400 && !string.IsNullOrEmpty(txt) && txt.IndexOf("Model Not Exist", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    throw new Exception($"Deepseek API error: model not found. Received 400 response from {url}.\nResponse: {txt}\nSuggestion: verify the model name and try 'deepseek-chat' as a valid model name.");
                }

                throw new Exception($"Deepseek API error: {(int)resp.StatusCode} - {resp.ReasonPhrase}\n{txt}");
            }

            // Try to parse common response shapes and return a sensible text result
            try
            {
                using var doc = JsonDocument.Parse(txt);
                if (TryExtractText(doc.RootElement, out var outText))
                {
                    return outText ?? string.Empty;
                }
            }
            catch
            {
                // ignore parse errors and return raw text
            }

            return txt;
        }

        private static bool TryExtractText(JsonElement el, out string? text)
        {
            text = null;
            // common top-level fields
            if (el.ValueKind == JsonValueKind.Object)
            {
                if (el.TryGetProperty("text", out var t) && t.ValueKind == JsonValueKind.String)
                {
                    text = t.GetString();
                    return true;
                }

                if (el.TryGetProperty("output", out var o))
                {
                    if (o.ValueKind == JsonValueKind.String)
                    {
                        text = o.GetString();
                        return true;
                    }
                    if (o.ValueKind == JsonValueKind.Object && o.TryGetProperty("text", out var ot) && ot.ValueKind == JsonValueKind.String)
                    {
                        text = ot.GetString();
                        return true;
                    }
                }

                if (el.TryGetProperty("result", out var r))
                {
                    if (r.ValueKind == JsonValueKind.String)
                    {
                        text = r.GetString();
                        return true;
                    }
                }

                // choices: [{ message: { content: "..." } }] or choices: [{ text: "..." }]
                if (el.TryGetProperty("choices", out var choices) && choices.ValueKind == JsonValueKind.Array && choices.GetArrayLength() > 0)
                {
                    var first = choices[0];
                    if (first.ValueKind == JsonValueKind.Object)
                    {
                        if (first.TryGetProperty("message", out var msg) && msg.ValueKind == JsonValueKind.Object && msg.TryGetProperty("content", out var content) && content.ValueKind == JsonValueKind.String)
                        {
                            text = content.GetString();
                            return true;
                        }

                        if (first.TryGetProperty("text", out var ct) && ct.ValueKind == JsonValueKind.String)
                        {
                            text = ct.GetString();
                            return true;
                        }
                    }
                }

                // data: could be array of strings or objects
                if (el.TryGetProperty("data", out var data))
                {
                    if (data.ValueKind == JsonValueKind.String)
                    {
                        text = data.GetString();
                        return true;
                    }
                    if (data.ValueKind == JsonValueKind.Array && data.GetArrayLength() > 0)
                    {
                        var first = data[0];
                        if (first.ValueKind == JsonValueKind.String)
                        {
                            text = first.GetString();
                            return true;
                        }
                        if (first.ValueKind == JsonValueKind.Object && first.TryGetProperty("text", out var ft) && ft.ValueKind == JsonValueKind.String)
                        {
                            text = ft.GetString();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        // keep the old EscapeArg helper in case other code expects it
        private static string EscapeArg(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return '"' + s.Replace("\"", "\\\"") + '"';
        }
    }
}
