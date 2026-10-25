namespace AiDevPlatform.Infrastructure.Services;

internal static class LocalAiRequestGate
{
    internal static readonly SemaphoreSlim Semaphore = new(1, 1);
}