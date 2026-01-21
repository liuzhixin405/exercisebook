# Permissions initialization (Identity DB)

说明：本说明文档介绍 `src/IdentityPaymentApi/Scripts/init_run.sql` 的用途与使用步骤。该脚本在 Identity 数据库中创建演示性的角色与权限，并把权限赋给 `Payer` 角色（不包含把具体用户加入角色的语句，需手动执行或通过 API 完成）。

注意：脚本针对 PostgreSQL 编写（双引号标识符）。请不要在 SQL Server 环境下执行。

前提
- PostgreSQL 可用且可访问（参见 `src/IdentityPaymentApi/appsettings.json` 中的 `IdentityConnection`）。
- 已对 `ApplicationDbContext` 运行 EF Core 迁移，或表结构已经存在。
  - （如果需要生成/应用迁移）示例：
    - `dotnet ef migrations add AddIdentityPermissions --context ApplicationDbContext --project src/IdentityPaymentApi --startup-project src/IdentityPaymentApi --output-dir Migrations/IdentityPermissions`
    - `dotnet ef database update --context ApplicationDbContext --project src/IdentityPaymentApi --startup-project src/IdentityPaymentApi`

步骤
1. 注册一个测试用户（通过 API）：
   - POST `/api/auth/register`（JSON 包含 `email`、`password`、`fullName`），记录响应中的 `Id` 或在 DB 的 `AspNetUsers` 中查询该用户的 `Id`。

2. 编辑脚本并替换 `<USER_ID>`（可选）
   - 脚本文件： `src/IdentityPaymentApi/Scripts/init_run.sql`
   - 如果要顺便把用户加入 `Payer` 角色，请在 DB 中执行：

```sql
INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
SELECT '<USER_ID>', '00000000-0000-0000-0000-000000000001'
WHERE NOT EXISTS (
  SELECT 1 FROM "AspNetUserRoles" WHERE "UserId" = '<USER_ID>' AND "RoleId" = '00000000-0000-0000-0000-000000000001'
);
```

3. 在 PostgreSQL 上执行脚本（示例，替换连接信息）：

```sh
psql "Host=localhost;Port=5432;Database=IdentityPaymentDemo_Identity;Username=victor;Password=1230" -f src/IdentityPaymentApi/Scripts/init_run.sql
```

4. 验证数据已写入：

```sql
SELECT * FROM "Permissions";
SELECT * FROM "RolePermissions" WHERE "RoleId" = '00000000-0000-0000-0000-000000000001';
SELECT * FROM "AspNetUserRoles" WHERE "UserId" = '<USER_ID>';
```

5. 启动应用并测试授权
- 通过 `POST /api/auth/login` 获取 JWT token。将 token 在 Swagger 的 Authorize 中填入 `Bearer <token>`。
- 受权限保护的端点示例（已在代码中注册策略）：
  - `POST /api/payments` -> 需要 `payments.create`
  - `GET /api/payments`  -> 需要 `payments.read`
  - `GET /api/payments/{id}` -> 需要 `payments.view`

示例请求（curl）：

```sh
curl -X POST https://localhost:5001/api/payments \
  -H "Authorization: Bearer <TOKEN>" \
  -H "Content-Type: application/json" \
  -d '{"amount":10.0,"currency":"USD","method":0,"description":"Test"}'
```

说明与注意事项
- 脚本使用 `INSERT ... SELECT ... WHERE NOT EXISTS` 的幂等写法，可重复执行不会产生重复行。
- 脚本本身不会创建用户（请通过注册接口创建用户以获得正确的密码哈希和 Identity 行为）。
- 我们把权限表放在 Identity DB（`ApplicationDbContext`），业务数据库只保留 `Payment` 表并保存 `UserId`。
- 若迁移尝试创建已存在的 Identity 表会失败。开发时可选择清空目标 DB 让 EF 迁移完全创建，或手动调整迁移以只创建缺失表。

扩展建议
- 生产环境下建议：把权限管理做成可管理的 UI/API；对权限查询添加缓存（MemoryCache/Redis）；在权限变更时使缓存失效。
- 如果希望把策略名自动映射到 permission（无需在 `Program.cs` 逐一注册），可以实现 `IAuthorizationPolicyProvider` 的动态策略提供器。

如果需要我：
- 把 `<USER_ID>` 值写入脚本并尝试在本地运行（需提供 user id 或数据库可访问），或
- 为你实现动态策略提供器样例（自动把任意策略名映射到 permission），告诉我你要哪一个。
