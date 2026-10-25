using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.DTOs.Application;

namespace AiDevPlatform.Core.Features.Applications;

public sealed class DevelopmentWorkflow
{
    private readonly IDevelopmentStore _store;
    private readonly IAiDevelopmentGenerator _generator;
    private readonly IDevelopmentVerifier _verifier;

    public DevelopmentWorkflow(IDevelopmentStore store, IAiDevelopmentGenerator generator, IDevelopmentVerifier verifier)
    {
        _store = store;
        _generator = generator;
        _verifier = verifier;
    }

    public async Task<TeamStandard> AddStandardAsync(TeamStandard standard, CancellationToken cancellationToken)
    {
        Text(standard.Name, 80); Text(standard.Version, 40); Text(standard.Instructions, 4000);
        if (standard.RequiredFiles == null || standard.RequiredFiles.Count > 12 ||
            standard.ForbiddenTerms == null || standard.ForbiddenTerms.Count > 20 ||
            standard.RequiredChecks == null || standard.RequiredChecks.Count > 12)
            throw new ArgumentException("规范条目过多。");
        foreach (var path in standard.RequiredFiles) ValidatePath(path);
        foreach (var term in standard.ForbiddenTerms) Text(term, 120);
        foreach (var check in standard.RequiredChecks) Text(check, 80);
        standard.RequiredChecks = standard.RequiredChecks.Select(check => check.Trim()).ToList();
        if (standard.RequiredChecks.Distinct(StringComparer.OrdinalIgnoreCase).Count() != standard.RequiredChecks.Count)
            throw new ArgumentException("检查名称不能重复。");
        standard.Name = standard.Name.Trim(); standard.Version = standard.Version.Trim();
        standard.Id = Guid.NewGuid(); standard.CreatedAt = DateTime.UtcNow;
        await _store.AddStandardAsync(standard, cancellationToken);
        return standard;
    }

    public async Task<DevelopmentTask> CreateAsync(CreateDevelopmentTask input, CancellationToken cancellationToken)
    {
        Text(input.Title, 120); Text(input.Technology, 200); Text(input.Requirement, 4000);
        ValidateFiles(input.ContextFiles, 8, 16000, true);
        var standard = (await _store.StandardsAsync(cancellationToken)).FirstOrDefault(item => item.Id == input.StandardId)
            ?? throw new ArgumentException("请选择存在的规范版本。");
        var task = new DevelopmentTask
        {
            Id = Guid.NewGuid(), Revision = 1, Title = input.Title.Trim(), Technology = input.Technology.Trim(),
            Requirement = input.Requirement, Standard = standard, ContextFiles = input.ContextFiles, CreatedAt = DateTime.UtcNow
        };
        await _store.SaveAsync(task, 0, cancellationToken);
        return task;
    }

    public async Task<DevelopmentTask> GetAsync(Guid id, CancellationToken cancellationToken) =>
        await _store.GetAsync(id, cancellationToken) ?? throw new KeyNotFoundException("任务不存在。");

    public async Task<DevelopmentTask> ClarifyAsync(Guid id, DevelopmentAction input, CancellationToken cancellationToken)
    {
        var task = await CurrentAsync(id, input.ExpectedRevision, cancellationToken);
        if (task.Confirmed) throw new InvalidOperationException("需求已冻结，请创建新任务变更需求。");
        task.Draft = await _generator.ClarifyAsync(task, cancellationToken);
        ValidateDraft(task.Draft);
        return await SaveAsync(task, cancellationToken);
    }

    public async Task<DevelopmentTask> ConfirmAsync(Guid id, ConfirmDevelopmentTask input, CancellationToken cancellationToken)
    {
        var task = await CurrentAsync(id, input.ExpectedRevision, cancellationToken);
        if (task.Confirmed || task.Draft == null) throw new InvalidOperationException("请先澄清需求，已确认的需求不可覆盖。");
        ValidateDraft(input.Draft);
        if (input.Answers == null || input.Answers.Count != task.Draft.Questions.Count)
            throw new ArgumentException("请逐一回答所有待确认问题。");
        foreach (var answer in input.Answers) Text(answer, 1000);
        input.Draft.Questions = task.Draft.Questions;
        input.Draft.Assumptions = task.Draft.Assumptions;
        task.Draft = input.Draft; task.Answers = input.Answers; task.Confirmed = true;
        return await SaveAsync(task, cancellationToken);
    }

