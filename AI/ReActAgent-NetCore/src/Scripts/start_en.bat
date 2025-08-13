@echo off
chcp 65001 >nul
echo ========================================
echo    ReAct Agent - .NET Core Version
echo ========================================
echo.

echo Checking .NET environment...
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ .NET environment not found, please install .NET 8.0 or higher
    echo Download URL: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo ✅ .NET environment check passed
echo.

echo Checking Ollama service...
curl -s http://localhost:11434/api/tags >nul 2>&1
if %errorlevel% neq 0 (
    echo ⚠️  Ollama service not detected, please ensure Ollama is running
    echo Download URL: https://ollama.ai
    echo.
    echo Press any key to continue starting the program (may fail to connect)...
    pause >nul
) else (
    echo ✅ Ollama service check passed
    echo.
)

echo Building project...
dotnet build --configuration Release
if %errorlevel% neq 0 (
    echo ❌ Project build failed
    pause
    exit /b 1
)

echo ✅ Project build successful
echo.

echo Starting agent...
echo Note: Ensure Ollama service is running and the recommended model is downloaded
echo Recommended model: qwen2.5-coder:7b
echo Installation command: ollama pull qwen2.5-coder:7b
echo.

echo Press any key to start the program...
pause >nul

dotnet run --configuration Release
pause