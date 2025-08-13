# ReAct Agent - .NET Core Version Startup Script
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   ReAct Agent - .NET Core Version" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check .NET environment
Write-Host "Checking .NET environment..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET environment check passed - Version: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ .NET environment not found, please install .NET 8.0 or higher" -ForegroundColor Red
    Write-Host "Download URL: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Write-Host "Building project..." -ForegroundColor Yellow
try {
    dotnet build --configuration Release
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed"
    }
    Write-Host "✅ Project build successful" -ForegroundColor Green
} catch {
    Write-Host "❌ Project build failed" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Write-Host "Starting agent..." -ForegroundColor Yellow
Write-Host "Note: Ensure Ollama service is running and qwen2.5-coder:7b model is downloaded" -ForegroundColor Cyan
Write-Host ""

try {
    dotnet run --configuration Release
} catch {
    Write-Host "❌ Startup failed: $($_.Exception.Message)" -ForegroundColor Red
}

Read-Host "Press Enter to exit"