    public async Task<DevelopmentTask> GenerateAsync(Guid id, DevelopmentAction input, CancellationToken cancellationToken)
    {
        var task = await CurrentAsync(id, input.ExpectedRevision, cancellationToken);
        if (!task.Confirmed) throw new InvalidOperationException("需求未确认，不能生成代码。");
        if (task.Changes.Count >= 24) throw new InvalidOperationException("每个任务最多保留 24 个源码版本，请新建任务。");
        if (input.Feedback?.Length > 2000) throw new ArgumentException("修改意见过长。");
        if (input.GenerationMode is not ("change" or "project" or "repair")) throw new ArgumentException("生成模式无效。");
        var baseline = task.Changes.LastOrDefault()?.Files ?? task.ContextFiles;
        var planned = task.Draft!.PlannedFiles.Concat(task.Standard.RequiredFiles).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (planned.Count > 24) throw new ArgumentException("项目计划超过 24 个文件的工作区上限。");
        var targets = planned.Except(baseline.Select(file => file.Path), StringComparer.OrdinalIgnoreCase).Take(1).ToList();
        if (input.GenerationMode == "project" && targets.Count == 0)
            throw new InvalidOperationException("计划文件已生成，请通过修改意见修复现有文件。");
        if (input.GenerationMode == "repair")
        {
            ValidatePath(input.TargetPath);
            Text(input.Feedback, 2000);
            var target = baseline.FirstOrDefault(file => file.Path.Equals(input.TargetPath, StringComparison.OrdinalIgnoreCase))
                ?? throw new ArgumentException("只能定点修复已有文件。");
            if (target.Content.Length > 14000) throw new ArgumentException("目标文件超出本轮完整上下文预算。");
            targets = new() { target.Path };
        }
        var proposal = input.GenerationMode is "project" or "repair"
            ? await _generator.GenerateFilesAsync(task, targets, input.Feedback ?? string.Empty, cancellationToken)
            : await _generator.GenerateAsync(task, input.Feedback ?? string.Empty, cancellationToken);
        try { Text(proposal.Summary, 1000); ValidateFiles(proposal.Files, 16, 100000, false); }
        catch (ArgumentException exception) { throw new System.Text.Json.JsonException("AI 文件不满足安全边界。", exception); }
        if (input.GenerationMode is "project" or "repair" && !targets.ToHashSet(StringComparer.OrdinalIgnoreCase).SetEquals(proposal.Files.Select(file => file.Path)))
            throw new System.Text.Json.JsonException("模型没有严格返回本批计划的完整文件，已有版本未改变。");
        var files = baseline.ToDictionary(file => file.Path, file => file, StringComparer.OrdinalIgnoreCase);
        foreach (var file in proposal.Files) files[file.Path] = file;
        var snapshot = files.Values.OrderBy(file => file.Path, StringComparer.Ordinal).ToList();
        ValidateFiles(snapshot, 24, 120000, false);
        var checks = new List<QualityCheck>
        {
            new() { Name = "文件边界", Status = "passed", Detail = "已校验相对路径、大小、重名与保留目录；没有写入或执行生成源码。" }
        };
        var missing = task.Standard.RequiredFiles.Where(path => !files.ContainsKey(path)).ToList();
        checks.Add(new() { Name = "约定文件", Status = missing.Count == 0 ? "passed" : "failed", Detail = missing.Count == 0 ? "必需文件齐全。" : string.Join("、", missing) });
        var ungenerated = planned.Where(path => !files.ContainsKey(path)).ToList();
        checks.Add(new() { Name = "项目计划", Status = ungenerated.Count == 0 ? "passed" : "failed",
            Detail = ungenerated.Count == 0 ? "计划文件齐全；不代表业务验收通过。" : "尚未生成：" + string.Join("、", ungenerated) });
        var violations = task.Standard.ForbiddenTerms.Where(term => snapshot.Any(file => file.Content.Contains(term, StringComparison.OrdinalIgnoreCase))).ToList();
        checks.Add(new() { Name = "禁用文本", Status = violations.Count == 0 ? "passed" : "failed", Detail = violations.Count == 0 ? "未命中禁用文本；不是语义或安全审计。" : string.Join("、", violations) });
        checks.Add(new() { Name = "构建 / 格式 / 测试 / 架构", Status = "not-run", Detail = "未接入隔离执行器；没有运行 AI 提供的命令，不能据此认定可构建或满足全部规范。" });
        checks.Add(new() { Name = "依赖 / 公共接口 / 业务验收", Status = "manual", Detail = "需人工核对新增依赖、公共接口影响及全部验收条件。" });
        task.Changes.Add(new ChangeRevision { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Summary = proposal.Summary,
            Files = snapshot, Checks = checks, Generation = proposal.Generation, GenerationFeedback = input.Feedback ?? string.Empty,
            GenerationTargetPath = input.GenerationMode == "repair" ? targets[0] : string.Empty });
        return await SaveAsync(task, cancellationToken);
    }

