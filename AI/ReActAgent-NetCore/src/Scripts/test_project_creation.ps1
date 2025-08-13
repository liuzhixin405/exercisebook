#!/usr/bin/env pwsh

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "🧪 项目创建测试脚本" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "📁 当前工作目录: $(Get-Location)" -ForegroundColor Yellow
Write-Host ""

# 创建测试用的proj目录
$testDir = "proj"
if (!(Test-Path $testDir)) {
    Write-Host "📁 创建测试目录: $testDir" -ForegroundColor Green
    New-Item -ItemType Directory -Path $testDir -Force | Out-Null
} else {
    Write-Host "📁 测试目录已存在: $testDir" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "🚀 开始测试项目创建..." -ForegroundColor Green
Write-Host ""

# 测试创建.NET Web API项目
Write-Host "📦 测试创建 .NET Web API 项目..." -ForegroundColor Cyan
Push-Location $testDir
Write-Host "📁 切换到工作目录: $(Get-Location)" -ForegroundColor Yellow
Write-Host ""

try {
    $result = & dotnet new webapi -n TestWebApi -o TestWebApi
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ .NET Web API 项目创建成功！" -ForegroundColor Green
        Write-Host "📁 项目路径: $(Get-Location)\TestWebApi" -ForegroundColor Cyan
        
        if (Test-Path "TestWebApi") {
            Write-Host "📋 项目内容:" -ForegroundColor Yellow
            Get-ChildItem "TestWebApi" | ForEach-Object {
                if ($_.PSIsContainer) {
                    Write-Host "  📁 $($_.Name)\" -ForegroundColor Blue
                } else {
                    Write-Host "  📄 $($_.Name)" -ForegroundColor White
                }
            }
        }
    } else {
        Write-Host "❌ .NET Web API 项目创建失败" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ 创建项目时发生异常: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "🧹 清理测试项目..." -ForegroundColor Yellow
if (Test-Path "TestWebApi") {
    Remove-Item "TestWebApi" -Recurse -Force
    Write-Host "✅ 测试项目已清理" -ForegroundColor Green
}

Write-Host ""
Write-Host "🔙 返回上级目录..." -ForegroundColor Yellow
Pop-Location
Write-Host "📁 当前目录: $(Get-Location)" -ForegroundColor Yellow

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "🎯 测试完成！" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "💡 如果项目被创建在 proj 目录下，说明修复成功" -ForegroundColor Yellow
Write-Host "💡 如果项目被创建在 proj 平级，说明还有问题" -ForegroundColor Yellow
Write-Host ""

Read-Host "按回车键继续..." 