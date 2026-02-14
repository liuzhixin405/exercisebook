using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DotnetAgent
{
    public class AgentLoader
    {
        private static readonly Regex FrontmatterRegex = new("^---\\n(.*?)\\n---\\n?(.*)$", RegexOptions.Singleline);

        public async Task<List<AgentDefinition>> LoadAgentsFromDirectoryAsync(string dir)
        {
            var result = new List<AgentDefinition>();
            if (!Directory.Exists(dir)) return result;

            var files = Directory.GetFiles(dir, "*.md");
            foreach (var file in files)
            {
                var content = await File.ReadAllTextAsync(file);
                try
                {
                    var defs = ParseAgentMarkdown(file, content);
                    result.AddRange(defs);
                }
                catch (System.Exception ex)
                {
                    // For now, surface simple error and continue
                    System.Console.WriteLine($"Failed to load agent {file}: {ex.Message}");
                }
            }

            return result;
        }

        public List<AgentDefinition> ParseAgentMarkdown(string filePath, string content)
        {
            var m = FrontmatterRegex.Match(content);
            if (!m.Success)
            {
                throw new System.Exception("Missing YAML frontmatter");
            }

            var yaml = m.Groups[1].Value;
            var body = m.Groups[2].Value.Trim();

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var raw = deserializer.Deserialize<Dictionary<string, object?>>(yaml);
            if (raw == null) throw new System.Exception("Empty frontmatter");

            // Detect kind
            var kindStr = raw.ContainsKey("kind") ? raw["kind"]?.ToString() : "local";
            if (kindStr == "remote")
            {
                var ad = new AgentDefinition
                {
                    Kind = AgentKind.Remote,
                    Name = raw.ContainsKey("name") ? raw["name"]?.ToString() ?? "" : "",
                    DisplayName = raw.ContainsKey("display_name") ? raw["display_name"]?.ToString() : null,
                    Description = raw.ContainsKey("description") ? raw["description"]?.ToString() : null,
                    SystemPrompt = body,
                };
                return new List<AgentDefinition> { ad };
            }

            // Local
            var model = raw.ContainsKey("model") ? raw["model"]?.ToString() : null;
            var adLocal = new AgentDefinition
            {
                Kind = AgentKind.Local,
                Name = raw.ContainsKey("name") ? raw["name"]?.ToString() ?? "" : "",
                DisplayName = raw.ContainsKey("display_name") ? raw["display_name"]?.ToString() : null,
                Description = raw.ContainsKey("description") ? raw["description"]?.ToString() : null,
                SystemPrompt = body,
                Model = model,
            };

            return new List<AgentDefinition> { adLocal };
        }
    }
}
