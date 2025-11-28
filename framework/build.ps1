# ASP.NET Core 设计模式框架构建脚本

Write-Host "开始构建 ASP.NET Core 设计模式框架..." -ForegroundColor Green

# 清理
Write-Host "清理项目..." -ForegroundColor Yellow
dotnet clean

# 还原包
Write-Host "还原 NuGet 包..." -ForegroundColor Yellow
dotnet restore

# 构建项目
Write-Host "构建项目..." -ForegroundColor Yellow
dotnet build --configuration Release --no-restore

if ($LASTEXITCODE -eq 0) {
    Write-Host "构建成功！" -ForegroundColor Green
    
    # 运行测试（如果有的话）
    Write-Host "运行测试..." -ForegroundColor Yellow
    dotnet test --configuration Release --no-build --verbosity normal
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "所有测试通过！" -ForegroundColor Green
    } else {
        Write-Host "测试失败！" -ForegroundColor Red
    }
    
    # 运行示例项目
    Write-Host "运行示例项目..." -ForegroundColor Yellow
    Set-Location "samples/Framework.Samples"
    Start-Process -FilePath "dotnet" -ArgumentList "run" -NoNewWindow -Wait
    Set-Location "../.."
    
} else {
    Write-Host "构建失败！" -ForegroundColor Red
    exit 1
}

Write-Host "构建完成！" -ForegroundColor Green
