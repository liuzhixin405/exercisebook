# .NET Core Agent

这是一个基于.NET 8.0的ReAct Agent实现，使用本地Ollama的qwen2.5-coder:7b模型。

## 功能特点

- 基于ReAct框架（Reasoning + Action）实现
- 支持本地Ollama模型调用
- 提供文件读写、终端命令执行等工具
- 支持交互式任务执行

## 环境要求

- .NET 8.0 SDK
- Ollama (需要安装并运行qwen2.5-coder:7b模型)

## 安装Ollama和模型

1. 下载并安装Ollama: https://ollama.com/download
2. 拉取qwen2.5-coder:7b模型:
   ```
   ollama pull qwen2.5-coder:7b
   ```

## 构建和运行

1. 进入项目目录:
   ```
   cd Agent
   ```

2. 还原NuGet包:
   ```
   dotnet restore
   ```

3. 构建项目:
   ```
   dotnet build
   ```

4. 运行Agent:
   ```
   dotnet run <项目目录路径>
   ```

例如:
```
dotnet run "C:\MyProject"
```

## 使用方法

运行后，系统会提示输入任务。Agent会根据任务内容进行思考和行动，使用可用工具来完成任务。

## 可用工具

- `read_file(filepath)`: 读取文件内容
- `write_to_file(filepath, content)`: 将内容写入文件
- `run_terminal_command(command)`: 执行终端命令

## 系统提示模板

Agent使用系统提示模板来指导模型行为，模板中包含了任务说明、格式要求、示例和可用工具列表。

## 注意事项

- 确保Ollama服务正在运行
- 对于敏感操作，系统会提示确认
- 文件路径建议使用绝对路径