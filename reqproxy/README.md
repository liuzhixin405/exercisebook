# transferreqproxy

一个用来理解“网关转发请求”的小 demo，照着 QuoteHub 的 Console → Admin 转发写的：人在网关登录，网关替这个人去调后端，
后端只认网关的 key，不认浏览器的 Cookie。

```
浏览器 / curl ──Cookie──▶ Gateway :5100 ──ApiKey + X-Demo-User/Role──▶ Backend hk :5200
                          （登录、YARP 转发）                    └────▶ Backend ny :5201
```

## 目录

| 路径 | 是什么 | 对应 QuoteHub |
|---|---|---|
| `src/Gateway/` | 转发服务：登录（Cookie）、按区域转发（YARP） | `QuoteHub.Console`，`Regions/RegionProxy.cs` |
| `src/Backend/` | 实际服务：只有 API，用 key 鉴权，按角色判断能不能改 | `QuoteHub.Admin`，`Access/KeyAuthenticationHandler.cs` |
| `demo.sh` | 一遍跑完所有场景的 curl 脚本 | |
| `TransferReqProxy.slnx` | Visual Studio 解决方案 | |
| `TransferReqProxy.slnLaunch` | VS 多项目启动配置：一键调试 Backend hk 和 Gateway | |

两个服务各只有一个 `Program.cs`，从上往下读即可。

## 环境要求

- .NET 10 SDK（`dotnet --list-sdks` 能看到 `10.0.x`）。
- 第一次编译要从 nuget.org 下载 `Yarp.ReverseProxy`、`Microsoft.AspNetCore.OpenApi`、`Swashbuckle.AspNetCore.SwaggerUI`。
- 跑 `demo.sh` 需要 bash 和 curl（Windows 上用 Git Bash）。

## 快速开始

在仓库根目录开三个终端：

```bash
dotnet run --project src/Backend --launch-profile "Backend hk"   # http://localhost:5200
dotnet run --project src/Backend --launch-profile "Backend ny"   # http://localhost:5201
dotnet run --project src/Gateway                                 # http://localhost:5100
```

再开一个终端：

```bash
bash demo.sh
```

三个服务的端口、区域和 key 都写在各自的 `Properties/launchSettings.json` 里。不要用 `dotnet bin/.../Gateway.dll` 在仓库
根目录直接启动：那样读不到项目目录里的 `appsettings.json`，Gateway 没有区域，所有转发都是 404。

## 账号、区域和 key

登录账号（写死在 `src/Gateway/Program.cs` 的 `people`）：

| 邮箱 | 密码 | 角色 | 能做什么 |
|---|---|---|---|
| `viewer@demo.local` | `viewer-pass` | viewer | 只能读 |
| `ops@demo.local` | `ops-pass` | operation | 能改深度，不能改 connector |
| `tech@demo.local` | `tech-pass` | technical | 都能改 |

区域（`src/Gateway/appsettings.json` 的 `Regions`）：

| 代码 | Backend 地址 | key |
|---|---|---|
| `hk` | `http://localhost:5200` | `hk-key-0123456789` |
| `ny` | `http://localhost:5201` | `ny-key-9876543210` |

每个 Backend 只认自己区域的 key（`Backend:ApiKey`）。Gateway 里的 key 和 Backend 的 key 必须一致，改一边要改另一边。
两个 Backend 的数据都在内存里，启动时都是 `Binance`（深度 10）和 `Kraken`（深度 5），版本 1；重启就恢复。

## 接口

### Gateway（:5100）

| 方法 | 路径 | 要登录 | 说明 |
|---|---|---|---|
| POST | `/login` | 否 | body `{"email":"...","password":"..."}`，成功后写 Cookie `demo-session` |
| POST | `/logout` | 否 | 清掉 Cookie |
| GET | `/api/me` | 是 | 当前登录的人和角色 |
| GET | `/api/regions` | 是 | 区域列表 |
| GET | `/api/regions/{区域}/whoami` | 是 | 转发到 Backend `/api/whoami` |
| GET | `/api/regions/{区域}/sources` | 是 | 转发到 Backend `/api/sources` |
| GET、PUT | `/api/regions/{区域}/sources/{name}` | 是 | 转发到 Backend `/api/sources/{name}` |

只有最后三行会被转发，它们来自 `RegionProxyConfig.Forwarded` 白名单。不在白名单里的路径是 404，路径对但方法不对是 405，
都不会到达 Backend。

### Backend（:5200 / :5201）

每个请求都要带：

```
Authorization: ApiKey <本区域的 key>
X-Demo-User: <邮箱>
X-Demo-Role: viewer | operation | technical
```

