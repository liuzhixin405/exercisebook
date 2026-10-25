# 团队 AI 开发工作台

## 项目命名

项目用途是按需求与团队规范，通过 AI 生成、修复、验证并审阅完整软件项目，不是游戏集合或以页面设计器为核心的低代码平台。

- 启动 API 前须通过环境变量 `Jwt__Key` 配置至少 32 字节的随机签名密钥；不要将密钥写入配置文件或提交到版本库。
- 目标工作区目录：`G:/github/ai-dev-platform`。项目内部改名已完成；若当前编辑器/终端占用 `games` 导致 `EBUSY`，请关闭 VS Code，从外部 PowerShell 执行下列迁移脚本。脚本不复制、不覆盖；改名后完整重编译、运行平台测试并打开新目录。根目录迁移仍需实际执行成功，不能把脚本存在当成已经改名。
- 解决方案：[AiDevPlatform.sln](../AiDevPlatform.sln)；项目及命名空间统一为 `AiDevPlatform.API`、`AiDevPlatform.Core`、`AiDevPlatform.Infrastructure`、`AiDevPlatform.Shared`、`AiDevPlatform.Web`。
- 页面产品名：AI 开发平台。已有任务数据目录仍沿用 `%LOCALAPPDATA%/LowCodePlatform`，数据库名、认证标识及 HTTP 客户端标记保留兼容，不因代码改名迁移或清空数据。
- 本次是名称与路径的机械重构，没有新增 .NET API。命名空间与程序集名一致性的本地官方依据为 `[来源: aspnet-core-aspnetcore-10.0#1026 p7475]`；未新增 L1/L2。目录选择与旧数据兼容策略属于本仓库设计，需复审，不由该文档证明。

```powershell
& 'G:\github\games\tools\Rename-Workspace.ps1'
```

脚本只停止命令行明确引用该工作区的 Node/.NET 项目进程，不停止共享模型，也不迁移外部任务数据。脚本的真实目录移动需在释放编辑器占用后验收；此前只做语法验证，不声称已完成迁移。

## 当前主流程

入口：`http://localhost:54820/Development`，首页自动跳转到工作台。主导航提供开发工作台、团队规范和历史应用，不再把页面设计器作为主要开发路径。

1. 新建不可覆盖的团队规范版本，填写架构/命名/错误处理/测试等要求、必需文件、禁用文本和必需验证检查。新建表单预填 `build`、`test`，检查标识不限制技术栈。
2. 创建任务，自由填写语言、框架和项目类型，绑定规范快照，可附带已有项目的文本文件。
3. AI 根据需求生成说明、待确认问题、假设、验收条件和计划文件；人工编辑说明、回答问题并确认后才能生成源码。
4. “项目”页按冻结文件计划连续生成，每次由模型返回一个完整缺失文件，Schema 限定本批文件数和允许路径，平台再次核对完整批次后保存独立源码版本。中途失败或取消应刷新确认已保存进度，可以继续缺失文件；本批被拒绝时不修改已有源码。已有文件可在项目页选择单个文件并填写修复意见，使用定点修复生成新版本；此前审阅意见与最近验证报告会进入模型上下文，其他文件不变。每个任务最多 24 个源码版本。
5. 项目页展示计划文件进度、模型生成记录、本机项目目录与文件哈希；“检查与审阅”页展示路径/大小、计划文件、必需文件和禁用文本的真实结果。外部构建、测试显示真实报告状态，未接入的检查仍为“未运行”；依赖、公共接口和业务验收需要人工核对。文件齐全不代表业务验收通过。
6. 最新版本可选择要求修改或接受。接受必须填写审阅意见、确认剩余风险、不能有硬规则失败，并且该规范要求的外部检查必须全部通过；只有满足同一门禁且已接受的版本可以下载 ZIP。

语言输入没有白名单，C#、React/Vue、Go、Rust 只是输入建议，不会强制套用固定 C# 模板。生成范围受模型能力与上下文预算限制，不保证任意语言生成的代码正确或完整。Web/API 不在宿主执行模型命令、安装依赖、编译或运行生成物；独立验证器仅在人工配置启用后，按管理员固定命令在受限容器中检查源码。不会自动提交/合并 Git 或发布项目。

Web 仅引用 Shared，`DevelopmentApiClient` 通过 HTTP 调用 API；工作流在 Core，模型调用和 JSON 存储在 Infrastructure。开发任务与历史应用是独立数据，创建旧应用仍不自动生成源码。

### 完整项目验收入口

本轮新版服务使用 Web `http://127.0.0.1:54940/Development`、API `http://127.0.0.1:54942`；原服务不变。电商任务 `a04d8bf9-4109-4fe2-9698-96cff2217c77` 从零个上下文文件创建，绑定不可跳过的 `build` / `test` 规范。数据单独保存在 `%LOCALAPPDATA%/LowCodePlatform/CommerceAcceptance`，避免两个 API 进程同时写同一份单进程 JSON 存储。

真实模型的规划初稿缺少前端、测试和启动文件，已在需求确认阶段修正为 16 文件计划，并记录业务状态、API/模块接口与验收条件。业务源码由平台调用模型生成。两文件批次曾被拒绝，精确 Schema 后仍超时；模型健康且槽位空闲，缩至单文件批次后实际保存了 6 个模型生成版本。不将接口测试成功或规划确认当作电商已交付。

**前一阶段真实结果：未完成、已驳回。** 任务修订 11，源码版本 `971d4b29-adf8-49f2-9c17-cb3057d82297`；6/16 文件（依赖清单、README、数据库、安全、商品、购物车），没有前端、订单、退款和完整测试。购物车只有占位响应且路由前缀重复；数据库偏离 `node:sqlite`、迁移事务未提交；认证接口、会话持久化和模块契约未实现；依赖版本及测试脚本也存在问题。上述静态审阅意见已通过平台 `review` 记录为 `changes-requested`，不是人工接受或测试通过。当时验证请求返回 502、没有任何验证报告，交付 ZIP 请求为 409。

