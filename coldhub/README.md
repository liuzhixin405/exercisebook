# coldhub：冷链仓库温度监控（一个最小的"采集 → 分发 → 落盘 → 下载 → 后台管理"系统）

一个能跑的最小实现：7 个小项目，约 2400 行 C#，另有约 330 行前端。代码里的注释解释了每个设计为什么这样写，
适合通过运行、调试、改几处代码来掌握这类系统的技术细节。

> 业务设定：几个**仓库**（Source）里有若干**冷库**（Device），每个冷库有几个**温度探头**（Probe）。
> 传感器网关把读数发到系统里，客户端实时订阅，事后下载历史，管理员在后台管理源和访问密钥。

## 一、名词

| 名词 | 含义 |
|---|---|
| 源 `Source`，如 `warehouse-a` | 一个数据来源（一个仓库的传感器网关）。注册后名字永不复用，退役也不行 |
| 设备 `Device`，如 `freezer-1` | 源下面被订阅的对象（一个冷库） |
| 读数 `Reading` | 一个冷库上所有探头在同一时刻的温度。一条就是完整状态，新的整体覆盖旧的 |
| 探头 `Probe(id, 尾数)` | 温度 = 尾数 × 10^指数。整数加指数，避免浮点误差 |
| 通道 `Reading:{Source}:{Device}` | Redis pub/sub 的通道名，每个冷库一个 |
| 连接器 `sine` / `random` | 一种数据来源的接入方式。`sine` 永远合法，`random` 会周期性出故障 |

## 二、架构

```
 ┌──────────┐  ① 取配置/热更新(ETag)     ┌─────────┐   ④ 取 key 哈希(5s 轮询)
 │ Source×N │ ◀──────────────────────── │  Admin  │ ◀───────────────────────┐
 │ 一个进程   │                            │ 注册表   │                         │
 │ 一个源    │                            │ 配置     │                    ┌────┴────┐
 └────┬─────┘                            │ key 哈希 │                    │   Hub   │ ──▶ 客户端
      │ ② 校验→编码→PUBLISH               └────▲────┘                    │ /ws     │   (WebSocket,
      ▼   Reading:{源}:{设备}                   │ Admin key              │ /history│    二进制帧)
 ┌──────────┐ ③ 订阅 Reading:*  ┌──────────┐ ┌──┴──────┐ 写 {源}/{日期}.csv.gz  └────▲────┘
 │  Redis   │ ────────────────▶ │ Recorder │ │ Console │ ─────▶ history/ ───────────┘ 读文件
 │          │ ◀─ Hub 按通道订阅   └──────────┘ └──▲──────┘
 └──────────┘                                    │ 登录 Cookie
                                              浏览器（后台管理）
```

| 项目 | 职责 |
|---|---|
| `Shared` | 二进制编解码、通道名、API key 哈希、健康检查与指标 |
| `Admin` | 源注册表、各源配置、API key（只存哈希）、两级权限；只是个 API |
| `Source` | 一个进程一个源；连接器 → 校验 → 编码 → 发布 |
| `Recorder` | 订阅全部读数，批量追加成历史文件 |
| `Hub` | key 校验、WebSocket 扇出、历史下载 |
| `Console` | **后台管理**：人登录（Cookie），代理 Admin；网页管理源和 key，看各进程健康 |
| `Client` | 订阅并用同一份编解码器解码的命令行客户端 |

为了保持最小，没有拆成领域层/应用层/基础设施层，每个宿主里只用文件夹区分；
但"端口在内层、实现在外层"的做法保留了：`Source` 里的 `IReadingPublisher`（端口）与 `ValkeyPublisher`（实现）。

## 三、运行

需要：.NET 10 SDK；本机 Redis（或兼容的 Valkey）监听 `127.0.0.1:6379`，无密码；首次还原包需要联网。

**一键启动 / 停止**（PowerShell，每个服务一个独立窗口，关窗口即停止）：

```powershell
dotnet build ColdHub.slnx
.\run-all.ps1
.\stop-all.ps1
```

启动后打开 **http://localhost:5340** 进入后台管理。

**一键验证**（启动全部进程 → 四十多项检查 → 全部结束，不留后台进程，约 1 分钟，Git Bash 里运行）：

```bash
./smoke.sh
```

