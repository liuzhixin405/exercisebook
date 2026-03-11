using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agent
{
    public class ReActAgent
    {
        private readonly Dictionary<string, Func<string[], object>> _tools;
        private readonly string _model;
        private readonly string _projectDirectory;
        private readonly string _baseUrl = "http://localhost:11434/api/chat";
        private static readonly Random _random = new Random();

        public ReActAgent(Dictionary<string, Func<string[], object>> tools, string model, string projectDirectory)
        {
            _tools = tools;
            _model = model;
            _projectDirectory = projectDirectory;
        }

        public async Task<string> RunAsync(string userInput)
        {
            var messages = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "role", "system" },
                    { "content", RenderSystemPrompt(PromptTemplate.ReactSystemPromptTemplate) }
                },
                new Dictionary<string, object>
                {
                    { "role", "user" },
                    { "content", $"<question>{userInput}</question>" }
                }
            };

            while (true)
            {
                // 请求模型
                var content = await CallModelAsync(messages);

                // 检测 Thought
                var thoughtMatch = Regex.Match(content, @"<thought>(.*?)</thought>", RegexOptions.Singleline);
                if (thoughtMatch.Success)
                {
                    var thought = thoughtMatch.Groups[1].Value;
                    Console.WriteLine($"\n\n💭 Thought: {thought}");
                }

                // 优先检测 Action：如果有 action 则执行；否则如果有 final_answer 则返回结果；二者皆无则抛错
                var actionMatch = Regex.Match(content, @"<action>(.*?)</action>", RegexOptions.Singleline);
                if (!actionMatch.Success)
                {
                    // 没有 action，检查 final_answer
                    if (content.Contains("<final_answer>"))
                    {
                        var finalAnswerMatch = Regex.Match(content, @"<final_answer>(.*?)</final_answer>", RegexOptions.Singleline);
                        return finalAnswerMatch.Groups[1].Value;
                    }

                    throw new InvalidOperationException("模型未输出 <action> 或 <final_answer>");
                }

                var action = actionMatch.Groups[1].Value;
                var (toolName, args) = ParseAction(action);

                Console.WriteLine($"\n\n🔧 Action: {toolName}({string.Join(", ", args)})");

                // 只有终端命令才需要询问用户，其他的工具直接执行
                var shouldContinue = toolName == "run_terminal_command" ? 
                    GetUserInput("\n\n是否继续？（Y/N）") : "y";
                
                if (shouldContinue.ToLower() != "y")
                {
                    Console.WriteLine("\n\n操作已取消。");
                    return "操作被用户取消";
                }

                try
                {
                    // 对文件相关工具，如果第一个参数是相对路径，则基于 _projectDirectory 解析为绝对路径
                    string[] execArgs = args;
                    if ((toolName == "read_file" || toolName == "write_to_file") && args.Length > 0)
                    {
                        var first = args[0];
                        if (!Path.IsPathRooted(first))
                        {
                            var combined = Path.GetFullPath(Path.Combine(_projectDirectory, first));
                            execArgs = (string[])args.Clone();
                            execArgs[0] = combined;
                        }
                    }

                    // 对于 run_terminal_command，做人性化预处理：
                    // - 如果是 `dotnet new` 且输出路径是磁盘根（例如 D:\），则改为在根下创建子目录（使用 -n 指定的名称或从自然语言任务中推断名称）
                    if (toolName == "run_terminal_command" && args.Length > 0)
                    {
                        try
                        {
                            var cmd = args[0];
                            var trimmed = cmd.Trim();
                            if (trimmed.StartsWith("dotnet new", StringComparison.OrdinalIgnoreCase))
                            {
                                // 查找 -o 或 --output 参数
                                var outMatch = Regex.Match(trimmed, @"(?:--output|-o)
\s+['""]?(?<out>[^'""\s]+)['""]?", RegexOptions.IgnoreCase);
                                var nameMatch = Regex.Match(trimmed, @"(?:-n|--name)\s+['""]?(?<name>[^'""\s]+)['""]?", RegexOptions.IgnoreCase);

                                string? outPath = outMatch.Success ? outMatch.Groups["out"].Value : null;
                                string? projName = nameMatch.Success ? nameMatch.Groups["name"].Value : null;

                                // 如果没有项目名，从 userInput 中尝试提取一个简单名称
                                if (string.IsNullOrWhiteSpace(projName))
                                {
                                    // 尝试从首个中文/英文词中提取名称，比如 "创建一个netcore控制台程序 DemoApi 到D盘"
                                    var nameGuess = Regex.Match(userInput ?? "", "([\\u4e00-\\u9fffA-Za-z0-9_-]{2,})");
                                    if (nameGuess.Success)
                                    {
                                        projName = nameGuess.Groups[1].Value;
                                    }
                                    else
                                    {
                                        projName = "NewConsoleApp" + DateTime.Now.ToString("yyyyMMddHHmmss");
                                    }
                                }

                                if (!string.IsNullOrWhiteSpace(outPath))
                                {
                                    // 如果输出是根目录（如 D:\ 或 C:\），改为在根目录下新建以 projName 命名的子目录
                                    var rootDriveMatch = Regex.Match(outPath, @"^[A-Za-z]:\\?$");
                                    if (rootDriveMatch.Success || outPath.EndsWith(":\\") || outPath.EndsWith(":/"))
                                    {
                                        var drive = outPath.TrimEnd('\\', '/');
                                        var newOut = Path.Combine(drive + Path.DirectorySeparatorChar, projName);
                                        trimmed = Regex.Replace(trimmed, @"(?:--output|-o)\s+['""]?[^'""\s]+['""]?", $"-o \"{newOut}\"");
                                    }
                                }
                                else
                                {
                                    // 如果没有指定输出，默认在当前目录下创建子目录 projName
                                    var newOut = Path.Combine(".", projName);
                                    trimmed = trimmed + " -o " + newOut;
                                }

                                // 如果命令里没有 -n/--name，添加它以保证 csproj 名称正确
                                if (!nameMatch.Success)
                                {
                                    trimmed = trimmed + " -n " + projName;
                                }

                                execArgs = (string[])args.Clone();
                                execArgs[0] = trimmed;
                                Console.WriteLine($"\n\n🔧 Preprocessed command -> {trimmed}");
                            }
                        }
                        catch
                        {
                            // 解析/预处理失败则按原命令执行
                        }
                    }

                    var observation = _tools[toolName](execArgs);
                    Console.WriteLine($"\n\n🔍 Observation：{observation}");

                    // 若是终端命令，尝试基于命令推断可能的副作用路径并验证是否已创建
                    if (toolName == "run_terminal_command" && execArgs.Length > 0)
                    {
                        try
                        {
                            var cmd = execArgs[0];
                            var expected = DetermineExpectedPathsFromCommand(cmd);
                            if (expected != null && expected.Count > 0)
                            {
                                var missing = new List<string>();
                                foreach (var p in expected)
                                {
                                    var full = Path.IsPathRooted(p) ? p : Path.GetFullPath(Path.Combine(_projectDirectory, p));
                                    if (!Directory.Exists(full) && !File.Exists(full))
                                    {
                                        missing.Add(full);
                                    }
                                }

                                if (missing.Count > 0)
                                {
                                    observation += $" (VERIFICATION FAILED: Missing {string.Join(", ", missing)})";
                                }
                                else
                                {
                                    observation += " (VERIFICATION PASSED)";
                                }
                            }
                        }
                        catch
                        {
                            // 验证出错时不阻塞主流程
                        }
                    }

                    var obsMsg = $"<observation>{observation}</observation>";
                    messages.Add(new Dictionary<string, object>
                    {
                        { "role", "user" },
                        { "content", obsMsg }
                    });
                }
                catch (Exception e)
                {
                    var observation = $"工具执行错误：{e.Message}";
                    Console.WriteLine($"\n\n🔍 Observation：{observation}");
                    var obsMsg = $"<observation>{observation}</observation>";
                    messages.Add(new Dictionary<string, object>
                    {
                        { "role", "user" },
                        { "content", obsMsg }
                    });
                }
                // 如果工具是 write_to_file，或者执行的命令不是 build 但可能改变项目文件，自动触发一次 dotnet build 来验证
                try
                {
                    bool shouldAutoBuild = false;
                    if (toolName == "write_to_file") shouldAutoBuild = true;
                    if (toolName == "run_terminal_command" && args.Length > 0)
                    {
                        var cmd = args[0].ToLowerInvariant();
                        if (!cmd.Contains("dotnet build") && (cmd.Contains("new ") || cmd.Contains("add ") || cmd.Contains("rm ") || cmd.Contains("del ") || cmd.Contains("mv ") || cmd.Contains("move ") || cmd.Contains("git ")))
                        {
                            shouldAutoBuild = true;
                        }
                    }

                    if (shouldAutoBuild && _tools.ContainsKey("run_terminal_command"))
                    {
                        var buildCmd = $"dotnet build \"{_projectDirectory}\"";
                        var buildObs = _tools["run_terminal_command"](new[] { buildCmd });
                        Console.WriteLine($"\n\n🔧 Auto-build Observation：{buildObs}");
                        var buildMsg = $"<observation>{buildObs}</observation>";
                        messages.Add(new Dictionary<string, object>
                        {
                            { "role", "user" },
                            { "content", buildMsg }
                        });
                    }
                }
                catch (Exception)
                {
                    // 忽略自动构建中发生的错误，不影响主流程
                }
            }
        }

        private string GetToolList()
        {
            var toolDescriptions = new List<string>();
            foreach (var kvp in _tools)
            {
                var name = kvp.Key;
                // 简单描述工具
                toolDescriptions.Add($"- {name}(args): 工具函数");
            }
            return string.Join("\n", toolDescriptions);
        }

        private string RenderSystemPrompt(string systemPromptTemplate)
        {
            var toolList = GetToolList();
            var fileList = string.Join(", ", Directory.GetFiles(_projectDirectory).Select(f => Path.GetFullPath(f)));
            
            return systemPromptTemplate
                .Replace("${tool_list}", toolList)
                .Replace("${operating_system}", Environment.OSVersion.ToString())
                .Replace("${file_list}", fileList);
        }

        private async Task<string> CallModelAsync(List<Dictionary<string, object>> messages)
        {
            Console.WriteLine("\n\n正在请求模型，请稍等...");

            var requestBody = new
            {
                model = _model,
                messages = messages,
                stream = true // 启用流式处理
            };

            using var client = new HttpClient();
            // 设置更长的超时时间以适应流式处理
            client.Timeout = TimeSpan.FromMinutes(5);
            
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync(_baseUrl, content);
            response.EnsureSuccessStatusCode();

            // 处理流式响应
            var result = new StringBuilder();
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            {
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        try
                        {
                            var responseObject = JsonConvert.DeserializeObject<dynamic>(line);
                            if (responseObject?.message?.content != null)
                            {
                                var contentPart = responseObject.message.content.ToString();
                                if (!string.IsNullOrEmpty(contentPart))
                                {
                                    result.Append(contentPart);
                                    // 实时显示模型输出的部分内容，带有打字机效果
                                    await WriteWithTypewriterEffect(contentPart);
                                }
                            }
                        }
                        catch
                        {
                            // 忽略解析错误的行
                        }
                    }
                }
            }
            
            // 确保所有输出都已显示
            Console.Out.Flush();
            
            var finalResult = result.ToString();
            
            messages.Add(new Dictionary<string, object>
            {
                { "role", "assistant" },
                { "content", finalResult }
            });
            
            return finalResult;
        }

        /// <summary>
        /// 以打字机效果显示文本
        /// </summary>
        /// <param name="text">要显示的文本</param>
        private async Task WriteWithTypewriterEffect(string text)
        {
            foreach (char c in text)
            {
                Console.Write(c);

                // 对于某些特殊字符，稍微增加延迟以改善视觉效果
                if (c == '.' || c == '!' || c == '?' || c == '\n')
                {
                    await Task.Delay(10);
                }
                else if (c == ',' || c == ';' || c == ':')
                {
                    await Task.Delay(5);
                }
                else
                {
                    // 随机延迟0-2毫秒，创造更自然的打字机效果
                    await Task.Delay(_random.Next(0, 3));
                }
            }
            
            // 立即刷新输出
            Console.Out.Flush();
        }

        private (string, string[]) ParseAction(string codeStr)
        {
            var match = Regex.Match(codeStr, @"(\w+)\((.*)\)", RegexOptions.Singleline);
            if (!match.Success)
            {
                throw new ArgumentException("Invalid function call syntax");
            }

            var funcName = match.Groups[1].Value;
            var argsStr = match.Groups[2].Value.Trim();

            // 手动解析参数
            var args = ParseArguments(argsStr);
            return (funcName, args.ToArray());
        }

        private List<string> ParseArguments(string argsStr)
        {
            var args = new List<string>();
            var currentArg = new StringBuilder();
            var inString = false;
            char stringChar = '\0';
            var i = 0;
            var parenDepth = 0;

            while (i < argsStr.Length)
            {
                var ch = argsStr[i];

                if (!inString)
                {
                    if (ch == '"' || ch == '\'')
                    {
                        inString = true;
                        stringChar = ch;
                        currentArg.Append(ch);
                    }
                    else if (ch == '(')
                    {
                        parenDepth++;
                        currentArg.Append(ch);
                    }
                    else if (ch == ')')
                    {
                        parenDepth--;
                        currentArg.Append(ch);
                    }
                    else if (ch == ',' && parenDepth == 0)
                    {
                        // 遇到顶层逗号，结束当前参数
                        args.Add(ParseSingleArg(currentArg.ToString().Trim()));
                        currentArg.Clear();
                    }
                    else
                    {
                        currentArg.Append(ch);
                    }
                }
                else
                {
                    currentArg.Append(ch);
                    if (ch == stringChar && (i == 0 || argsStr[i - 1] != '\\'))
                    {
                        inString = false;
                        stringChar = '\0';
                    }
                }

                i++;
            }

            // 添加最后一个参数
            if (currentArg.Length > 0)
            {
                args.Add(ParseSingleArg(currentArg.ToString().Trim()));
            }

            return args;
        }

        private string ParseSingleArg(string argStr)
        {
            argStr = argStr.Trim();

            // 如果是字符串字面量
            if ((argStr.StartsWith("\"") && argStr.EndsWith("\"")) ||
                (argStr.StartsWith("'") && argStr.EndsWith("'")))
            {
                // 移除外层引号并处理转义字符
                var innerStr = argStr.Substring(1, argStr.Length - 2);
                // 处理常见的转义字符
                innerStr = innerStr.Replace("\\\"", "\"").Replace("\\'", "'");
                innerStr = innerStr.Replace("\\n", "\n").Replace("\\t", "\t");
                innerStr = innerStr.Replace("\\r", "\r").Replace("\\\\", "\\");
                return innerStr;
            }

            // 返回原始字符串
            return argStr;
        }

        /// <summary>
        /// 基于常见命令推断预期会被创建或修改的路径（文件或目录）。
        /// 仅作启发式检测，不能覆盖所有情况。
        /// </summary>
        private List<string> DetermineExpectedPathsFromCommand(string command)
        {
            var results = new List<string>();
            if (string.IsNullOrWhiteSpace(command)) return results;

            try
            {
                // 查找 --output 或 -o 参数
                var m = Regex.Match(command, @"(?:--output|-o)\s+['""]?(?<p>[^ '""]+)['""]?", RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    results.Add(m.Groups["p"].Value.Trim());
                    return results;
                }

                // 查找 -n 项目名
                m = Regex.Match(command, "-n\\s+['\"]?(?<name>[^\\s'\"]+)['\"]?", RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    var name = m.Groups["name"].Value.Trim();
                    results.Add(name); // 目录名或 csproj 名称
                    results.Add(name + ".csproj");
                    return results;
                }

                // 针对 dotnet new 且无输出参数，默认在当前目录会产生项目文件（.csproj）或 Program.cs
                if (command.IndexOf("dotnet new", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    results.Add("Program.cs");
                }
            }
            catch
            {
                // 忽略解析错误
            }

            return results;
        }

        private string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? "";
        }
    }
}