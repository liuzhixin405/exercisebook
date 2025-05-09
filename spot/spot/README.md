# Spot 应用

这是一个基于 .NET 8 构建的现代化Web API项目，采用了干净架构(Clean Architecture)设计原则。

## 技术栈

- **.NET 8**: 最新的.NET框架
- **Entity Framework Core**: ORM框架，用于数据访问
- **MediatR**: 中介者模式实现，用于处理请求和通知
- **FluentValidation**: 验证库
- **JWT认证**: 基于令牌的身份验证
- **Swagger/OpenAPI**: API文档和测试工具

## 项目结构

项目采用干净架构(Clean Architecture)分层设计:

- **Core**
  - `spot.Domain`: 包含领域实体和业务规则
  - `spot.Application`: 包含应用层逻辑，如用例、DTO和接口

- **Infrastructure**
  - `spot.Infrastructure.Persistence`: 包含数据持久化实现
  - `spot.Infrastructure.FileManager`: 文件管理服务实现

- **Presentation**
  - `spot.WebApi`: Web API层，处理HTTP请求

## 数据库配置

项目支持两种数据库模式:

1. **内存数据库**: 适用于开发和测试，无需额外的数据库服务器
2. **SQL Server**: 适用于生产环境

可以在`appsettings.json`文件中通过`UseInMemoryDatabase`设置来切换数据库模式。

## 运行项目

### 前提条件

- .NET 8 SDK
- Visual Studio 2022/VS Code (可选)

### 步骤

1. 克隆仓库
2. 在项目根目录打开命令行
3. 执行以下命令:

```bash
cd spot/Src/Presentation/spot.WebApi
dotnet restore
dotnet run
```

## API文档

启动应用后，可以通过以下地址访问Swagger UI:

```
http://localhost:5000/swagger
```

或者在本地运行时:

```
https://localhost:5001/swagger
```

> 注: 端口号可能会根据环境配置而变化。启动应用时，会在控制台输出实际的访问地址。

## 本地化支持

应用支持多语言，目前支持:
- 英语 (en)
- 波斯语 (fa)

可以在`appsettings.json`的`Localization`部分配置默认语言和支持的语言。

## 认证与授权

API使用JWT Bearer Token进行认证。可以通过以下端点获取令牌:

```
POST /api/v1/Auth/Login
```

请求体示例:
```json
{
  "userName": "admin",
  "password": "P@ssw0rd"
}
```

成功后，响应将包含JWT令牌，可用于后续请求的认证。将令牌添加到请求头:

```
Authorization: Bearer {你的令牌}
```

其他认证相关接口:

- `POST /api/v1/Auth/LoginByUserName`: 通过用户名直接登录（仅用于演示）
- `POST /api/v1/Auth/ChangePassword`: 修改密码
- `POST /api/v1/Auth/ChangeUserName`: 修改用户名
- `POST /api/v1/Auth/RegisterGhostAccount`: 注册临时账户 