**手动运行**（每个进程一个终端，在对应目录里 `dotnet run`；也可以在 Visual Studio / VS Code 里分别启动并下断点）：

```bash
cd src/Admin    && dotnet run                      # :5310  先起，其他都依赖它
cd src/Source   && Source__Name=warehouse-a Source__Connector=sine   ASPNETCORE_URLS=http://localhost:5321 dotnet run
cd src/Source   && Source__Name=warehouse-b Source__Connector=random ASPNETCORE_URLS=http://localhost:5322 dotnet run
cd src/Recorder && dotnet run                      # :5330
cd src/Hub      && dotnet run                      # :5410
cd src/Console  && dotnet run                      # :5340  后台管理
cd src/Client   && dotnet run -- ws://localhost:5410/ws demo-client-key Reading:warehouse-a:freezer-1,Reading:warehouse-b:dock-1
```

- 后台管理的账号：`alice / alice-demo-pass`（admin，可改）、`bob / bob-demo-pass`（viewer，只读）。
- `src/Source/Properties/launchSettings.json` 里已有两个 profile（warehouse-a / warehouse-b），在 Visual Studio 里直接选。
- Admin 的数据文件 `admin-data.json`、历史目录 `history/` 都在运行时生成，已被 `.gitignore` 忽略。
- demo 里的 key 和密码（`demo-service-key`、`demo-admin-key`、`demo-client-key` 等）写在各 `appsettings.json` 里，**仅供本地演示**；
  真实部署里任何密钥都不应进仓库，应由环境变量或密钥管理提供。
- 连接串用 `127.0.0.1` 而不是 `localhost`：在作者的机器上 `localhost` 会让 StackExchange.Redis 连不上。
- `.ps1` 文件必须是带 BOM 的 UTF-8，否则 Windows PowerShell 5.1 会把中文读坏、报语法错误。

下载历史（先让 Source 和 Recorder 跑一会儿；`<日期>` 是 UTC 日期，格式 `yyyy-MM-dd`）：

```bash
curl -H "Authorization: ApiKey demo-client-key" -o a.csv.gz "http://localhost:5410/history/warehouse-a/<日期>"
curl -H "Authorization: ApiKey demo-client-key" -o f.csv.gz "http://localhost:5410/history/warehouse-a/<日期>?devices=freezer-1&format=csv.gz"
```

文件是 gzip，解压后是 CSV（Git Bash 用 `zcat`，Windows 用 7-Zip）。用 Postman 下载要点：Headers 里加 `Authorization: ApiKey demo-client-key`（不是 Bearer），
用 **Send and Download** 保存，扩展名保持 `.csv.gz`。

## 四、按技术点读代码

### 1. 二进制线上格式 —— `Shared/ReadingCodec.cs`

手写的 SBE 风格格式：消息头（`blockLength/templateId/schemaId/version`）、定长块、重复组、变长字符串，全小端，用 `BinaryPrimitives`。

- 解码用头里的 `blockLength` 而不是常量去找重复组：新版本在定长块后追加字段，旧读者仍能读（向前兼容）。
- 收到的字节是别人发的，不可信：长度不够就抛 `FormatException`，不会越界。
- 编码器拒绝不合法的数据：探头编号必须严格递增，低于绝对零度（-273.15℃）的读数一定是传感器故障。

### 2. 一进程一源 + 连接器目录 —— `Source/Connectors/Connectors.cs`、`Source/SourceOptions.cs`

`Source__Name` 选源，`Source__Connector` 选连接器，名字写错启动即失败。要多接一个源就多起一个进程，不在一个进程里塞多个。

### 3. Options 校验、启动即失败 —— 各 `*Options.cs` 与 `Program.cs`

`[OptionsValidator]`（源码生成）加 `ValidateOnStart()`：配置缺失在启动时就崩，不是等用到才发现。把 `Source__ServiceKey` 设成 3 个字符试试。

### 4. 配置热更新（ETag 轮询）—— `Source/Configuration.cs`、`Admin/Program.cs`

Source 每 2 秒带 `If-None-Match` 问 Admin，没变回 304。Admin 不可用时沿用上一份配置继续发布，并只记一条 Warning、恢复时一条 Information。

- 实验：在后台管理里编辑 `warehouse-a` 的设备或间隔，Source 不重启，约 2 秒内日志出现新配置。

