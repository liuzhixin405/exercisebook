using System.Text.Json;
using System.Text;
using System.Security.Cryptography;
using AiDevPlatform.Core.Features.Applications;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.DTOs.Application;
using Microsoft.Extensions.Configuration;

namespace AiDevPlatform.Infrastructure.Services;

public sealed class JsonDevelopmentStore : IDevelopmentStore
{
    private static readonly SemaphoreSlim Gate = new(1, 1);
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
    private readonly string _directory;

    public JsonDevelopmentStore(IConfiguration configuration)
    {
        _directory = Path.GetFullPath(configuration["DevelopmentWorkspace:DataPath"] ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LowCodePlatform", "Development"));
        Directory.CreateDirectory(_directory);
    }

    public async Task<List<TeamStandard>> StandardsAsync(CancellationToken cancellationToken)
    {
        await Gate.WaitAsync(cancellationToken);
        try
        {
            var standards = await ReadAllAsync<TeamStandard>("*.standard.json", cancellationToken);
            if (standards.Count == 0)
            {
                var seed = new TeamStandard
                {
                    Id = new Guid("22222222-2222-4222-8222-222222222222"), Name = "团队基础规范", Version = "1.0.0", CreatedAt = DateTime.UtcNow,
                    Instructions = "遵循所选语言生态的标准格式和清晰的模块边界。优先复用已有接口，不重复实现公共能力。显式处理错误，禁止记录凭据。输入必须校验，外部数据不可信。提供真实测试和运行说明，不得以 TODO 或假成功代替实现。新增依赖、公共接口和跨模块变更需说明影响。",
                    RequiredFiles = new() { "README.md" }, ForbiddenTerms = new() { "TODO" }
                };
                await WriteAsync(seed.Id, "standard", seed, cancellationToken);
                standards.Add(seed);
            }
            return standards.OrderByDescending(item => item.CreatedAt).ToList();
        }
        finally { Gate.Release(); }
    }

    public async Task AddStandardAsync(TeamStandard standard, CancellationToken cancellationToken)
    {
        await Gate.WaitAsync(cancellationToken);
        try
        {
            var existing = await ReadAllAsync<TeamStandard>("*.standard.json", cancellationToken);
            if (existing.Count >= 50 || existing.Any(item => item.Name.Equals(standard.Name, StringComparison.OrdinalIgnoreCase) &&
                    item.Version.Equals(standard.Version, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("规范名称与版本已存在，或已达到本机 50 个版本上限。");
            await WriteAsync(standard.Id, "standard", standard, cancellationToken);
        }
        finally { Gate.Release(); }
    }

    public Task<List<DevelopmentTask>> TasksAsync(CancellationToken cancellationToken) => ReadAllAsync<DevelopmentTask>("*.task.json", cancellationToken);

    public async Task<DevelopmentTask?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var path = Path.Combine(_directory, $"{id:D}.task.json");
        return File.Exists(path) ? JsonSerializer.Deserialize<DevelopmentTask>(await File.ReadAllTextAsync(path, cancellationToken), Options) : null;
    }

    public async Task SaveAsync(DevelopmentTask task, int expectedRevision, CancellationToken cancellationToken)
    {
        await Gate.WaitAsync(cancellationToken);
        try
        {
            var current = await GetAsync(task.Id, cancellationToken);
            if ((current?.Revision ?? 0) != expectedRevision) throw new InvalidOperationException("任务已更新，请刷新后重试。");
            if (current == null && Directory.EnumerateFiles(_directory, "*.task.json").Take(100).Count() >= 100)
                throw new InvalidOperationException("本机最多保存 100 个任务。");
            await WriteAsync(task.Id, "task", task, cancellationToken);
        }
        finally { Gate.Release(); }
    }

    private async Task<List<T>> ReadAllAsync<T>(string pattern, CancellationToken cancellationToken)
    {
        var result = new List<T>();
        foreach (var path in Directory.EnumerateFiles(_directory, pattern).Take(100))
        {
            var value = JsonSerializer.Deserialize<T>(await File.ReadAllTextAsync(path, cancellationToken), Options);
            if (value != null) result.Add(value);
        }
        return result;
    }

    public async Task<ProjectWorkspace> MaterializeAsync(DevelopmentTask task, ChangeRevision change, CancellationToken cancellationToken)
    {
        DevelopmentWorkflow.ValidateFiles(change.Files, 24, 120000, false);
        var root = Path.Combine(_directory, "projects");
        EnsureDirectory(root);
        var workspace = new ProjectWorkspace { Id = Guid.NewGuid(), TaskId = task.Id, ChangeId = change.Id, CreatedAt = DateTime.UtcNow };
        workspace.DirectoryPath = Path.Combine(root, $"{task.Id:N}-{change.Id:N}-{workspace.Id:N}");
        var staging = Path.Combine(root, ".creating-" + workspace.Id.ToString("N"));
        EnsureDirectory(staging);
        try
        {
            foreach (var file in change.Files)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var target = Path.Combine(staging, file.Path.Replace('/', Path.DirectorySeparatorChar));
                EnsureDirectory(Path.GetDirectoryName(target)!);
                await WriteNewAsync(target, file.Content, cancellationToken);
                var bytes = Encoding.UTF8.GetBytes(file.Content);
                workspace.Files.Add(new ProjectFileFingerprint { Path = file.Path, Utf8Bytes = bytes.Length,
                    // [联网-官方: https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.sha256.hashdata?view=net-8.0]
                    Sha256 = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(),
                    Origin = FileOrigin(task, change.Id, file.Path) });
            }
            EnsureDirectory(Path.Combine(staging, "_platform"));
            await WriteNewAsync(Path.Combine(staging, "_platform", "project.json"), JsonSerializer.Serialize(new
            {
                schemaVersion = 1, purpose = "inspection-only", taskId = task.Id, changeId = change.Id,
                task.Title, task.Technology, task.Requirement, task.Standard, task.Draft, task.Answers,
                change.Generation, change.GenerationTargetPath, change.GenerationFeedback, change.Checks, change.Verifications, change.Review, workspace
            }, Options), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            // [联网-官方: https://learn.microsoft.com/en-us/dotnet/api/system.io.directory.move?view=net-8.0]
            Directory.Move(staging, workspace.DirectoryPath);
            return workspace;
        }
        finally { if (Directory.Exists(staging)) Directory.Delete(staging, true); }
    }

    private static void EnsureDirectory(string path)
    {
        var parent = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(parent)) EnsureDirectory(parent);
        if (Directory.Exists(path) || File.Exists(path))
        {
            // [联网-官方: https://learn.microsoft.com/en-us/dotnet/api/system.io.file.getattributes?view=net-8.0]
            if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
                throw new IOException("项目目录不能经过符号链接或重解析点。");
        }
        // [来源: dotnet-csharp#0125 p744-745] Directory.CreateDirectory
        Directory.CreateDirectory(path);
    }

    private static string FileOrigin(DevelopmentTask task, Guid changeId, string path)
    {
        var previous = task.ContextFiles.FirstOrDefault(file => file.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
        var origin = previous != null ? "context" : "legacy-unattributed";
        foreach (var revision in task.Changes)
        {
            var file = revision.Files.FirstOrDefault(item => item.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
            if (file != null)
            {
                if (previous == null || previous.Content != file.Content)
                    origin = revision.Generation is { } trace ? $"model:{trace.RequestId:D}" : "legacy-unattributed";
                previous = file;
            }
            if (revision.Id == changeId) break;
        }
        return origin;
    }

    private static async Task WriteNewAsync(string path, string content, CancellationToken cancellationToken)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        // [来源: dotnet-csharp#0125 p742-743] FileStream/WriteAsync; [联网-官方: https://learn.microsoft.com/en-us/dotnet/api/system.io.filemode?view=net-8.0] CreateNew
        using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, true);
        await stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
    }

    // [联网-官方: https://learn.microsoft.com/en-us/dotnet/api/system.io.file.move?view=net-8.0]
    private async Task WriteAsync<T>(Guid id, string type, T value, CancellationToken cancellationToken)
    {
        var path = Path.Combine(_directory, $"{id:D}.{type}.json");
        var temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            await File.WriteAllTextAsync(temporary, JsonSerializer.Serialize(value, Options), cancellationToken);
            File.Move(temporary, path, true);
        }
        finally { if (File.Exists(temporary)) File.Delete(temporary); }
    }
}