| 方法 | 路径 | 角色 | 说明 |
|---|---|---|---|
| GET | `/api/whoami` | 任意 | 返回 Backend **实际收到**的东西：人、角色、有没有收到 Cookie、`X-Demo-*` 头 |
| GET | `/api/sources` | 任意 | 源列表 |
| GET | `/api/sources/{name}` | 任意 | 一个源 |
| PUT | `/api/sources/{name}` | operation、technical | body `{"depth":20,"expectedVersion":1,"connector":null}` |

PUT 的返回：

| 情况 | 状态码 |
|---|---|
| 保存成功，版本加 1 | 200 |
| viewer 来改 | 403 |
| 改了 `connector`，但不是 technical | 403 |
| `expectedVersion` 不是当前版本（别人先改了） | 409，返回当前状态 |
| 源不存在 | 404 |

## 一次转发里发生了什么

以 `PUT http://localhost:5100/api/regions/hk/sources/Binance` 为例：

1. **认证**：`UseAuthentication` 从 Cookie 认出这个人和角色。没登录的话，路由的 `CanView` 策略直接返回 401，不转发。
2. **匹配路由**：YARP 找到 `hk` 区域的路由，目标是 cluster `hk`，也就是 `http://localhost:5200`。
3. **改路径**：去掉 `/api/regions/hk`，再加上 `/api`，得到 `/api/sources/Binance`。
4. **改请求头**（`AddRequestTransform`）：
   - 删掉客户端自己带的 `X-Demo-*` 头，防止伪造角色；
   - 删掉 Cookie；
   - 换上 `Authorization: ApiKey hk-key-0123456789`；
   - 加上登录人的 `X-Demo-User` 和 `X-Demo-Role`。
5. **Backend 鉴权**：`KeyHandler` 核对 key，再从两个头里读出人和角色。
6. **Backend 判断权限**：`CanEdit` 策略、connector 检查、版本检查。
7. **响应原样返回**，只删掉 `Set-Cookie`。

Gateway 只检查“有没有登录”，能不能改由 Backend 决定。这和 QuoteHub 一样：Console 所有转发路由只要求能查看，Admin 才判断能不能改。

Gateway 每转发一次，日志里有一行，例如：

```
PUT /api/regions/hk/sources/Binance  =>  http://localhost:5200/api/sources/Binance   as ops@demo.local (operation)
```

## demo.sh 演示的场景

| 步骤 | 做什么 | 期望 |
|---|---|---|
| 1 | 未登录访问 | 401，Backend 收不到请求 |
| 2 | viewer 登录 | 200 |
| 3 | 读 hk 的源列表 | 200 |
| 4 | `whoami` | Backend 没收到 Cookie，收到的是网关的 key 和 viewer 的身份 |
| 5 | 自己带 `X-Demo-Role: technical` | 被网关删掉，Backend 看到的还是 viewer |
| 6 | viewer 保存 | 网关放行，Backend 返回 403 |
| 7 | ops 登录，改深度 / 改 connector | 200 / 403 |
| 8 | 用旧版本号再改 | 409 |
| 9 | 读 ny 的同一个源 | 深度还是 10，两个区域互不影响 |
| 10 | 白名单外的方法 / 路径 | 405 / 404 |
| 11 | 绕过网关直接调 Backend：不带 key、带 Cookie、错 key | 都是 401 |

脚本在第 7 步把 hk 的 Binance 改到了版本 2；再跑一次之前先重启 hk 的 Backend，否则第 7、8 步的结果会不同。

## 用 Swagger 调试

### Gateway：http://localhost:5100/swagger

1. 展开 `POST /login`，Try it out，填 `{"email":"ops@demo.local","password":"ops-pass"}`，Execute。
2. 浏览器会记住 Cookie。之后在同一个页面调用 `/api/regions/hk/...`，都以 ops 的身份转发。
3. 每个转发接口都有一个 `X-Demo-Role` 输入框。填 `technical` 再调 `whoami`，可以看到这个头没有到达 Backend。

转发接口是 YARP 的路由，不是普通接口，Swagger 发现不了，所以是在 `AddOpenApi` 里从 `Forwarded` 白名单手动加进文档的。
白名单加了接口，文档会跟着变。

### Backend：http://localhost:5200/swagger、http://localhost:5201/swagger

绕过网关直接调用，用来对照网关替你发了什么：

1. 点 Authorize，填三项：`ApiKey` 填 `ApiKey hk-key-0123456789`（ny 用 `ApiKey ny-key-9876543210`，前面的 `ApiKey ` 不能省），
   `User` 填一个邮箱，`Role` 填一个角色。
