namespace OllamaContext7Api.Services
{
    public interface IAIService
    {
        Task<string> GetAnswerAsync(string question);
        Task<string> ProcessWithContextAsync(string question, string context);
    }
}
