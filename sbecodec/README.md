# sbecodec：手写一个迷你 SBE 编解码

`dotnet run -c Release`。布局和字段含义见 `Program.cs` 顶部注释。会打印十六进制、解码结果，并对比同样数据 JSON 的大小；最后做 100 万次编码加解码，确认 0 字节分配（Debug 下可能不是 0）。
- `blockLength`：让旧读者能跳过新版本追加的字段。
- 重复组、变长数据（必须在最后）。
- `ref struct` 视图：直接读缓冲区，不分配对象。

下一步：读 `src/QuoteHub.Wire` 里生成的 Snapshot 编解码，对照本 demo。