    public async Task<DevelopmentTask> MaterializeAsync(Guid id, DevelopmentAction input, CancellationToken cancellationToken)
    {
        var task = await CurrentAsync(id, input.ExpectedRevision, cancellationToken);
        var change = task.Changes.LastOrDefault() ?? throw new InvalidOperationException("没有可创建项目的源码版本。");
        if (!input.RiskAcknowledged || change.Workspace != null)
            throw new InvalidOperationException("请确认创建文件的风险；已关联的项目目录不会被覆盖。");
        ValidateFiles(change.Files, 24, 120000, false);
        change.Workspace = await _store.MaterializeAsync(task, change, cancellationToken);
        return await SaveAsync(task, cancellationToken);
    }

    public async Task<DevelopmentTask> ReviewAsync(Guid id, DevelopmentAction input, CancellationToken cancellationToken)
    {
        var task = await CurrentAsync(id, input.ExpectedRevision, cancellationToken);
        var change = task.Changes.LastOrDefault() ?? throw new InvalidOperationException("没有待审阅源码。");
        if (change.Review != "pending") throw new InvalidOperationException("此版本已审阅，不能覆盖记录。");
        Text(input.Feedback, 2000);
        if (input.Decision is not ("accepted" or "changes-requested")) throw new ArgumentException("审阅结论无效。");
        if (input.Decision == "accepted" && (!input.RiskAcknowledged || !CanDeliver(task.Standard, change)))
            throw new InvalidOperationException("请先修复硬性违规并通过规范的必需检查，再确认人工验收风险。");
        change.Review = input.Decision; change.ReviewComment = input.Feedback;
        return await SaveAsync(task, cancellationToken);
    }

    public async Task<DevelopmentTask> VerifyAsync(Guid id, DevelopmentAction input, CancellationToken cancellationToken)
    {
        var task = await CurrentAsync(id, input.ExpectedRevision, cancellationToken);
        var change = task.Changes.LastOrDefault() ?? throw new InvalidOperationException("没有待验证源码。");
        if (change.Review != "pending" || task.Standard.RequiredChecks.Count == 0 || change.Verifications.Count >= 5)
            throw new InvalidOperationException("只能验证配置了必需检查的待审版本，每版最多 5 次。");
        var request = new VerificationRequest
        {
            RunId = Guid.NewGuid(), TaskId = task.Id, ChangeId = change.Id, Technology = task.Technology,
            Files = change.Files, RequiredChecks = task.Standard.RequiredChecks
        };
        var report = await _verifier.VerifyAsync(request, cancellationToken);
        if (report == null || report.RunId != request.RunId || report.TaskId != task.Id || report.ChangeId != change.Id ||
            report.Checks == null || report.Checks.Count != request.RequiredChecks.Count ||
            report.Checks.Any(check => check == null || !request.RequiredChecks.Contains(check.Name, StringComparer.Ordinal) ||
                check.Status is not ("passed" or "failed" or "not-run") || string.IsNullOrWhiteSpace(check.Detail) || check.Detail.Length > 2000) ||
            report.Checks.Select(check => check.Name).Distinct(StringComparer.Ordinal).Count() != report.Checks.Count)
            throw new System.Text.Json.JsonException("验证报告未绑定本次请求或缺少有效检查。");
        report.ReceivedAt = DateTime.UtcNow;
        change.Verifications.Add(report);
        return await SaveAsync(task, cancellationToken);
    }

