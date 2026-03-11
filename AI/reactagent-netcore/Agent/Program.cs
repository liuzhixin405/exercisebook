using Agent;

class Program
{
    static async Task Main(string[] args)
    {
        // 使用当前目录作为项目目录
        var projectDirectory = Environment.CurrentDirectory;

        // 定义工具
        var tools = new Dictionary<string, Func<string[], object>>
        {
            { "read_file", Tools.ReadFile },
            { "write_to_file", Tools.WriteToFile },
            { "run_terminal_command", Tools.RunTerminalCommand }
        };

        // 创建Agent实例，使用本地Ollama的qwen2.5-coder:7b模型
        var agent = new ReActAgent(tools, "granite4", projectDirectory);

        Console.WriteLine("欢迎使用ReActAgent！输入 'quit' 或 'exit' 退出程序。");

        while (true)
        {
            Console.Write("\n请输入任务（或输入 'quit'/'exit' 退出）：");
            var task = Console.ReadLine();

            if (string.IsNullOrEmpty(task))
            {
                Console.WriteLine("任务不能为空");
                continue;
            }

            // 检查退出命令
            if (task.ToLower() == "quit" || task.ToLower() == "exit")
            {
                Console.WriteLine("再见！");
                break;
            }

            try
            {
                var finalAnswer = await agent.RunAsync(task);
                Console.WriteLine($"\n\n✅ Final Answer：{finalAnswer}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n\n❌ 错误：{ex.Message}");
            }
        }
    }
}