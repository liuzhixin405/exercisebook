# Agentic Commerce + ERP Demo

这是一个 .NET 8 示例项目，用来演示如何把 AI 原生工作流，也就是 Agentic Workflow，融入正常的业务开发中。

这个 demo 的重点不是做一个聊天机器人，而是展示：

```text
用户目标
  -> Agent 编排
  -> 生成执行计划
  -> 调用受控业务工具
  -> 触发风控策略
  -> 必要时进入人工审批
  -> 记录审计日志
```

核心原则是：**AI 负责理解目标和编排流程，业务规则、写操作、审批和审计仍然由 .NET 后端控制。**

## 项目结构

```text
src/
  AgenticCommerceErpDemo.Api/
    Endpoints/
    Program.cs

  AgenticCommerceErpDemo.Application/
    Agents/
    Ai/
    Approvals/
    Auditing/
    Business/
    Guardrails/
    Knowledge/
    State/
    Tools/

  AgenticCommerceErpDemo.Domain/
    Catalog/
    Common/
    Customer/
    Finance/
    Inventory/
    Knowledge/
    Procurement/
    Promotions/

  AgenticCommerceErpDemo.Infrastructure/
    Ai/
    Auditing/
    Guardrails/
    Knowledge/
    Persistence/
    Services/
    Tools/
    DependencyInjection.cs
```

## 分层职责

`AgenticCommerceErpDemo.Api`

HTTP 入口层，只负责 endpoint 映射、应用启动和依赖注入调用。它不直接包含库存、采购、促销、客户投诉等业务逻辑。

`AgenticCommerceErpDemo.Application`

应用层，负责 Agentic Workflow 的核心用例。这里包含：

- `AgentOrchestrator`：Agent 编排器
- `IAiModel`：AI 规划器抽象
- `IBusinessToolRegistry`：业务工具注册表抽象
- `IGuardrailPolicy`：风控策略抽象
- `IApprovalRepository`：审批仓储抽象
- `IAuditLog`：审计日志抽象
- 各类业务服务接口

`AgenticCommerceErpDemo.Domain`

领域层，只放业务模型和枚举，比如商品、库存、销售预测、采购单草稿、促销草稿、客户投诉、财务风险报告等。

这个层不依赖 AI、不依赖数据库、不依赖 ASP.NET Core。

`AgenticCommerceErpDemo.Infrastructure`

基础设施层，放可替换的具体实现，比如：

- 内存数据存储
- 种子数据
- 本地 deterministic AI planner
- 业务服务实现
- 知识库检索
- 工具注册表
- 风控策略
- 审计日志
- DI 注册

## 覆盖的业务场景

这个 demo 模拟了 ERP + 电商混合场景：

- 库存风险分析
- 销售预测对比
- 商品转化率下降分析
- 客户投诉摘要
- 财务预算和风险检查
- 采购单草稿创建
- 促销草稿创建
- 高风险动作进入人工审批
- 全链路审计日志
- 当前系统状态查询
- 可替换的 `IAiModel` AI 规划器

## 运行方式

```powershell
cd C:\Users\victor\AppData\Local\Temp\AgenticCommerceErpDemo
dotnet run --project .\src\AgenticCommerceErpDemo.Api\AgenticCommerceErpDemo.Api.csproj --urls http://localhost:5187
```

打开：

```text
http://localhost:5187/demo
```

## 调用示例

提交一个复杂业务目标：

```http
POST /agent/tasks
Content-Type: application/json

{
  "goal": "Analyze East warehouse inventory risk, declining conversion products, customer complaints, and prepare replenishment plus promotion actions."
}
```

这个请求会触发一条完整的 Agentic Workflow：

```text
KnowledgeAgent
  -> knowledge.search

InventoryAgent
  -> inventory.analyzeRisk

CommerceAgent
  -> catalog.analyzeConversionDrops

CustomerAgent
  -> customer.summarizeComplaints

FinanceAgent
  -> finance.checkRisk

CommerceAgent
  -> commerce.createPromotionDraft

InventoryAgent
  -> inventory.createPurchaseOrderDraft

OpsAgent
  -> ops.notify
```

其中采购单草稿属于高风险动作，会进入人工审批。

## 其他接口

查看当前系统状态：

```http
GET /state
```

查看审计日志：

```http
GET /audit
```

审批高风险动作：

```http
POST /approvals/{approvalId}/approve
```

拒绝高风险动作：

```http
POST /approvals/{approvalId}/reject
```

## 架构流转

```text
用户目标 / 系统事件
        |
AgenticCommerceErpDemo.Api
        |
AgentOrchestrator
        |
IAiModel
        |
AgentPlan
        |
IBusinessToolRegistry
        |
确定性的 ERP / 电商业务服务
        |
IGuardrailPolicy + IApprovalRepository + IAuditLog
```

## 为什么这样设计

AI 不应该直接操作数据库，也不应该绕过业务服务直接执行高风险动作。

在这个 demo 中，AI 只能生成计划，并通过 `IBusinessToolRegistry` 调用系统暴露出来的业务工具。真正的库存分析、采购单草稿、促销草稿、财务检查，仍然由明确的 C# 业务服务执行。

高风险动作会被 `IGuardrailPolicy` 拦截，然后生成 `ApprovalRequest`，等待人工审批。

这更接近生产系统应该采用的方式：

```text
AI 做理解和编排
.NET 业务服务做确定性执行
风控策略做边界控制
审批机制处理高风险动作
审计日志保证可追踪和可回放
```

## 生产化扩展方向

如果要把这个 demo 演进到真实项目，可以继续加：

- Entity Framework Core 持久化
- Redis / SQL Server 存储审批和审计日志
- OpenTelemetry 链路追踪
- 真实 LLM planner，例如 OpenAI、Azure OpenAI 或本地模型
- RAG 知识库，例如 Azure AI Search、Elasticsearch、Qdrant、pgvector
- 后台任务队列，例如 Hangfire、Quartz.NET、MassTransit
- 多租户权限控制
- 工具级权限和速率限制
- Agent 结果评估和回放测试

## 关键边界

这个 demo 想表达的核心不是“把业务系统变成 AI 系统”，而是：

**在原有确定性业务系统之上，增加一层可编排、可审计、可审批的智能流程层。**

对于 ERP、电商、CRM、供应链这类系统，这种方式比单纯做聊天框更有价值。
