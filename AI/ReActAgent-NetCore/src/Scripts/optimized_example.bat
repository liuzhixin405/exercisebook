@echo off
echo ========================================
echo ReActAgent-NetCore 优化示例
echo ========================================
echo.
echo 本示例展示了如何通过MCP服务优化项目，减少模型调用次数
echo.
echo 优化点：
echo 1. 利用MCP filesystem服务执行文件系统操作
echo 2. 利用MCP fetch-server服务获取网络信息
echo 3. 利用MCP mysql服务执行数据库操作
echo 4. 减少直接调用模型执行简单任务的次数
echo.
echo 使用方法：
echo 1. 确保MCP服务已启动
echo 2. 运行主程序：dotnet run
echo 3. 在任务描述中明确指出使用MCP服务
echo.
echo 示例任务：
echo "- 使用MCP filesystem服务创建一个Web项目结构"
echo "- 使用MCP fetch-server服务查询React项目创建步骤"
echo "- 使用MCP mysql服务初始化数据库"
echo.
echo 按任意键退出...
pause >nul