**继续执行后的真实结果：仍未完成，但已取得真实容器失败报告。** 任务修订 16，累计 8 个源码版本，最新版本 `d2e2c133-66f3-4eb8-9575-c80ddca885ce`，仍为 6/16 文件。平台两次调用真实模型定点修复数据库；第一次保留未定义的 `Database`/嵌套 Promise，第二次改为 `DatabaseSync` 却仍遗漏文件名、沿用回调并生成未闭合代码。没有手工替换业务源码或导入参考电商。

Windows PATH 无 Docker，但 Ubuntu WSL2 中已有 Docker 29.8.1。复用此引擎，构建固定官方 Node 22.15.1/Express 5.1.0 的可信检查镜像；验证器在 `http://127.0.0.1:54826/verify`，本轮 API 已切至此地址。首次报告使用镜像 `sha256:4a78337829489aeea1812b72b9af783fd095b5911fc779b247772788f8ea8018`。平台保存真实报告 `49c433fe-15b8-4e60-ac80-9ccd2e19d25e`：`build=failed`，数据库第 128 行 `SyntaxError: Unexpected end of input`；`test=not-run`。容器已清理，页面显示真实错误；AI 复核为 `changes-requested`，实际版本下载路由返回 409。早期接入曾因清理超时返回 502，未伪造报告；扩大独立清理预算后成功保存失败报告。

**本轮续作验证**：平台回归 53/53 通过；另以不关联电商任务的可信夹具走通真实容器正向 `build` / `test`，其中 2 项测试核对 Express 5.1.0、非 root、根目录与源码不可写、SQLite 可写 `/tmp`，47 秒完成。第一次正向烟测因逐文件启动 Node 的开销耗尽预算，后改为容器内 V8 单进程只编译语法，不执行待检查源码。当前运行镜像已更新为 `sha256:768c5ab0faf885b2835fe2f0a310ef8e85c0da4868c75a61811efc1f9ba6ecd8`。烟测报告没有写入电商任务，也不能替代电商业务验收。当前只发现 9931 上的 Qwen3-4B；其他常见本地端口无模型服务，模型目录也只有该文件。系统空闲内存约 15.3GB，GTX 1050 Ti 4GB 显存只剩约 0.5GB，未擅自下载或另启模型。

待检查源码目录：`%LOCALAPPDATA%/LowCodePlatform/CommerceAcceptance/projects/a04d8bf941094fe2969896cff2217c77-971d4b29adf849f29c17cb3057d82297-dd625e0ed80e442daaf6bb44e1807745`。由平台 `materialize` 生成，包含 6 份原始模型产物和 `_platform/project.json`，每个文件可追溯到实际模型生成记录与字节哈希，运行状态仍为 `not-run`。这是失败尝试的可检查产物，不是完整电商或可运行交付；没有执行其安装脚本、依赖或源码。

前一阶段平台验证：Release 编译通过；`LOWCODE_TEST_CONFIGURATION=Release` 下 `node --test --test-concurrency=1 tests/api-boundary.test.cjs tests/designer.test.cjs tests/verification.test.cjs` 为 50/50 通过，包括真实临时 API/Web 进程的工作区、重启持久化、链接目录拒绝、逐文件来源与批次续作，以及前端连续请求/中断恢复模拟测试。首次并行回归的既有验证器 HTTP 用例曾出现一次 `ECONNRESET`，单独及串行完整重跑通过，未修改该验证器掩盖波动。真实电商目录 6 个文件逐字节哈希核对通过。浏览器 DOM 检查 1366px/390px 均无文档横向溢出，任务页正确显示 6/16、未运行和未通过；截图工具因显示表面不可用而未完成截图验收。平台协议测试和哈希匹配不证明生成物业务正确。

`projects/commerce` 是此前绕过平台单独编写的参考项目，不是本任务产物，未导入本任务，不能用其 17 项测试或演示流程证明 AI 平台已经生成完整电商。

### 数据与边界

- 规范和任务默认持久化到 `%LOCALAPPDATA%\LowCodePlatform\Development`，可用 `DevelopmentWorkspace__DataPath` 指定目录。以服务进程用户的目录和访问权限为准，备份应包含全部 `.standard.json` 和 `.task.json`。
- 写入使用临时文件替换、单进程锁和任务修订号冲突检查；重启后可继续处理。它不是多实例数据库或不可篡改审计日志，不提供加密或细粒度用户权限。
- 每个任务保存所选规范的完整快照；新建其他规范版本不会改变历史任务。默认上限：50 个规范、100 个任务；需求 4000 字符、规范正文 4000 字符、上下文 8 文件/合计 16000 字符。
- 单次普通变更最多 16 文件/合计 100000 字符；项目批次为一个完整缺失文件；合并快照最多 24 文件/合计 120000 字符，单文件最多 40000 字符。项目计划与必需文件合计不能超过 24 个路径。只支持文本新增/替换，不支持删除、二进制、完整仓库导入或对单文件按 token 分块生成。每批源文件上下文预算 14000 字符，只选完整文件并同时提供完整文件目录；被省略内容可能导致接口不一致，必须通过实际验证检查，不能假定模型已读取整个仓库。
- 相对路径必须通过校验，拒绝路径穿越、绝对路径、重复/文件目录冲突、Windows 设备名、`.git`、平台保留目录和非示例 `.env`。源码是不可信文本；经用户明确确认，可创建独立项目目录，但仍不执行。
- 项目目录固定在数据目录下 `projects/{taskId}-{changeId}-{workspaceId}`，客户端和模型不能指定宿主路径。逐层检查父目录并拒绝现存重解析点，文件以 `CreateNew` 独占创建，完成后发布新目录，不覆盖已有项目。`_platform/project.json` 保存任务、源码版本、规范、生成记录、检查/审阅的创建时快照及 UTF-8 字节 SHA-256；来源区分上下文、实际生成记录与无法归属的历史文件。
- 工作区与同一源码版本关联后不可重复覆盖，新源码版本另建目录。源码目录不是只读或不可篡改审计；用户本机修改不会自动回写平台，清单也不是当前磁盘内容已校验的证明。不允许不可信本机进程同时改写数据目录：逐层链接检查不等于操作系统句柄级防竞态沙箱。目录发布与任务 JSON 保存也不是跨文件事务，取消/修订冲突可能留下未关联目录，应依据清单人工核对，不自动删除用户目录。
- ZIP 包含源码和 `_platform/delivery.json`，记录需求、规范快照、澄清结果、答案、检查及审阅信息。接受审阅不代表测试通过，下载中的“未运行”仍保留。
- 禁用文本是字面子串检查，不是语义安全审计；必需文件存在也不代表其内容正确。架构规范主要依赖提示词和人工审阅，尚未接入真实架构分析器或隔离 CI。
- 澄清/生成与旧页面草稿共用单次模型请求闸门。代码生成模型超时 180 秒、Web 190 秒、浏览器 195 秒；模型输出截断、结构不合法或不可用会失败，不伪造成功。取消后应刷新确认状态再重试。
- 请勿上传凭据、个人隐私或敏感业务源码。规范、需求和源文件保存在本机 JSON 中，会作为模型上下文发送到配置的本机服务。

