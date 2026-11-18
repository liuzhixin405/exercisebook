# distributor：Distributor 的扇出

`dotnet run`，端口 5400。进程内的 `Hub` 代替 Valkey pub/sub，`FakeSource` 代替 Source 发布 `Book:Fake:*`。
- 浏览器打开 http://localhost:5400 ：订阅 `Book:Fake:BTCUSD`。
- 带 `?delayMs=2000` 再开一个标签页当慢客户端：它的队列（上限 10）会丢最旧的消息，快客户端不受影响，`/stats` 可看队列长度。
- 错误的 key：`/ws?key=bad` 返回 401（QuoteHub 里 key 的哈希由 Admin 下发）。
- 手动发布：`curl -X POST localhost:5400/publish/Book:Fake:BTCUSD -d '{"bid":1}'`。
- 订阅协议：`{"op":"subscribe","channels":["Book:Fake:ETHUSD"]}`，`unsubscribe` 同理。

对应 QuoteHub：`QuoteHub.Distributor/Program.cs`。
