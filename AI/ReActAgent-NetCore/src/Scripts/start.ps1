# ReAct智能体 - .NET Core版本启动脚本
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   ReAct智能体 - .NET Core版本" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 检查.NET环境
Write-Host "正在检查.NET环境..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET环境检查通过 - 版本: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ 未找到.NET环境，请先安装.NET 8.0或更高版本" -ForegroundColor Red
    Write-Host "下载地址：https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    Read-Host "按回车键退出"
    exit 1
}

Write-Host ""

# 构建项目
Write-Host "正在构建项目..." -ForegroundColor Yellow
try {
    dotnet build --configuration Release
    if ($LASTEXITCODE -ne 0) {
        throw "构建失败"
    }
    Write-Host "✅ 项目构建成功" -ForegroundColor Green
} catch {
    Write-Host "❌ 项目构建失败" -ForegroundColor Red
    Read-Host "按回车键退出"
    exit 1
}

Write-Host ""

# 启动智能体
Write-Host "正在启动智能体..." -ForegroundColor Yellow
Write-Host "提示：确保Ollama服务正在运行，且已下载qwen2.5-coder:7b模型" -ForegroundColor Cyan
Write-Host ""

try {
    dotnet run --configuration Release
} catch {
    Write-Host "❌ 启动失败: $($_.Exception.Message)" -ForegroundColor Red
}

Read-Host "按回车键退出" 