### 工作流接口

所有接口前缀为 `/api/development`，均限制本机 Development；客户端标记不是身份认证。

| 路径 | 行为 |
| --- | --- |
| `GET/POST standards` | 列出/新建不可覆盖的规范版本 |
| `GET/POST tasks` | 列出任务摘要/创建任务 |
| `GET tasks/{id}` | 获取需求、源码快照、检查和审阅记录 |
| `POST tasks/{id}/clarify` | 生成需求澄清草稿 |
| `POST tasks/{id}/confirm` | 人工确认需求和答案 |
| `POST tasks/{id}/generate` | 按确认需求和固定规范生成源码变更 |
| `POST tasks/{id}/generate`，`generationMode: project` | 严格生成下一批缺失计划文件，逐批保留版本；默认模式仍是 `change` |
| `POST tasks/{id}/generate`，`generationMode: repair` | 以 `targetPath` 选择已有文件、`feedback` 提供 1–2000 字符意见；目标不超过 14000 字符，严格只替换目标，新版本不继承验证或工作区；修复目标及意见保存在版本、清单和交付元数据中 |
| `POST tasks/{id}/materialize` | 以当前修订号和 `riskAcknowledged: true` 创建最新版本的独立项目目录；不执行、不自动接受 |
| `POST tasks/{id}/review` | 接受最新待审版本或要求修改 |
| `POST tasks/{id}/verify` | 请求配置的可信外部验证器验证最新待审源码 |
| `GET tasks/{id}/changes/{changeId}/download` | 下载已接受版本及交付记录 |

修改任务必须携带 `expectedRevision`，旧页面操作返回 409，不覆盖新状态。Web 的 JSON POST 仍受 Razor 防伪验证保护，错误不返回上游原始响应。正式多人协作前必须补真实身份/角色、持久数据库、审计和隔离验证，不能直接开放当前本机入口。

## 必需验证门禁

已实现**外部验证协议、放行规则和独立 Docker 执行器**。后续检查发现 Ubuntu WSL2 已有可用 Docker，已通过本轮 API 在真实受限容器中取得电商源码的语法失败报告；没有在宿主执行生成源码。完整业务测试和所有隔离故障分支尚未验收。验证器与 API 接入默认关闭；未配置时返回 503，严格规范不能通过勾选风险确认绕过。

在 API 进程环境中配置以下变量后重启，地址必须属于经过管理员审查的本机可信服务：

```powershell
$env:Verification__Enabled='true'
$env:Verification__Endpoint='http://127.0.0.1:54824/verify'
```

配置只是启用客户端，**不会启动或实现该服务**。平台禁止代理、重定向、远程地址、URL 凭据、查询参数和片段；HTTP 超时 90 秒，响应上限 64 KiB。禁止把验证地址指向模型或一个直接执行命令的通用 HTTP/Shell 服务。

### 协议

平台向配置地址 POST JSON，携带本次生成的 `runId`、`taskId`、`changeId`、自由文本 `technology`、当前完整 `files` 快照和规范冻结的 `requiredChecks`。请求不接收浏览器传来的命令、报告或成功状态；以下仅为协议示例：

```json
{
	"runId": "11111111-1111-4111-8111-111111111111",
	"taskId": "22222222-2222-4222-8222-222222222222",
	"changeId": "33333333-3333-4333-8333-333333333333",
	"technology": "Go CLI",
	"files": [{ "path": "main.go", "content": "..." }],
	"requiredChecks": ["build", "test"]
}
```

验证器必须原样回传三个 ID，每个要求的检查恰好一次，检查名大小写精确匹配。状态仅允许 `passed`、`failed`、`not-run`；`detail` 必填且最多 2000 字符，包含简短真实诊断，不应含凭据或个人信息：

```json
{
	"runId": "11111111-1111-4111-8111-111111111111",
	"taskId": "22222222-2222-4222-8222-222222222222",
	"changeId": "33333333-3333-4333-8333-333333333333",
	"checks": [
		{ "name": "build", "status": "failed", "detail": "Compiler reported invalid syntax; build failed." },
		{ "name": "test", "status": "not-run", "detail": "Build failed; tests were not started." }
	]
}
```