### 5. 发布前的校验、限流、"只记一次"日志 —— `Source/Publishing/ReadingPublisher.cs`

`Publish` 永不抛异常。三类没发出去的数据各有各的处理（见类注释），并用 `coldhub_source_readings_dropped{reason}` 计数。

- 看 `warehouse-b`（`random` 连接器）：故障成段出现，日志是"开始一条、恢复一条"，而不是每次一条。
- 限流 `MaxPerSecond`：每个设备每秒最多 N 条，太快的直接跳过，不延后补发。

### 6. 发后不管 + 积压保护（滞回）—— `Source/Valkey/ValkeyPublisher.cs`、`Source/Publishing/PublishBacklog.cs`

`FireAndForget` 省掉每次往返，代价是断线时命令被静默丢；所以先查连接、再查积压，超限就丢并计入 `unreachable`。
积压用高低两条线（5000 / 500），避免在阈值附近抖动。

- 实验：停掉 Redis，Source 记**一条** Warning；启动 Redis，记**一条** Information，`/health/ready` 先 503 后 200。（本 demo 没有自动化验证这一项，是读代码得出的预期。）

### 7. 一个通道只订阅一次的扇出 + 慢客户端隔离 —— `Hub/SubscriptionHub.cs`、`Hub/ClientSession.cs`

- `SubscriptionHub`：不管多少客户端订阅同一通道，只向 Redis 订一次；第一个来订、最后一个走退订。热路径用不可变数组（写时复制），不加锁。
- `ClientSession`：每客户端一条容量 10 的有界队列，满了**丢最旧的**；所有发送走同一个发送循环（WebSocket 同一时刻只能有一个发送）。
- 实验：Client 连 `ws://localhost:5410/ws?delayMs=1000`（服务端每发一条停 1 秒，模拟慢客户端）：它丢数据，别的客户端不受影响；
  看 `localhost:5410/metrics` 的 `coldhub_hub_messages_dropped`。`smoke.sh` 的 [2b] 验证了这一点。

### 8. API key：只存哈希、两级权限、吊销生效 —— `Admin/Access.cs`、`Shared/ApiKeys.cs`、`Hub/KeyDirectory.cs`

- 创建 key 时明文只在响应里出现一次，库里只有 SHA-256。Hub 轮询取哈希集合，在内存里校验。
- Admin 有两个策略：`Service`（程序之间）和 `Admin`（改配置、建 key）；每个端点都点名一个策略。service key 建 key 返回 403。
- 认证头必须是 `Authorization: ApiKey xxx`，`Bearer` 一律 401。
- 实验：在后台管理里建一把 key → 最多 5 秒后 Hub 认它 → 吊销 → 再等最多 5 秒失效（Hub 是轮询，所以吊销有延迟）。`smoke.sh` 的 [4] 走了一遍。
- 踩坑记录：`AddHttpClient<T>` 会把 `T` 注册成**瞬态**。`KeyDirectory` 既是后台轮询、又被端点注入，必须是同一个实例，所以改成命名客户端 + 单例。

### 9. 落盘：批量、重试、一条日志 —— `Recorder/RecorderWorker.cs`、`Recorder/HistoryFile.cs`

订阅回调只解码入队，不做 IO；独立循环每秒批量追加。写失败的批次留到下一轮，故障只记一条 Warning、恢复一条 Information。

- 文件格式：每批压成一个 gzip"成员"追加到当天文件末尾——**首尾相连的 gzip 仍是合法的 .gz**，追加不用改写旧内容，进程被杀也不会弄坏之前的数据。
- 先在内存里压好，再一次追加到文件：写失败时文件里不会留下半个成员。

### 10. 历史下载与输入校验 —— `Hub/Endpoints.cs`

`source`、`date`、`devices` 会拼进文件路径，所以先校验（名字用白名单字符，日期严格解析），`../` 直接 400。
不带 `devices` 原样发文件；带了就解压 → 过滤 → 再压。

- 踩坑记录（代码注释里也写了）：Kestrel 禁止同步写响应，`StreamWriter` / `GZipStream` 的 `Dispose` 会同步写出最后的数据，必须 `leaveOpen` 并显式 `FlushAsync` / `DisposeAsync`。

### 11. 健康检查与指标 —— `Shared/ServiceDefaults.cs`、`Shared/Valkey.cs`

