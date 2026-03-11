# Dotnet Agent (参考 Gemini CLI)

这是一个极简的 `.NET`（.NET 7）示例，用来复刻该仓库中的 agent 概念，并使用 Deepseek HTTP API 生成回答。

结构
- `Program.cs`：最小 runner，加载 `agents` 目录下的 `.md` 定义并调用 Deepseek。
- `AgentDefinition.cs`：agent DTO。
- `AgentLoader.cs`：从 Markdown frontmatter 解析 agent（YAML 前置块）。
- `OllamaClient.cs`：调用 Deepseek HTTP API（类名为 `DeepseekClient`）。
- `agents/`：示例 agent 定义（请创建）

快速开始

1. 安装 .NET 7 SDK

2. 使用 Deepseek HTTP API：
   - 请参考 Deepseek 的文档或服务方说明
   - 你可以在 `agents/generalist.md` 前置 YAML 中填写 `deepseek_api_key` 与 `deepseek_base_url`，或通过环境变量 `DEEPSEEK_API_KEY`、`DEEPSEEK_BASE_URL` 提供。

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
model: deepseek-chat
deepseek_api_key: ""
deepseek_base_url: ""
run_config:
   maxTurns: 5
   maxTimeMinutes: 2
---
You are a helpful assistant. Answer concisely.

然后在运行时输入提示。

注意
- 这是一个起点和参考实现；如果要完整复刻 `gemini-cli` 的功能（工具注册、调度、hooks、subagents 等），需要继续扩展。
 - Deepseek 的 HTTP API 结构或返回值可能与其他服务不同，请根据你的 Deepseek 服务调整 `OllamaClient`/`DeepseekClient`。