- 缺项、多项、重复、无效状态、畸形/过大报告、错误 ID 或重放其他请求的报告均返回 502，不写入任务。
- 合法失败或未运行报告会保存，不能接受该版；合法报告由平台标记 `receivedAt`。每个源码版本最多 5 次验证，保留全部有效记录，最新一次决定当前结果。
- 若报告返回前任务已有新修订，保存返回 409，不把过期结果挂到新版本。生成新源码会创建新的 `changeId`，不会继承旧报告；旧已接受版本的下载仍绑定旧快照。
- 生成修复时会带上上版审阅意见和最后一次验证报告，提示模型将日志当作不可信数据，不允许修改规范、伪造新版本检查成功或削弱测试来绕过门禁。
- 检查页分开展示本机规则、外部检查和验证历史；历史版本隐藏当前版验证/审阅表单。日志及源码只按文本渲染。ZIP 元数据包含验证记录。
- 旧规范和任务快照不迁移：`requiredChecks` 缺省为空，仍按旧人工审阅规则工作。清空新规范的检查清单也表示不要求外部验证，并非严格门禁；团队使用应选择包含必需检查的新版本。

### 信任边界

ID 绑定只能防止串版和协议重放，**不能证明验证器实际执行了检查或具备隔离能力**。配置该服务前必须独立审查其只读输入、临时工作区、无宿主挂载/特权、网络/资源/时间限制、依赖来源和固定命令映射。语言与检查标识映射由可信执行器管理，不能执行模型提供的任意命令；未知技术栈必须返回失败或未运行，而非退回宿主执行。

此本机开发协议尚无服务间身份认证、签名报告、执行证明或多实例队列。单纯配置一个会返回 `passed` 的服务不构成质量保障；接入生产 CI 时必须同时解决身份、审计和执行信任。真实 Docker 已走过创建、启动、源码解包、失败检查和删除；这不证明所有资源约束、取消、日志上限或容器逃逸防护均已验收。自动回归仍使用模拟 Docker 验证控制流程。

## Docker 验证器

实现位于 [tools/verification/server.cjs](../tools/verification/server.cjs)，需要 Node.js 22+、本机 Docker CLI 与 Linux 容器引擎。以下默认启动不执行源码，也不要求 Docker 已安装：

```powershell
Set-Location tools/verification
npm.cmd ci --ignore-scripts --no-fund --no-audit
node server.cjs
```

默认配置 [profiles.example.json](../tools/verification/profiles.example.json) 的 `enabled` 为 `false`，示例镜像 ID 全零且不可用于实际验证。监听地址固定为 `127.0.0.1:54824`；`GET /health` 返回 503、`status: disabled`、`runtimeChecked: false`。启用后的健康响应仅表示 `configured`，不代表 Docker、镜像或检查通过；这些在每次验证时预检。

### 管理员启用

1. 在独立、受控环境安装和审查 Docker，不自动提权安装。要求 Linux 引擎启用内置 seccomp；Windows 容器、无 seccomp 或 unconfined 配置均拒绝。
2. 准备经审查且已存在于本机的 Linux 工具链镜像，用其完整 `sha256:` 镜像 ID 固定版本。镜像必须含 `/bin/sleep`、`/bin/tar` 和所配置的绝对路径工具，不得声明 `VOLUME`。管理员在可信环境预置依赖，验证期间不拉镜像或下载依赖。
3. 使用管理员维护的配置文件，设置 `enabled: true` 与真实镜像 ID，并审查 `matchFiles`、`excludeFiles`、`environment`、`checks`。通过 `VERIFIER_CONFIG` 指向该文件后启动服务。本机配置可使用被忽略的 `tools/verification/profiles.local.json`；不要加入凭据。
4. 服务默认连接 Windows 的 `npipe:////./pipe/dockerDesktopLinuxEngine`，Linux 的 `unix:///var/run/docker.sock`。可用 `VERIFIER_DOCKER_HOST` 指定其他本机命名管道或 Unix socket；拒绝 TCP/SSH 远程 Docker 端点，忽略 Docker context 和其他 Docker 环境变量。Windows 可设置 `VERIFIER_WSL_DISTRIBUTION=Ubuntu` 显式走 `wsl.exe --distribution Ubuntu --exec docker`，此时只允许本地 Unix socket；不使用 shell 或宿主执行回退。
5. 在真实 Docker 环境独立验证成功、编译/测试失败、断连/超时、日志溢出和清理失败，以及网络、只读、用户和资源限制；确认不会触碰宿主数据后，再启用上文 API 的两个 `Verification__*` 变量。

语言生成没有白名单，但验证只能使用已审查的配置。配置按标志文件而不是自由文本 `technology` 选择，必须恰好匹配一个 profile，且包含全部必需检查；否则返回 `not-run`。命令是管理员固定的可执行文件参数数组，不使用 shell，也不接收 AI 的运行命令。增加 Rust、Node.js、.NET 等支持需要相应离线镜像和固定 profile，不修改生成器语言规则。

Go 示例仅适用于根目录单包、标准库、离线 `go.mod` 项目：`build` 输出到 `/tmp/program`，`test` 执行 `go test -count=1 .`；不代表支持多模块或任意 Go 项目。测试文件与源码都可能由 AI 生成，测试通过不能代替独立验收、覆盖率和人工审阅。

Node 电商示例的可信镜像使用 [Dockerfile.node](../tools/verification/Dockerfile.node) 和 [node-check.cjs](../tools/verification/node-check.cjs)，构建上下文通过 `.dockerignore` 排除项目源码与其他文件。镜像构建时安装固定 Express 5.1.0，不执行包安装脚本；验证时离线，不执行生成的 npm scripts。`build` 在单进程中用 V8 只编译 JavaScript 语法，不执行；后端按 CommonJS 包装，`public/` 按普通浏览器脚本处理，`.mjs` 明确拒绝。再核对本轮 16 文件计划和仅 Express 5.1.0 的依赖契约；这是解释型项目静态构建检查，不是打包或业务验收。`test` 固定调用 `node --test --test-concurrency=1 tests/store.test.cjs tests/http.test.cjs`。其他依赖或项目结构必须另建经审查的镜像/profile，不能静默降级、自动联网安装或弱化规范。最终本地镜像 ID 固定运行内容；Express 的传递依赖未单独锁定为仓库 lockfile，不声称重新构建必定得到相同镜像。

