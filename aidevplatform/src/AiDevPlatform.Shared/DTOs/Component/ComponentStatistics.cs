namespace AiDevPlatform.Shared.DTOs.Component;

public class ComponentStatistics
{
    public int TotalComponents { get; set; }
    public int ActiveComponents { get; set; }
    public int InactiveComponents { get; set; }
    public int PublicComponents { get; set; }
    public int PrivateComponents { get; set; }
    public int SystemComponents { get; set; }
    public int CustomComponents { get; set; }
    public int ThirdPartyComponents { get; set; }
    public Dictionary<string, int> ComponentsByCategory { get; set; } = new();
    public Dictionary<string, int> ComponentsByType { get; set; } = new();
    public int TotalUsages { get; set; }
    public int MostUsedComponentId { get; set; }
    public string? MostUsedComponentName { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