- `/health/live` 只表示进程活着；`/health/ready` 汇总各依赖自己注册的检查（Redis、Admin 配置、key）。
- 指标用 `System.Diagnostics.Metrics` 加 `IMeterFactory`，Meter 名 `ColdHub.<Area>`，指标名 `coldhub.<area>.<thing>`，标签低基数。`/metrics` 用 `MeterListener` 手写输出，看清指标怎么流出来。
- 踩坑记录：监听器必须在各 Meter 开始计数前创建（所以随宿主启动，不等第一次抓取）；`int` 类型的 gauge 要单独注册回调；
  计数器要预先 `Add(0)`，否则第一条序列要等第一次增量才出现。

### 12. 后台管理：登录、转发、权限 —— `Console/Program.cs`、`Console/wwwroot/`

Admin 只是个 API，人用的后台是 Console：浏览器 ↔ Console 用**登录 Cookie**，Console ↔ Admin 用**管理员 key**，两套凭据互不通用。

- Admin key 只存在于 Console 的服务端配置里，浏览器拿不到；响应里也不会带出来（`smoke.sh` 检查了）。
- Cookie：`HttpOnly`（脚本读不到）、`SameSite=Strict`（别的站点发起的请求不带它）。接口未登录直接回 401，不跳转登录页。
- 两个角色：`viewer` 只读，`admin` 可改。每个接口都点名一个策略（`Read` / `Write`）；查看 key 列表也只给 admin。
- 转发是**白名单**：`Forward` 只转发 Program.cs 里列出的路径，其余的到不了 Admin。写请求必须是 `application/json`，否则 415。
- 登录失败不区分"没有这个用户"和"密码错"，密码用固定时间比较。
- 网页是纯静态文件（`wwwroot`，无框架无构建）：服务器返回的文字一律用 `textContent` 放进页面，不拼 HTML；响应带 CSP `default-src 'self'`，不允许内联脚本。
- 新建 key 的明文只在创建那一次显示，关掉就看不到了（库里本来就没有）。
- 页面：**源**（列表、编辑配置、注册、退役）、**API key**（列表、创建、吊销）、**健康**（每 3 秒探测各进程的 `/health/ready`）。

### 13. 工程约定 —— 根目录文件

`Directory.Build.props`（警告当错误）、`Directory.Packages.props`（包版本只写一处）、`nuget.config`（包源映射）、`.slnx`。

## 五、故意没做的

| 没做 | 说明 |
|---|---|
| 真正的登录体系（企业统一登录）、更细的角色 | demo 用配置里的两个账号、两个角色 |
| 多地区部署、跨地区转发 | 部署层面的事，不是代码 |
| 真正的数据库（关系库存配置、时序库存历史、对象存储） | 用 JSON 文件和 `.csv.gz` 代替，上层机制不变 |
| 按时间窗口聚合（比如每分钟最高/最低/平均）与重启后接续 | 业务较重 |
| 读数监控与告警（设备静默检测、告警分级） | 业务较重 |
| 错误日志推送到聊天工具 | 只是个日志提供者 |
| 指标导出到监控系统、容器镜像发布、CI | 与业务代码无关 |
| 设备连接层（多连接分片、重连退避、订阅限速） | 本 demo 的连接器是模拟的 |

## 六、可以动手改的练习

1. 加第三个连接器 `steady`（恒定 4℃）：在 `ConnectorCatalog` 登记一行，在后台管理里注册源，再用 `Source__Connector=steady` 起一个新进程。
2. 给 `Reading` 加一个字段（比如湿度）：加在定长块**末尾**并增大 `FixedBlock`，再用旧版 Client 去读——应当仍能解码（这就是 `blockLength` 的用处）。
3. 给 `/ws` 加按源的权限：一把 key 只能订阅某些源（想一想 Admin 下发的哈希集合要怎么变）。
4. 把 `ClientSession.Capacity` 改成 1 或 1000，对比慢客户端丢数据的速度。
5. 给 Console 加一个"最新读数"页：让 Hub 暴露各设备最近一条读数，在页面上画出各冷库当前温度。
6. 给写操作加操作记录（谁在什么时候改了什么配置），并在后台管理里加一页查看。
7. 在 `ReadingPublisher.Publish` 里让 `_publisher` 抛异常（模拟 Redis 故障），观察日志、`unreachable` 计数和 `/health/ready`。