### 执行限制

- 每个检查使用新容器，前一项失败后余项 `not-run`，不复用可修改的构建产物；全部必需检查通过才可交付。
- 不挂载宿主目录、Docker socket 或卷，不使用特权；网络 `none`、只读根文件系统、删除全部 capabilities、`no-new-privileges`、非 root 的 `10001:10001` 检查用户。
- 源码以结构化 tar 流经标准输入发送，只生成普通文件，不落宿主临时源码目录。可信解包命令临时使用 root；工作区及源码由 root 拥有、文件权限 `0444`，检查用户不能修改源码或目录。工作区是 2 MiB 的 noexec tmpfs；检查输出只写 128 MiB 的 `/tmp`。
- 每容器限制 256 MiB 内存且禁止额外 swap、1 CPU、64 PID、128 文件描述符。整次 HTTP 请求预算 55 秒；容器主进程固定 65 秒寿命，即使执行器崩溃也会结束，但容器元数据仍可能遗留，需管理员审计清理。
- HTTP 请求最多 256 KiB，文件/路径限制与工作台一致；只允许本机服务端来源，拒绝浏览器来源头。单次只运行一个验证，争用返回 409；未读完的请求体也受预算约束，超时释放槽位。
- 每次 Docker 命令 stdout/stderr 合计最多 64 KiB，超限终止调用；诊断去控制字符后截为 1800 字符。日志仍是不可信文本，会回传工作台与模型，不能放入敏感信息。Docker 设置/清理失败只返回通用错误，不泄露内部输出。
- 无论检查、解包或启动是否成功，都尝试删除对应容器；清理有独立 20 秒预算（WSL 实测超过原 5 秒），清理失败不签发有效报告；必要时总响应时间会包含独立清理耗时。Docker 不可用或异常返回 503，绝不回退到宿主执行。

**隔离限制**：Docker 共享内核，守护进程通常具备高权限；以上约束不等于抵御所有恶意代码或容器逃逸。镜像、管理员配置、Docker 引擎和宿主仍是信任基础。多用户或不可信项目应使用独立执行节点、虚拟机/更强沙箱、身份认证和审计，不能把当前本机原型直接暴露到网络。

## 保留的旧功能

- 复用已运行的 llama.cpp 服务，不另启模型进程：`http://127.0.0.1:9931/v1/`，模型 `Qwen3-4B-Q4_K_M.gguf`。
- 页面设计器旧入口保留：`http://localhost:54820/Designer`，不在主导航中。
- 需求描述 -> 受 JSON Schema 约束的草稿 -> 人工预览 -> 确认替换画布 -> 撤销/重做 -> 本机草稿保存/刷新恢复。
- AI 只生成文本、输入框、按钮；布局由前端确定，不接受模型输出的脚本、SQL、HTML 或执行工具。
- 当前调用链为：浏览器 -> Web Razor 处理器 -> API `POST /api/ai/page-drafts` -> Core/Infrastructure -> 本地模型。
- Web 只引用 Shared 契约，不再引用 API、Core、Infrastructure，也不再注册数据库或业务实现；`PageDraftApiClient` 负责 HTTP 转发。`IPageDraftGenerator` 由 API 使用，其模型 HTTP 实现仍在 Infrastructure。
- 补齐 Identity 核心服务注册，解决启动时 `UserManager<User>` 无法注入的问题。
- 补齐 Razor Tag Helpers 导入；修复设计器属性值转义、组件 ID、真实输入控件、空画布撤销、预览与浏览器草稿恢复。
- 补齐应用管理菜单对应的列表、搜索、新建与编辑页面；Web 通过 API 管理应用，从列表进入设计器时校验应用并显示名称。

**旧设计器边界：只是开发环境的页面草稿闭环，不是完整表单运行时。** “提交”按钮不写入业务数据库；草稿保存在当前浏览器 localStorage，清理站点数据或更换浏览器会丢失，也没有跨用户同步。

## 配置与运行

API 的模型配置在 `src/AiDevPlatform.API/appsettings.Development.json`：`LocalAI:Enabled`、`LocalAI:BaseUrl`、`LocalAI:Model`，对应环境变量 `LocalAI__Enabled`、`LocalAI__BaseUrl`、`LocalAI__Model`。

Web 的配置在 `src/AiDevPlatform.Web/appsettings.Development.json`：`Api:BaseUrl` 默认 `http://localhost:54822/`；`UiFeatures:AiDraftEnabled` 控制界面入口。环境变量分别为 `Api__BaseUrl`、`UiFeatures__AiDraftEnabled`。API 地址应为带结尾斜杠的根地址。模型地址、模型名称和后端 Enabled 开关不再由 Web 管理；界面开关不会启用 API。

当前仅支持 HTTP loopback API/模型地址，禁用代理与重定向。Web 保留本机 Development 限制与 Razor 防伪验证。API 同时检查 Development、后端 Enabled、本机来源、本机 Host、`X-LowCode-Client: Web`，并拒绝携带 `Origin` 或 `Sec-Fetch-Site` 的浏览器直连请求。Swagger 中可查看契约，但浏览器中的 Try it out 不用于调用此开发专用入口。

**这些限制不是服务间认证**：本机非浏览器程序可以设置该客户端标记。生产环境即使启用开关仍返回 403；后续开放远程部署前必须接入真实身份、授权、限流和 TLS，不能直接删除上述检查。现有其他 API 的 AllowAll CORS 并未因本次拆分而自动收紧。

