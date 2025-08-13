@echo off
echo 正在构建项目...
dotnet build --configuration Release
if %errorlevel% neq 0 (
    echo 构建失败
    pause
    exit /b %errorlevel%
)

echo.
echo 正在运行测试...
echo 请确保Ollama服务正在运行，并且已下载qwen2.5-coder:7b模型
echo.
echo 测试步骤：
echo 1. 程序启动后，输入项目目录路径（或使用当前目录）
echo 2. 输入测试任务，例如：
echo    - "创建一个Web项目"
echo    - "创建一个Python机器学习项目"
echo    - "创建一个React应用"
echo.
echo 按任意键开始运行程序...
pause >nul

dotnet run --configuration Release