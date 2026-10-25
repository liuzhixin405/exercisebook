namespace AiDevPlatform.Shared.DTOs.ApplicationPage;

public sealed class GeneratePageDraftRequest
{
    public string? Prompt { get; set; }
}

public sealed class PageDraft
{
    public string Title { get; set; } = string.Empty;
    public List<PageDraftComponent> Components { get; set; } = new();
}

public sealed class PageDraftComponent
{
    public string Type { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string InputType { get; set; } = "text";
}