    public static bool CanDeliver(TeamStandard standard, ChangeRevision change)
    {
        if (change.Checks.Any(check => check.Status == "failed")) return false;
        if (standard.RequiredChecks.Count == 0) return true;
        var report = change.Verifications.LastOrDefault();
        return report != null && report.ChangeId == change.Id &&
            standard.RequiredChecks.All(name => report.Checks.Any(check => check.Name == name && check.Status == "passed"));
    }

    private async Task<DevelopmentTask> CurrentAsync(Guid id, int revision, CancellationToken cancellationToken)
    {
        var task = await GetAsync(id, cancellationToken);
        if (task.Revision != revision) throw new InvalidOperationException("任务已更新，请刷新后重试。");
        return task;
    }

    private async Task<DevelopmentTask> SaveAsync(DevelopmentTask task, CancellationToken cancellationToken)
    {
        var previous = task.Revision++;
        await _store.SaveAsync(task, previous, cancellationToken);
        return task;
    }

    public static void ValidateDraft(RequirementDraft? draft)
    {
        if (draft == null) throw new ArgumentException("需求稿为空。");
        Text(draft.Specification, 6000);
        if (draft.Questions == null || draft.Questions.Count > 8 || draft.Assumptions == null || draft.Assumptions.Count > 10 ||
            draft.AcceptanceCriteria == null || draft.AcceptanceCriteria.Count is < 1 or > 12 ||
            draft.PlannedFiles == null || draft.PlannedFiles.Count is < 1 or > 16)
            throw new ArgumentException("需求稿条目无效。");
        foreach (var text in draft.Questions.Concat(draft.Assumptions).Concat(draft.AcceptanceCriteria)) Text(text, 1000);
        foreach (var path in draft.PlannedFiles) ValidatePath(path);
    }

    public static void ValidateFiles(List<SourceFile>? files, int count, int size, bool allowEmpty)
    {
        if (files == null || files.Count > count || (!allowEmpty && files.Count == 0)) throw new ArgumentException("文件数量无效。");
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var total = 0;
        foreach (var file in files)
        {
            if (file == null) throw new ArgumentException("文件为空。");
            ValidatePath(file.Path);
            if (!names.Add(file.Path) || file.Content == null || file.Content.Length > 40000 || file.Content.Contains('\0'))
                throw new ArgumentException("文件重名、过大或包含二进制内容。");
            total += file.Content.Length;
        }
        if (total > size || names.Any(name => names.Any(other => other.StartsWith(name + "/", StringComparison.OrdinalIgnoreCase))))
            throw new ArgumentException("文件总大小超限或目录冲突。");
    }

    public static void ValidatePath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || path.Length > 180 || path.Any(character => char.IsControl(character) || "\\:*?\"<>|".Contains(character)))
            throw new ArgumentException("文件路径无效。");
        foreach (var segment in path.Split('/'))
        {
            var stem = segment.Split('.')[0].ToUpperInvariant();
            if (segment is "" or "." or ".." || segment.EndsWith(' ') || segment.EndsWith('.') ||
                segment.Equals(".git", StringComparison.OrdinalIgnoreCase) || segment.Equals("_platform", StringComparison.OrdinalIgnoreCase) ||
                (segment.StartsWith(".env", StringComparison.OrdinalIgnoreCase) && !segment.Equals(".env.example", StringComparison.OrdinalIgnoreCase)) ||
                stem is "CON" or "PRN" or "AUX" or "NUL" ||
                (stem.Length == 4 && (stem.StartsWith("COM") || stem.StartsWith("LPT")) && char.IsDigit(stem[3])))
                throw new ArgumentException("不接受越界、敏感、保留或设备路径。");
        }
    }

    private static void Text(string? value, int max)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > max) throw new ArgumentException("必填内容为空或超过长度限制。");
    }
}