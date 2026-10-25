namespace AiDevPlatform.Shared.DTOs.Form;

public class FormStatistics
{
    public int TotalForms { get; set; }
    public int ActiveForms { get; set; }
    public int InactiveForms { get; set; }
    public int PublicForms { get; set; }
    public int PrivateForms { get; set; }
    public int TotalSubmissions { get; set; }
    public int PendingSubmissions { get; set; }
    public int ApprovedSubmissions { get; set; }
    public int RejectedSubmissions { get; set; }
    public int CompletedSubmissions { get; set; }
    public double ApprovalRate { get; set; }
    public double AverageSubmissionTime { get; set; } // in hours
    public Dictionary<string, int> SubmissionsByStatus { get; set; } = new();
    public Dictionary<string, int> SubmissionsByForm { get; set; } = new();
    public List<FormTrend> SubmissionTrends { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public class FormTrend
{
    public DateTime Date { get; set; }
    public int Submissions { get; set; }
    public int Approvals { get; set; }
    public int Rejections { get; set; }
}