```powershell
dotnet build src/AiDevPlatform.API/AiDevPlatform.API.csproj
dotnet build src/AiDevPlatform.Web/AiDevPlatform.Web.csproj
node --test tests/designer.test.cjs tests/api-boundary.test.cjs tests/verification.test.cjs
```

在两个独立终端启动，模型服务需要事先运行：

```powershell
$env:ASPNETCORE_ENVIRONMENT='Development'
dotnet run --project src/AiDevPlatform.Web/AiDevPlatform.Web.csproj --no-build --no-launch-profile --urls http://localhost:54820
```

```powershell
$env:ASPNETCORE_ENVIRONMENT='Development'
dotnet run --project src/AiDevPlatform.API/AiDevPlatform.API.csproj --no-build --no-launch-profile --urls http://localhost:54822
```

API Swagger：`http://localhost:54822/swagger/index.html`。先启动 API，再启动 Web；没有 API 时 Web 仍可打开设计器，但 AI 生成返回 502。重新构建某个应用前停止该应用；Web 已不再间接编译 API，避免原先由项目引用带来的后端输出锁冲突。不要停止共享的 llama.cpp 服务。

旧页面草稿服务限制需求 2000 字符、输出 18 个组件、响应缓冲 256 KiB、模型 HTTP 超时 90 秒；其 Web 超时 95 秒、浏览器 100 秒。新工作台限制见上文。取消令牌贯穿 Web、API 与模型请求，异常不向浏览器返回模型或 API 原始错误内容。

## 应用管理

入口：`http://localhost:54820/Applications`。支持按名称或编码搜索，以及新建、编辑名称、唯一编码、描述和版本。编码以字母开头，仅接受字母、数字、下划线及短横线，保存时统一为大写；重复编码返回 409，并保留表单输入。表单由 Razor 防伪令牌保护，服务端和 API 都校验输入。

| API | 行为 |
| --- | --- |
| `GET /api/applications` | 当前本机开发所有者的应用列表 |
| `GET /api/applications/{id}` | 获取应用，不存在返回 404 |
| `POST /api/applications` | 新建，成功返回 201 和 Location |
| `PUT /api/applications/{id}` | 更新，成功返回 200 |

`ApplicationsApiClient` 使用与 AI 客户端相同的本机 API 地址与服务器请求头；Web 不访问数据库。应用管理不依赖 AI 开关，但 Web/API 均只允许本机 Development 使用；API 拒绝浏览器来源头。上述开发防护不替代身份认证，Production 返回 403。

开发数据归固定本机所有者 `11111111-1111-4111-8111-111111111111`，不接受请求中的 OwnerId。首次新建时补建开发所有者，并满足 EF 映射要求的 Email 字段。这不是正式用户登录或多租户实现。单进程写锁避免并发创建重复编码；迁移到持久数据库/多实例时仍需数据库唯一约束、真实身份和授权。

列表中的画笔入口携带应用 ID；设计器通过 API 校验并显示应用名称，提供返回应用管理的入口，未知或无效 ID 返回 404。草稿仍保存在浏览器，按应用 ID 和页面 ID 隔离，尚未写入页面定义 API。应用记录仍使用 EF InMemory，重启 API 会丢失；遗留的本机草稿不会因此自动删除。不包含应用删除、发布、权限管理或数据库迁移。

## 已验证

- Web、API 独立构建成功；前端测试 8 项、服务边界测试 22 项、Docker 执行器控制流程测试 16 项。后者使用模拟 Docker，不运行生成源码，不代表真实容器验证通过。
- 执行器覆盖禁用/未知配置、危险路径及命令注入输入、固定镜像/命令、每项新容器、只读源码所有权与隔离参数、失败/取消/缺失退出码拦截、创建/启动/解包与清理失败、日志上限诊断、HTTP 来源/大小/并发限制、慢请求截止和健康状态；新 npm 依赖审计为 0 个已知漏洞。Node MockTimers 测试会输出实验 API 警告。
- 新增验证测试覆盖风险勾选不能绕过必需检查、失败/未运行拦截、跨版本失效、五次上限、畸形/重复/缺项/重放/过大报告、禁止重定向、未配置和非本机端点、并发过期保存、修复反馈、Web 防伪及前端按钮状态/日志文本安全。测试使用模拟验证器，不执行生成源码。
- 前端回归包含同一任务提交后强制加载新状态，防止仅修改 URL 标签却继续显示旧修订。
- 新工作台真实 Qwen3 调用已走过创建任务、澄清、人工修订确认、生成源码和退回审阅。Go 样例生成了 4 个文件，但存在重复声明、语法和依赖违规，已保存为“要求修改”，没有放行为可下载交付；未运行其源码。不能将真实模型调用成功等同于代码质量合格，当前小模型需要更强的模型与隔离验证配合。
- 新工作台已检查桌面与 390px 手机宽度，源码保留在自身滚动区域；手机文档没有横向溢出。真实样例尚未通过人工验收，已接受 ZIP 闭环由模拟模型集成测试验证，而不是由该失败样例验证。
- 新工作流覆盖任意语言输入、规范版本冻结、确认/修订号约束、违规审阅拦截、保留源码版本、进程重启后持久化、已接受版本 ZIP 与元数据，以及危险路径、错误/截断模型响应。Web 集成覆盖 JSON 防伪、主导航、确认/生成转发、源码转义和下载拦截。
- 原设计器测试覆盖属性转义、非重叠布局、撤销重做、分页面草稿、损坏存储及拒绝恢复任意脚本/样式属性。
- 服务边界测试启动临时端口上的 Web/API 及模拟后端，不需要调用真实模型；覆盖 Web 运行依赖隔离、JSON/客户端标记转发、400/403/409/504 状态映射、上游错误内容隐藏、非法 JSON 与断连映射 502，以及防伪、非法输入、浏览器来源、Host 和生产环境拦截。测试结束后关闭临时进程。
- 应用测试覆盖真实 API 增改查、编码大小写去重和并发去重、无效字段与未知 ID、Web 表单及防伪、菜单链接、搜索、输出转义、API 故障保留输入、应用设计器 ID 校验，以及生产环境禁用。
- 浏览器实际新建并编辑了“客户管理”，确认非法编码被原生表单校验拦截，设计器携带正确应用 ID 和名称。桌面及 390px 手机宽度截图检查通过；手机编辑页无文档横向溢出，列表在表格容器内横向滚动，操作按钮不超出表格。
- 经拆分后的 Web -> API -> 现有 Qwen3 真实调用：客户登记需求生成 8 个组件，包括 3 个真实输入框，浏览器预览、应用、撤销、重做、保存、刷新恢复通过。
- 缺少防伪令牌、空需求、2001 字符需求均返回 400；API 在生产配置下即使 Enabled=true 仍返回 403。
- 390px 宽度检查未发现文档横向溢出；API Swagger HTTP 200。
- 尚未自动验证：API 到真实模型的离线/格式破坏分支、并发争用与超时取消的全部行为，以及真实远程连接的拒绝（已测外国 Host 与浏览器来源）。没有新增 C# 自动化测试项目。不能据此宣称生产就绪。

