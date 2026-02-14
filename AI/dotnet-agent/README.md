# Dotnet Agent (参考 Gemini CLI)

这是一个极简的 `.NET`（.NET 7）示例，用来复刻该仓库中的 agent 概念，并使用本地 Ollama 模型生成回答。

结构
- `Program.cs`：最小 runner，加载 `agents` 目录下的 `.md` 定义并调用 Ollama。
- `AgentDefinition.cs`：agent DTO。
- `AgentLoader.cs`：从 Markdown frontmatter 解析 agent（YAML 前置块）。
- `OllamaClient.cs`：调用本地 Ollama 的 HTTP 或 CLI。
- `agents/`：示例 agent 定义（请创建）

快速开始

1. 安装 .NET 7 SDK

2. 安装并运行 Ollama，本机模型示例：
   - 请参考 https://ollama.com 文档
   - 确保 Ollama 可通过 HTTP (默认端口可能为 11434) 或 CLI `ollama` 可用

3. 在 `dotnet-agent/agents` 下创建 agent 文件，例如 `generalist.md`（示例见下）

4. 构建并运行：

```bash
cd dotnet-agent
dotnet build
dotnet run --project DotnetAgent.csproj
```

示例 agent (`dotnet-agent/agents/generalist.md`)

---
name: generalist
display_name: Generalist Agent
model: llama2
---
You are a helpful assistant. Answer concisely.

然后在运行时输入提示。

注意
- 这是一个起点和参考实现；如果要完整复刻 `gemini-cli` 的功能（工具注册、调度、hooks、subagents 等），需要继续扩展。
- Ollama 的 HTTP API 路径和 CLI 参数可能随版本不同，请根据本地 Ollama 实际接口调整 `OllamaClient`。
