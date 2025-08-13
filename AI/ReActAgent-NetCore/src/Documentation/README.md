# ReAct智能体 - .NET Core版本

这是一个基于ReAct模式的AI智能体，使用本地Ollama模型来执行各种编程任务。

## 功能特性

- 🤖 基于ReAct（Reasoning + Action）模式的智能体
- 🏠 本地运行，无需网络连接（需要Ollama）
- 🔧 支持多种工具操作：
  - 读取文件内容
  - 写入文件内容
  - 执行终端命令
  - 列出目录内容
  - 搜索文件
  - 创建目录
- 📁 智能路径处理，所有文件操作都相对于指定的项目目录
- ⚙️ 可配置的模型和参数
- 🛡️ 安全检查机制（终端命令需要用户确认）

## 系统要求

- .NET 8.0 或更高版本
- Ollama服务（https://ollama.ai）
- 本地模型（推荐 qwen2.5-coder:7b）

## 安装步骤

1. 安装 .NET 8.0 SDK
2. 安装 Ollama（从 https://ollama.ai 下载）
3. 下载推荐模型：
   ```bash
   ollama pull qwen2.5-coder:7b
   ```

## 使用方法

### Windows系统：

直接运行 `start.bat` 文件（中文界面）或 `start_en.bat` 文件（英文界面）
也可以使用PowerShell脚本：`start.ps1`（中文界面）或 `start_en.ps1`（英文界面）

### 其他系统或手动运行：

1. 构建项目：
   ```bash
   dotnet build --configuration Release
   ```

2. 运行项目：
   ```bash
   dotnet run --configuration Release
   ```

## 配置说明

配置文件：`appsettings.json`

```json
{
  "Ollama": {
    "BaseUrl": "http://localhost:11434",  // Ollama服务地址
    "Model": "qwen2.5-coder:7b"           // 使用的模型
  },
  "Agent": {
    "DefaultProjectDirectory": ""         // 默认项目目录（可选）
  }
}
```

如果未指定项目目录，程序将创建并使用名为`proj`的默认目录。

## 使用示例

1. 启动程序后，输入项目目录路径（或按回车使用默认的'proj'目录）
2. 输入任务描述，例如：
   - "列出当前目录的所有文件"
   - "读取Program.cs文件的内容"
   - "创建一个名为test.txt的文件，内容为'Hello World'"
   - "编译当前项目"
   - "创建一个新的Web项目"
   - "创建一个Python机器学习项目"
   - "创建一个React应用"
   - "根据我的需求创建一个项目结构"
   - "匹配适合区块链开发的项目模板"

## 安全说明

- 执行终端命令前会要求用户确认
- 文件读取限制为1MB以内
- 搜索结果和命令输出有数量限制

## 路径处理

系统现在支持智能路径处理：

1. 所有文件操作都相对于启动时指定的项目目录
2. 支持相对路径和绝对路径
3. 相对路径会自动解析为相对于项目目录的绝对路径
4. 避免了文件被错误地创建在系统根目录或其他不正确的位置

例如，如果项目目录是 `/home/user/myproject`：
- 使用相对路径 `src/main.py` 会解析为 `/home/user/myproject/src/main.py`
- 使用绝对路径 `/tmp/test.txt` 会保持不变

## 自定义模板

系统支持自定义项目模板：

1. 在程序目录下创建`Templates`文件夹
2. 在该文件夹中创建JSON格式的模板文件
3. 模板文件格式示例：

```json
{
  "Name": "Custom Project Template",
  "Description": "自定义项目模板",
  "Directories": [
    "src",
    "docs"
  ],
  "Files": [
    {
      "Path": "README.md",
      "Content": "# Custom Project\n\n## 项目说明",
      "Description": "项目说明文件"
    }
  ]
}
```

模板文件名（不含扩展名）将作为模板的标识符使用。

## 故障排除

1. 如果遇到连接错误，请确保Ollama服务正在运行
2. 如果模型响应不理想，请尝试更换其他模型
3. 如果遇到权限问题，请检查文件和目录权限