## 后续生产化

当前交付是本机工作流基础，不是已经完成的团队治理系统。优先补齐以下可验证能力，再考虑远程协作：

1. 团队身份、角色和任务/规范授权；持久数据库、多实例修订冲突及不可篡改审计。
2. 在真实容器环境验收并强化隔离执行器：已有默认关闭的 Docker 实现、固定命令、资源限制、报告记录和必需检查门禁；仍需审查镜像/依赖、验证实际隔离效果，并扩充各技术栈配置和独立验收，不采信模型自报结果。
3. 仓库接入、上下文检索、文件删除与真实差异；规范变更评审和 Git PR 门禁，禁止未经批准直接合并或发布。
4. 模型评估集、生成预算、失败重试与可观测性；敏感数据和源码保留策略。

### 遗留模块风险

| 优先级 | 现场发现 | 建议验收标准 |
| --- | --- | --- |
| P0 | EF Core 仍使用 InMemory；本轮已移除 Web 数据源注册，数据访问统一归 API，但尚无持久化 | 使用持久数据库与迁移；重启不丢数据，页面保存使用 API 而非浏览器存储 |
| P0 | 有 Auth handler，但没有完整登录入口与端到端权限闭环 | 登录/刷新/退出、应用及页面授权、租户隔离；未授权请求与跨租户请求有自动测试 |
| P0 | AutoMapper 12.0.1 构建报告 NU1903 高危公告 GHSA-rvv3-g6hj-g44x | 评估升级/替换与许可证影响，升级后通过映射回归测试；本轮未升级 |
| P1 | EF 启动有多处影子外键警告，例如 UserId1、ComponentId1 | 明确主外键类型和关系，生成真实数据库迁移并验证约束 |
| P1 | ApplicationPages 按 ID 获取仍是占位返回；设计器保存原来没有真实落库 | 页面定义服务端 CRUD、校验、版本与冲突处理；编辑、保存、重载走同一份定义 |
| P1 | 工作流启动/完成任务没有形成持久化状态推进闭环 | 状态机、事务、幂等、授权与审计；禁止 AI 绕过引擎推进状态 |
| P1 | 代码生成、数据库模型同步、邮件仍有 TODO 或未实现分支 | 明确支持范围，不对未实现能力返回成功；补相应契约与集成测试 |
| P1 | API 使用 AllowAll CORS；应用管理已补齐，隐私政策等其他路由仍有缺失 | 部署时收紧来源；补齐其他真实页面或移除无效导航，不提供空链接 |
| P2 | 缺少发布审批、回滚、监控及完整自动化验收 | 页面版本、预览/发布隔离、审计、备份恢复、CI 安全与业务回归门禁 |

另外，现有 `ApplicationPagesController` 仍有 CS8604 可空参数警告。本轮未扩展修复这些既有问题。

## 依据分级与复审

