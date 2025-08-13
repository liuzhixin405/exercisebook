@echo off
chcp 65001 >nul
echo ========================================
echo    ReAct智能体 - .NET Core版本
echo ========================================
echo.

echo 正在检查.NET环境...
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ 未找到.NET环境，请先安装.NET 8.0或更高版本
    echo 下载地址：https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo ✅ .NET环境检查通过
echo.

echo 正在检查Ollama服务...
curl -s http://localhost:11434/api/tags >nul 2>&1
if %errorlevel% neq 0 (
    echo ⚠️  未检测到Ollama服务，请确保Ollama正在运行
    echo 下载地址：https://ollama.ai
    echo.
    echo 按任意键继续启动程序（可能会连接失败）...
    pause >nul
) else (
    echo ✅ Ollama服务检查通过
    echo.
)

echo 正在构建项目...
dotnet build --configuration Release
if %errorlevel% neq 0 (
    echo ❌ 项目构建失败
    pause
    exit /b 1
)

echo ✅ 项目构建成功
echo.

echo 正在启动智能体...
echo 提示：确保Ollama服务正在运行，且已下载推荐模型
echo 推荐模型：qwen2.5-coder:7b
echo 安装命令：ollama pull qwen2.5-coder:7b
echo.
echo 按任意键启动程序...
pause >nul

dotnet run --configuration Release
pause