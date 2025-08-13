@echo off
echo 正在测试默认proj目录功能...
echo.

echo 1. 清理之前的测试目录（如果存在）
if exist proj rmdir /s /q proj

echo 2. 启动程序并使用默认proj目录
echo 按回车键选择默认proj目录...
echo.
timeout /t 2 /nobreak >nul

echo 正在启动程序...
dotnet run --configuration Release

echo.
echo 测试完成！
echo 检查是否创建了proj目录以及其中的文件
dir proj