# IdentityPaymentApi

这是一个基于 .NET 9 的 Web API 程序，提供了 ASP.NET Core Identity + JWT 鉴权以及多种支付方式的示例。

## 前置条件
- .NET 9 SDK
- PostgreSQL 数据库实例（默认连接串中使用 `postgres` 用户）
- 可选：`dotnet-ef` 工具，用于手动迁移（`dotnet tool install --global dotnet-ef`）

## 快速启动
1. 根据环境编辑 `src/IdentityPaymentApi/appsettings.json` 中的 `ConnectionStrings:DefaultConnection` 和 `Jwt:Secret`。
2. 如果需要手动创建迁移，可执行：
   ```bash
   dotnet ef migrations add InitialCreate -p src/IdentityPaymentApi --startup-project src/IdentityPaymentApi
   ```
3. 运行程序：
   ```bash
   dotnet run --project src/IdentityPaymentApi
   ```
   启动时会自动迁移数据库并创建 `Payer` 角色。

## 可用接口
- `POST /api/auth/register`：注册账号（成功后自动加入 `Payer` 角色）
- `POST /api/auth/login`：使用邮箱/密码登录并获取 JWT
- `POST /api/payments`：创建支付记录（需 `Payer` 角色的 JWT）
- `GET /api/payments`：查看当前用户的所有支付记录
- `GET /api/payments/{id}`：查看指定支付记录

## 支付模型
请求体 `PaymentRequest` 支持金额、币种和 `PaymentMethod`（`CreditCard`/`BankTransfer`/`Wallet`），后端会记录状态和时间戳。

## JWT & 权限
- 使用 `JwtSettings` 中配置的 `Secret` 生成签名，默认过期 60 分钟
- 支付相关接口通过 `Payments` 授权策略，仅 `Payer` 角色可访问
