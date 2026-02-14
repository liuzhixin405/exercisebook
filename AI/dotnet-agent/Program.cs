using System;
using System.IO;
using System.Threading.Tasks;
using DotnetAgent;

Console.WriteLine("Dotnet Agent - minimal runner\n");

string FindAgentsDir()
{
    var candidates = new[]
    {
        Path.Combine(AppContext.BaseDirectory, "agents"),
        Path.Combine(Directory.GetCurrentDirectory(), "agents"),
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "agents")),
    };

    foreach (var c in candidates)
    {
        if (Directory.Exists(c)) return c;
    }

    // default to project-level agents path
    return Path.Combine(Directory.GetCurrentDirectory(), "agents");
}

var agentsDir = FindAgentsDir();
var loader = new AgentLoader();
var agents = await loader.LoadAgentsFromDirectoryAsync(agentsDir);

// Simple CLI parsing
var argv = Environment.GetCommandLineArgs();
bool list = false;
string? agentName = null;
string? query = null;
int? timeoutMins = null;

for (int i = 1; i < argv.Length; i++)
{
    var a = argv[i];
    if (a == "--list" || a == "-l") { list = true; }
    else if (a == "--agent" || a == "-a") { if (i + 1 < args.Length) { agentName = args[++i]; } }
    else if (a == "--query" || a == "-q") { if (i + 1 < args.Length) { query = args[++i]; } }
    else if (a == "--timeout-mins" || a == "-t") { if (i + 1 < argv.Length && int.TryParse(argv[++i], out var v)) timeoutMins = v; }
}

if (list)
{
    Console.WriteLine($"Loaded {agents.Count} agents:\n");
    foreach (var a in agents)
    {
        Console.WriteLine($"- {a.Name} ({a.Kind}) - {a.DisplayName}");
    }
    return;
}

if (agents.Count == 0)
{
    Console.WriteLine($"No agents found in {agentsDir}");
    return;
}

var chosen = agents.Find(a => a.Name == (agentName ?? string.Empty)) ?? agents.Find(a => a.Name == "generalist") ?? agents[0];

Console.WriteLine($"Using agent: {chosen.Name}\n");

var inputPrompt = query;
if (string.IsNullOrEmpty(inputPrompt))
{
    Console.Write("Enter query: ");
    inputPrompt = Console.ReadLine() ?? string.Empty;
}

if (string.IsNullOrEmpty(inputPrompt))
{
    Console.WriteLine("No query provided.");
    return;
}

var runner = new AgentRunner();
try
{
    var resp = await runner.RunAsync(chosen, inputPrompt, timeoutMins ?? chosen.RunConfig?.MaxTimeMinutes);
    Console.WriteLine("\n==== Agent Response ====");
    Console.WriteLine(resp);
}
catch (TimeoutException te)
{
    Console.WriteLine($"Agent timeout: {te.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Agent error: {ex.Message}");
}
