# ECommerce 电商系统

一个基于 .NET Core 和 React 的现代化电商平台，支持完整的订单生命周期管理、支付处理、库存管理和用户管理。

## 🚀 功能特性

### 核心功能
- **用户管理**: 注册、登录、用户信息管理
- **商品管理**: 商品CRUD、图片上传、分类管理
- **购物车**: 添加商品、数量调整、价格计算
- **订单管理**: 完整的订单生命周期（待支付→已支付→已确认→已发货→已送达→已完成）
- **支付系统**: 模拟支付处理、支付状态跟踪
- **库存管理**: 实时库存跟踪、库存锁定机制
- **地址管理**: 用户收货地址管理

### 管理功能
- **后台管理**: 管理员仪表板
- **订单管理**: 订单状态管理、批量操作
- **商品管理**: 商品上架、库存管理
- **用户管理**: 用户信息查看和管理
- **库存管理**: 库存调整、交易记录

### 技术特性
- **事件驱动架构**: 基于RabbitMQ的异步消息处理
- **微服务架构**: 清晰的分层架构设计
- **实时通知**: 订单状态变更通知
- **响应式设计**: 支持移动端和桌面端
- **安全认证**: JWT Token认证

## 🏗️ 技术栈

### 后端
- **.NET 8**: 主要开发框架
- **Entity Framework Core**: 数据访问层
- **MySQL**: 数据库
- **RabbitMQ**: 消息队列
- **AutoMapper**: 对象映射
- **Serilog**: 日志记录

### 前端
- **React 18**: 用户界面框架
- **TypeScript**: 类型安全
- **Tailwind CSS**: 样式框架
- **React Router**: 路由管理
- **Axios**: HTTP客户端

## 📁 项目结构

```
ecommerce/
├── ECommerce-background/          # 后端项目
│   ├── ECommerce.API/             # Web API层
│   ├── ECommerce.Application/     # 应用服务层
│   ├── ECommerce.Domain/          # 领域层
│   ├── ECommerce.Infrastructure/  # 基础设施层
│   └── ECommerce.Core/            # 核心层
├── ecommerce-frontend/            # 前端项目
│   ├── src/
│   │   ├── components/            # React组件
│   │   ├── pages/                 # 页面组件
│   │   ├── services/              # API服务
│   │   ├── interfaces/            # TypeScript接口
│   │   └── utils/                 # 工具函数
│   └── public/                    # 静态资源
└── README.md                      # 项目文档
```

## 🚀 快速开始

### 环境要求
- .NET 8 SDK
- Node.js 18+
- MySQL 8.0+
- RabbitMQ 3.8+

### 后端启动

1. **克隆项目**
```bash
git clone <repository-url>
cd ecommerce
```

2. **配置数据库**
```bash
# 创建数据库
mysql -u root -p
CREATE DATABASE ecommerce;
```

3. **启动后端**
```bash
cd ECommerce-background
dotnet restore
dotnet run --project ECommerce.API
```

后端将在 `https://localhost:7037` 启动

### 前端启动

1. **安装依赖**
```bash
cd ecommerce-frontend
npm install
```

2. **启动前端**
```bash
npm start
```

前端将在 `http://localhost:3000` 启动

## 🔧 配置说明

### 后端配置
编辑 `ECommerce-background/ECommerce.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ecommerce;Uid=root;Pwd=your_password;"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest"
  }
}
```

### 前端配置
编辑 `ecommerce-frontend/.env`:

```env
REACT_APP_API_URL=https://localhost:7037/api
```

## 📊 数据库设计

### 主要表结构
- **Users**: 用户信息
- **Products**: 商品信息
- **Orders**: 订单信息
- **OrderItems**: 订单项
- **Addresses**: 收货地址
- **OutboxMessages**: 事件消息

### 订单状态流转
```
Pending → Paid → Confirmed → Shipped → Delivered → Completed
   ↓
Cancelled/Refunded
```

## 🔄 事件驱动架构

系统使用事件驱动架构处理异步操作：

### 主要事件
- **OrderCreatedEvent**: 订单创建
- **OrderPaidEvent**: 订单支付
- **OrderStatusChangedEvent**: 订单状态变更
- **PaymentSucceededEvent**: 支付成功
- **InventoryUpdatedEvent**: 库存更新

### 消息队列
- **order.confirmation.queue**: 订单确认
- **order.shipment.queue**: 订单发货
- **order.completion.queue**: 订单完成
- **order.expired.queue**: 订单过期

## 🛡️ 安全特性

- **JWT认证**: 基于Token的身份验证
- **角色权限**: 用户和管理员角色分离
- **输入验证**: 前后端数据验证
- **SQL注入防护**: 参数化查询
- **XSS防护**: 输入输出过滤

## 📱 用户界面

### 用户端功能
- 商品浏览和搜索
- 购物车管理
- 订单查看和跟踪
- 地址管理
- 用户信息管理

### 管理端功能
- 商品管理
- 订单管理
- 用户管理
- 库存管理
- 数据统计

## 🧪 测试

### 后端测试
```bash
cd ECommerce-background
dotnet test
```

### 前端测试
```bash
cd ecommerce-frontend
npm test
```

## 📈 性能优化

- **数据库索引**: 关键字段建立索引
- **缓存机制**: Redis缓存热点数据
- **异步处理**: 消息队列处理耗时操作
- **分页查询**: 大数据量分页加载
- **图片优化**: 图片压缩和CDN

## 🚀 部署

### Docker部署
```bash
# 构建镜像
docker build -t ecommerce-api ./ECommerce-background
docker build -t ecommerce-frontend ./ecommerce-frontend

# 运行容器
docker-compose up -d
```

### 生产环境配置
- 使用HTTPS
- 配置反向代理
- 设置环境变量
- 启用日志记录
- 配置监控告警

## 🤝 贡献指南

1. Fork 项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 打开 Pull Request

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情

## 📞 联系方式

如有问题或建议，请通过以下方式联系：
- 提交 Issue
- 发送邮件
- 项目讨论区

---

**注意**: 这是一个演示项目，生产环境使用前请进行充分的安全测试和性能优化。