2. 调任意接口。key 错、缺邮箱、角色不在三种之内，都是 401。

## 用 Visual Studio 断点调试

需要 Visual Studio 2022 17.11 或更新版本。

**方法一：用仓库里的启动配置（推荐）**

1. 第一次用时开启预览功能：工具 → 选项 → 环境 → 预览功能，勾选“启用多项目启动配置文件”（Enable Multi-Project
   Launch Profiles），然后重启 VS。
2. 打开 `TransferReqProxy.slnx`。
3. 工具栏启动按钮旁的下拉框里选 **Gateway + Backend hk**。这个配置在 `TransferReqProxy.slnLaunch` 里，会以调试方式启动
   Backend（`Backend hk`，5200）和 Gateway（5100）。
4. 按 F5，两个服务的 Swagger 会自动打开。

**方法二：手动设置**

1. 右键解决方案 → 配置启动项目 → 多个启动项目。
2. Backend：操作选“启动”，调试目标选 `Backend hk`；Gateway：操作选“启动”。
3. 确定。

**ny 区域**：VS 一个项目只启动一个配置文件，要用 ny 就在终端里另外跑
`dotnet run --project src/Backend --launch-profile "Backend ny"`。不需要 ny 时可以不启动它，访问 `/api/regions/ny/...`
会得到 502，hk 不受影响。

断点按请求经过的顺序打：

| 文件:行 | 看什么 |
|---|---|
| `src/Gateway/Program.cs:106` `AddRequestTransform` 里面 | `transform.HttpContext.Request.Path`（原路径）、`transform.Path`（改写后）、`transform.DestinationPrefix`（目标）、`transform.HttpContext.User`（登录的人） |
| `src/Backend/Program.cs:135` `HandleAuthenticateAsync` | 收到的 key 和两个头 |
| `src/Backend/Program.cs:95` connector 检查 | 角色判断 |
| `src/Backend/Program.cs:100` 版本检查 | 409 是怎么来的 |

在 Gateway 的断点上建议加条件 `transform.HttpContext.Request.Method == "PUT"`，只停在保存那一次。

## 可以自己试的改动

- **加一个转发接口**：在 Backend 加 `GET /api/sources/{name}/history`，再把 `(["GET"], "/sources/{name}/history")` 加进
  Gateway 的 `Forwarded`。只改 Backend 不改白名单，从网关访问是 404。
- **加一个区域**：在 `src/Gateway/appsettings.json` 的 `Regions` 加 `ld`，再用新端口和新 key 起一个 Backend。路由和 Swagger 都会自动多出 `ld`。
- **去掉删 Cookie 那一行**（`Program.cs:117`），再看 `whoami` 的 `cookieReceived`。
- **去掉删 `X-Demo-*` 的循环**，再重复第 5 步，看伪造的角色会不会生效。

## 常见问题

| 现象 | 原因 |
|---|---|
| 所有 `/api/regions/...` 都是 404 | Gateway 没读到 `appsettings.json`：用 `dotnet run --project src/Gateway` 启动，或在 `src/Gateway` 目录里启动 |
| 转发返回 502 | 对应区域的 Backend 没启动，或端口不对 |
| 转发返回 401，但已登录 | Gateway 的 key 和 Backend 的 `Backend:ApiKey` 不一致 |
| 端口被占用 | 上次的进程还在：`taskkill /F /IM Backend.exe`、`taskkill /F /IM Gateway.exe` |
| Swagger 里调用转发接口是 401 | 还没在同一个页面执行 `/login`，或 Cookie 过期 |

## 和 QuoteHub 的对照

| demo | QuoteHub |
|---|---|
| `X-Demo-User` / `X-Demo-Role` | `X-QuoteHub-User` / `X-QuoteHub-Role` |
| Gateway 的 `ApiKey`（每个区域一把） | `Regions__{code}__ConsoleKey` |
| `RegionProxyConfig.Forwarded` | `QuoteHub.Console/Regions/RegionProxy.cs` 的 `Forwarded` |
| Backend `KeyHandler` | `QuoteHub.Admin/Access/KeyAuthenticationHandler.cs` |
| connector 只有 technical 能改 | `QuoteHub.Admin/Api/SourceEndpoints.cs` `SaveAsync` 里的检查 |
| `expectedVersion` 和 409 | `SourceConfigurationEdit.ExpectedVersion` |

demo 省掉的：Entra ID 登录、密码哈希、数据库、Service key、改配置后通知源进程。
