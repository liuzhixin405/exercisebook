using ModelContextProtocol.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MCPHostApp
{
    static class McpToolExtensions
    {
        public static object ToOpenAIToolObject(this McpClientTool tool)
        {
            // 将 tool.JsonSchema（string）反序列化成 object，供 PostAsJsonAsync 使用
            return JsonSerializer.Deserialize<object>(tool.JsonSchema);
        }
    }


}
