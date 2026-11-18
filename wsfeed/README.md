# wsfeed：Source 的 WebSocket 接入骨架

假交易所（`/ws`）和 feed 客户端放在同一个进程里，`dotnet run` 即可。
- 5 个 symbol、每连接最多 2 个，所以拆成 3 条连接（QuoteHub：`Feeds/ConnectionPlan.cs`）。
- 每条 SUBSCRIBE 之间间隔 `1000/MaxRequestsPerSecond` ms（限速）。
- 假交易所 8 秒后掐断连接，客户端按 1s、2s、4s…（上限 5s）退避重连，收到消息后退避归零（QuoteHub：`Feeds/WebSocketFeed.cs`）。

试一试：把 `MaxSymbolsPerConnection` 改成 5，看连接数；把 `MaxRequestsPerSecond` 改成 1，看订阅变慢。