- **L0 本地官方 PDF**：`aspnet-core-aspnetcore-10.0#0247`，p1642 的命名 `AddHttpClient`、p1644-1645 的类型化客户端注册、p1647 的 `StringContent`、JSON 序列化与 `PostAsync`；`aspnet-core-aspnetcore-10.0#0554` p4650-4658 的控制器、路由、JSON 参数绑定与状态返回。这里只引用已查看到的相关块，不将整节截断后未读取的内容当作依据；项目为 net8.0，最终以当前项目编译验证兼容性。
- **L1 官方网页**：[llama.cpp server](https://github.com/ggml-org/llama.cpp/blob/master/tools/server/README.md) 支持 `response_format` / schema 与 `chat_template_kwargs.enable_thinking`；[JSON 反序列化](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/deserialization)；[SemaphoreSlim.WaitAsync](https://learn.microsoft.com/en-us/dotnet/api/system.threading.semaphoreslim.waitasync?view=net-8.0)；[IPAddress.IsLoopback](https://learn.microsoft.com/en-us/dotnet/api/system.net.ipaddress.isloopback?view=net-8.0)；[Razor 防伪](https://learn.microsoft.com/en-us/aspnet/core/security/anti-request-forgery?view=aspnetcore-8.0)；[Identity 服务](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.identityservicecollectionextensions.addidentitycore?view=aspnetcore-8.0)。这些不是可回溯 PDF 页码的 L0 依据。
- **应用管理补充 L1**：[Razor Pages](https://learn.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-8.0) 的 BindProperty、Page/RedirectToPage、路由及表单防伪；[模型验证](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-8.0) 的 Required/StringLength/RegularExpression、ModelState 和 ApiController 自动 400；[Validator.ValidateObject](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.validator.validateobject?view=net-8.0) 的全属性校验与 ValidationException。联网资料回写知识库时仍归 L1，合成页码不作为 PDF 依据。
- **L2 非官方资料**：未使用。
- **工作台补充 L1**：[File.Move](https://learn.microsoft.com/en-us/dotnet/api/system.io.file.move?view=net-8.0) 的带覆盖参数重命名；[ZipArchive](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.ziparchive?view=net-8.0) 的内存归档、CreateEntry 与释放完成归档。已先查本地库并确认相关结果不满足 L0，故使用官方网页，不冒充 PDF 页码；文件替换不等于数据库事务或多进程锁。
- **L3 / 待复审**：以下写法来自模型知识，未经任何文档逐项核实：`Uri` 地址组合、`HttpClientHandler` 的代理/重定向开关及响应缓冲上限、请求大小特性、按 `HttpRequestException.StatusCode` 分类返回错误，以及其余未单列引用的语言/API 用法。已通过整体编译和相应边界测试，但这不等于已逐行查证。业务 DTO、提示词、校验策略、来源/客户端标记检查、确定性布局及前端存储是本仓库设计，不应被解读为官方推荐的完整业务或认证方案。
- **应用管理补充 L3 / 待复审**：固定开发所有者及其初始化、仓储组合、内存数据库下的单进程去重锁、应用与设计器关联属于本地业务设计，并非官方完整应用管理或认证方案；这些边界需在持久化和多用户改造时重新审查。
- **工作台补充 L3 / 待复审**：以下写法来自模型知识，未经任何文档逐项核实：JSON 文件存储的其余 IO/序列化组合、修订冲突和状态机设计、路径/内容检查、提示词、规范快照和快照合并。它们已通过本轮编译与边界测试，但不是正式团队权限、语义架构审计或生产数据库方案。按 API 用法划分的 L0/L1 依据不覆盖这些业务结论，需人工重点复审。
- **外部验证补充依据**：`HttpDevelopmentVerifier` 的 `IHttpClientFactory`/`CreateClient`、`StringContent`/`PostAsync`/`EnsureSuccessStatusCode` 以及命名客户端注册对应 L0 `[来源: aspnet-core-aspnetcore-10.0#0247 p1642, p1647]`，已读取具体代码块；控制器动作绑定沿用 L0 `#0554 p4650-4658`。后续 Polly 等未读取部分不作为依据。没有新增 L1/L2 来源。
- **外部验证 L3 / 重点复审**：以下写法来自模型知识，未经任何文档逐项核实：新增 DTO、LINQ 名称匹配与状态门禁、报告绑定和存储组合、`Uri.TryCreate`/`IsLoopback` 的地址策略、HTTP 缓冲/代理/重定向配置、错误分类、提示词与 UI 状态。它们是本仓库业务设计，已通过编译与相应测试；不等同于官方推荐的隔离执行、服务认证或报告证明方案，需由人复审上述信任边界。
- **Docker 执行器官方依据**：[container create](https://docs.docker.com/reference/cli/docker/container/create/) 的只读、网络、资源、tmpfs、用户与安全选项；[container exec](https://docs.docker.com/reference/cli/docker/container/exec/) 的用户、工作目录、环境与标准输入；[container cp](https://docs.docker.com/reference/cli/docker/container/cp/) 所述 tmpfs 限制及通过 exec/tar 传送的替代方式。本轮没有修改 C# API 用法。
- **Docker 执行器待复审**：profile 选择、镜像信任、源码所有权、tar/Node 流处理、超时/清理组合及安全策略是本仓库设计。WSL Docker 已实际返回生成源码的语法失败并完成清理；模拟测试及这次真实失败均不能证明所有资源限制、取消行为或容器逃逸防护。
- **项目工作区 L0**：现场 `kb --coverage` 为 10 份文档、2322 个 section；复合查询对 `FileMode.CreateNew` / `Directory.Move` 未命中，因此不将其他词的高分视作这两项命中。已读取 `dotnet-csharp#0125` p742-743 的 `FileStream` / `WriteAsync` 示例、p744-745 的目录创建示例；沿用现有 JSON 与 HTTP 调用依据。
- **项目工作区 L1**：[FileMode.CreateNew](https://learn.microsoft.com/en-us/dotnet/api/system.io.filemode?view=net-8.0) 的已存在文件拒绝；[Directory.Move](https://learn.microsoft.com/en-us/dotnet/api/system.io.directory.move?view=net-8.0) 的已存在目标拒绝；[SHA256.HashData](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.sha256.hashdata?view=net-8.0) 的字节哈希；[File.GetAttributes](https://learn.microsoft.com/en-us/dotnet/api/system.io.file.getattributes?view=net-8.0) 的属性读取。L2 未使用。
- **项目工作区与分批生成 L3 / 重点复审**：以下写法来自模型知识，未经任何文档逐项核实：新增 DTO/接口/属性及其余语言/LINQ/API 组合、相对路径到目录的映射、父目录递归检查、独占写入与目录发布组合、文件来源追溯、模型记录、动态 JSON Schema、上下文选取、分批版本与前端连续请求/取消、固定错误分类。它们是本仓库设计；官方 API 文档只支持相应 API 的行为，不证明项目可构建、沙箱安全、生成质量或完整电商交付。上述本机并发写入、非事务发布和上下文省略边界需人工重点复审。
- **定点修复补充 L1**：[JsonSerializer.Serialize](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializer.serialize?view=net-8.0) 和 [Enumerable.LastOrDefault](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.lastordefault?view=net-8.0) 的官方签名；本地查询最高仅 3/4 分，不满足 L0。LastOrDefault 的本地补查晚于初次修改，不声称已完全遵循“先查后写”的顺序。修复 DTO、单目标合并、审阅反馈选择、提示词和元数据组合仍归 L3，需人工复审；未新增 L2。

上线前仍需单独审查模型调用的取消行为、畸形/截断响应、授权边界、敏感数据策略和上述 L3 项。