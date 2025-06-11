using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// 天气工具类
public class WeatherTool
{
    [Description("获取指定城市的当前天气信息")]
    public static string GetWeather(
        [Description("城市名称")] string city,
        [Description("温度单位 (celsius 或 fahrenheit)")] string unit = "celsius")
    {
        // 模拟天气数据
        var temperature = unit == "celsius" ? "22°C" : "72°F";
        return $"{city}的当前天气：晴天，温度 {temperature}，湿度 65%";
    }
}

// 计算器工具类
public class CalculatorTool
{
    [Description("执行基本数学计算")]
    public static double Calculate(
        [Description("第一个数字")] double a,
        [Description("运算符 (+, -, *, /)")] string operation,
        [Description("第二个数字")] double b)
    {
        return operation switch
        {
            "+" => a + b,
            "-" => a - b,
            "*" => a * b,
            "/" => b != 0 ? a / b : throw new ArgumentException("除数不能为零"),
            _ => throw new ArgumentException("不支持的运算符")
        };
    }
}

// Ollama客户端类
public class OllamaClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly Dictionary<string, ToolDefinition> _tools;

    public OllamaClient(string baseUrl = "http://localhost:11434")
    {
        _httpClient = new HttpClient();
        _baseUrl = baseUrl;
        _tools = new Dictionary<string, ToolDefinition>();

        // 注册工具
        RegisterTool("get_weather", typeof(WeatherTool).GetMethod("GetWeather"));
        RegisterTool("calculate", typeof(CalculatorTool).GetMethod("Calculate"));
    }

    private void RegisterTool(string name, System.Reflection.MethodInfo method)
    {
        var parameters = new Dictionary<string, object>();
        var required = new List<string>();

        foreach (var param in method.GetParameters())
        {
            var description = param.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "";
            var paramType = param.ParameterType == typeof(string) ? "string" :
                           param.ParameterType == typeof(double) ? "number" : "string";

            parameters[param.Name] = new
            {
                type = paramType,
                description = description
            };

            if (!param.HasDefaultValue)
                required.Add(param.Name);
        }

        _tools[name] = new ToolDefinition
        {
            Name = name,
            Description = method.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "",
            Parameters = new
            {
                type = "object",
                properties = parameters,
                required = required.ToArray()
            },
            Method = method
        };
    }

    public async Task<string> ChatWithToolsAsync(string message, string model = "qwen2.5-coder:7b")
    {
        try
        {
            var messages = new List<object>
            {
                new { role = "user", content = message }
            };

            var toolDefinitions = _tools.Values.Select(t => new
            {
                type = "function",
                function = new
                {
                    name = t.Name,
                    description = t.Description,
                    parameters = t.Parameters
                }
            }).ToArray();

            var request = new
            {
                model = model,
                stream = false,
                messages = messages,
                tools = toolDefinitions
            };

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            Console.WriteLine($"发送请求: {json}"); // 调试信息

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/chat", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"HTTP错误 {response.StatusCode}: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"收到响应: {responseContent}"); // 调试信息

            var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

            // 检查响应结构
            if (!result.TryGetProperty("message", out var messageElement))
            {
                // 可能是错误响应或不同的格式
                if (result.TryGetProperty("error", out var errorElement))
                {
                    return $"模型错误: {errorElement.GetString()}";
                }

                // 尝试直接返回响应内容
                return result.GetRawText();
            }

            // 检查是否有工具调用
            if (messageElement.TryGetProperty("tool_calls", out var toolCalls) && toolCalls.ValueKind == JsonValueKind.Array)
            {
                var toolResults = new List<object>();

                foreach (var toolCall in toolCalls.EnumerateArray())
                {
                    try
                    {
                        var functionElement = toolCall.GetProperty("function");
                        var functionName = functionElement.GetProperty("name").GetString();

                        // 处理参数 - 可能是字符串或对象
                        JsonElement arguments;
                        if (functionElement.TryGetProperty("arguments", out var argsElement))
                        {
                            if (argsElement.ValueKind == JsonValueKind.String)
                            {
                                // 参数是JSON字符串，需要解析
                                var argsString = argsElement.GetString();
                                arguments = JsonSerializer.Deserialize<JsonElement>(argsString);
                            }
                            else
                            {
                                // 参数已经是对象
                                arguments = argsElement;
                            }
                        }
                        else
                        {
                            arguments = new JsonElement();
                        }

                        if (_tools.TryGetValue(functionName, out var tool))
                        {
                            var toolResult = await ExecuteToolAsync(tool, arguments);
                            toolResults.Add(new
                            {
                                role = "tool",
                                tool_call_id = toolCall.TryGetProperty("id", out var idElement) ? idElement.GetString() : Guid.NewGuid().ToString(),
                                content = toolResult
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"处理工具调用时出错: {ex.Message}");
                        toolResults.Add(new
                        {
                            role = "tool",
                            tool_call_id = Guid.NewGuid().ToString(),
                            content = $"工具调用错误: {ex.Message}"
                        });
                    }
                }

                // 将工具结果发送回模型
                if (toolResults.Any())
                {
                    messages.Add(new { role = "assistant", tool_calls = toolCalls });
                    messages.AddRange(toolResults);

                    var followUpRequest = new
                    {
                        model = model,
                        stream = false,
                        messages = messages
                    };

                    var followUpJson = JsonSerializer.Serialize(followUpRequest, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    var followUpContent = new StringContent(followUpJson, Encoding.UTF8, "application/json");
                    var followUpResponse = await _httpClient.PostAsync($"{_baseUrl}/api/chat", followUpContent);
                    var followUpResponseContent = await followUpResponse.Content.ReadAsStringAsync();
                    var followUpResult = JsonSerializer.Deserialize<JsonElement>(followUpResponseContent);

                    if (followUpResult.TryGetProperty("message", out var finalMessage) &&
                        finalMessage.TryGetProperty("content", out var finalContent))
                    {
                        return finalContent.GetString() ?? "无响应内容";
                    }
                }
            }

            // 返回普通消息内容
            if (messageElement.TryGetProperty("content", out var contentElement))
            {
                return contentElement.GetString() ?? "无响应内容";
            }

            return "未能解析响应内容";
        }
        catch (Exception ex)
        {
            return $"请求失败: {ex.Message}";
        }
    }

    private async Task<string> ExecuteToolAsync(ToolDefinition tool, JsonElement arguments)
    {
        try
        {
            var parameters = tool.Method.GetParameters();
            var args = new object[parameters.Length];

            Console.WriteLine($"执行工具: {tool.Name}");
            Console.WriteLine($"参数: {arguments.GetRawText()}");

            for (int i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                var paramName = param.Name;

                if (arguments.ValueKind != JsonValueKind.Undefined &&
                    arguments.TryGetProperty(paramName, out var value))
                {
                    try
                    {
                        if (param.ParameterType == typeof(string))
                            args[i] = value.GetString();
                        else if (param.ParameterType == typeof(double))
                            args[i] = value.GetDouble();
                        else if (param.ParameterType == typeof(int))
                            args[i] = value.GetInt32();
                        else
                            args[i] = value.ToString();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"参数转换错误 {paramName}: {ex.Message}");
                        if (param.HasDefaultValue)
                            args[i] = param.DefaultValue;
                        else
                            args[i] = param.ParameterType == typeof(string) ? "" :
                                     param.ParameterType == typeof(double) ? 0.0 :
                                     param.ParameterType == typeof(int) ? 0 : null;
                    }
                }
                else if (param.HasDefaultValue)
                {
                    args[i] = param.DefaultValue;
                }
                else
                {
                    // 必需参数但未提供值
                    args[i] = param.ParameterType == typeof(string) ? "" :
                             param.ParameterType == typeof(double) ? 0.0 :
                             param.ParameterType == typeof(int) ? 0 : null;
                }
            }

            var result = tool.Method.Invoke(null, args);
            var resultString = result?.ToString() ?? "执行成功";
            Console.WriteLine($"工具执行结果: {resultString}");
            return resultString;
        }
        catch (Exception ex)
        {
            var errorMsg = $"工具执行错误: {ex.Message}";
            Console.WriteLine(errorMsg);
            return errorMsg;
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

// 工具定义类
public class ToolDefinition
{
    public string Name { get; set; }
    public string Description { get; set; }
    public object Parameters { get; set; }
    public System.Reflection.MethodInfo Method { get; set; }
}

// 主程序示例
public class Program
{
    public static async Task Main(string[] args)
    {
        var ollama = new OllamaClient("http://localhost:11434");

        Console.WriteLine("=== Ollama 本地模型 Tools 调用示例 ===");
        Console.WriteLine("请确保 Ollama 正在运行，并且已下载支持工具调用的模型（如 qwen2.5-coder:7b）");
        Console.WriteLine("输入 'exit' 退出程序\n");

        while (true)
        {
            Console.Write("用户: ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "exit")
                break;

            try
            {
                Console.Write("AI: ");
                var response = await ollama.ChatWithToolsAsync(input);
                Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
            }

            Console.WriteLine();
        }

        ollama.Dispose();
    }
}

// 使用示例:
// 1. "北京的天气怎么样？"
// 2. "计算 15 + 25"
// 3. "上海的天气如何，用华氏度显示"
// 4. "125 除以 5 等于多少？"