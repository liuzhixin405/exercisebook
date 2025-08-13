@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

echo.
echo ========================================
echo 🧪 项目创建测试脚本
echo ========================================
echo.

echo 📁 当前工作目录: %CD%
echo.

REM 创建测试用的proj目录
if not exist "proj" (
    echo 📁 创建测试目录: proj
    mkdir proj
) else (
    echo 📁 测试目录已存在: proj
)

echo.
echo 🚀 开始测试项目创建...
echo.

REM 测试创建.NET Web API项目
echo 📦 测试创建 .NET Web API 项目...
cd proj
echo 📁 切换到工作目录: %CD%
echo.

dotnet new webapi -n TestWebApi -o TestWebApi
if !errorlevel! equ 0 (
    echo ✅ .NET Web API 项目创建成功！
    echo 📁 项目路径: %CD%\TestWebApi
    if exist "TestWebApi" (
        echo 📋 项目内容:
        dir /b "TestWebApi"
    )
) else (
    echo ❌ .NET Web API 项目创建失败
)

echo.
echo 🧹 清理测试项目...
if exist "TestWebApi" (
    rmdir /s /q "TestWebApi"
    echo ✅ 测试项目已清理
)

echo.
echo 🔙 返回上级目录...
cd ..
echo 📁 当前目录: %CD%

echo.
echo ========================================
echo 🎯 测试完成！
echo ========================================
echo.
echo 💡 如果项目被创建在 proj 目录下，说明修复成功
echo 💡 如果项目被创建在 proj 平级，说明还有问题
echo.

pause 