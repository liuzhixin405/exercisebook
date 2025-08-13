@echo off
echo ========================================
echo    ReAct智能体测试
echo ========================================
echo.

echo 正在运行测试...

dotnet run --configuration Release -- "%~dp0"

echo.
echo 测试完成
pause 