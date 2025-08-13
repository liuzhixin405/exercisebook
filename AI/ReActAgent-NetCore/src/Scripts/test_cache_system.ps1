# 测试智能缓存系统的PowerShell脚本

Write-Host "🧪 智能缓存系统测试脚本" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan

# 设置工作目录
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectDir = Split-Path -Parent $scriptDir
Set-Location $projectDir

Write-Host "📁 工作目录: $projectDir" -ForegroundColor Green

# 编译项目
Write-Host "🔨 正在编译项目..." -ForegroundColor Yellow
try {
    $buildResult = dotnet build --no-restore
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ 编译成功" -ForegroundColor Green
    } else {
        Write-Host "❌ 编译失败" -ForegroundColor Red
        Write-Host $buildResult
        exit 1
    }
} catch {
    Write-Host "❌ 编译异常: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 检查Redis服务（可选）
Write-Host "🔍 检查Redis服务状态..." -ForegroundColor Yellow
try {
    $redisTest = Test-NetConnection -ComputerName "localhost" -Port 6379 -InformationLevel Quiet
    if ($redisTest) {
        Write-Host "✅ Redis服务可用 (localhost:6379)" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Redis服务不可用，将使用内存缓存" -ForegroundColor Yellow
    }
} catch {
    Write-Host "⚠️  无法检查Redis服务，将使用内存缓存" -ForegroundColor Yellow
}

# 运行缓存系统测试
Write-Host "🚀 启动缓存系统测试..." -ForegroundColor Yellow
Write-Host "注意: 这是一个基础测试，实际功能需要完整的Redis环境" -ForegroundColor Yellow

# 创建测试数据
$testQuestions = @(
    "创建一个React应用",
    "新建一个.NET Web API项目", 
    "生成一个Python Flask应用",
    "读取config.json文件",
    "创建src目录"
)

Write-Host "📝 测试问题列表:" -ForegroundColor Cyan
foreach ($question in $testQuestions) {
    Write-Host "  - $question" -ForegroundColor White
}

Write-Host ""
Write-Host "💡 测试说明:" -ForegroundColor Cyan
Write-Host "1. 这些问题将被预处理并提取关键词和意图" -ForegroundColor White
Write-Host "2. 系统会尝试在缓存中查找相似问题" -ForegroundColor White
Write-Host "3. 根据相似度决定使用缓存还是调用模型" -ForegroundColor White
Write-Host "4. 新的问答对会被存储到缓存中" -ForegroundColor White

Write-Host ""
Write-Host "🎯 要运行完整测试，请:" -ForegroundColor Cyan
Write-Host "1. 确保Redis服务正在运行" -ForegroundColor White
Write-Host "2. 运行主程序: dotnet run" -ForegroundColor White
Write-Host "3. 输入上述测试问题" -ForegroundColor White
Write-Host "4. 观察缓存系统的行为" -ForegroundColor White

Write-Host ""
Write-Host "🔧 缓存系统特性:" -ForegroundColor Cyan
Write-Host "✓ 问题预处理和标准化" -ForegroundColor Green
Write-Host "✓ 关键词提取和意图识别" -ForegroundColor Green
Write-Host "✓ 语义相似度计算" -ForegroundColor Green
Write-Host "✓ 智能路由决策" -ForegroundColor Green
Write-Host "✓ Redis缓存存储" -ForegroundColor Green
Write-Host "✓ 缓存统计和监控" -ForegroundColor Green

Write-Host ""
Write-Host "✅ 测试脚本执行完成" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Cyan 