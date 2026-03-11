using System.Diagnostics;
using System;
using System.IO;

namespace Agent
{
    public static class Tools
    {
        public static string ReadFile(string[] args)
        {
            if (args.Length == 0)
                throw new ArgumentException("需要提供文件路径参数");

            var filePath = args[0];
            
            // 如果是相对路径，优先在当前工作目录中查找
            if (!Path.IsPathRooted(filePath))
            {
                var currentDirPath = Path.Combine(Environment.CurrentDirectory, filePath);
                if (File.Exists(currentDirPath))
                {
                    filePath = currentDirPath;
                }
                else
                {
                    throw new FileNotFoundException($"文件未找到: {filePath}");
                }
            }

            return File.ReadAllText(filePath);
        }

        public static string WriteToFile(string[] args)
        {
            if (args.Length < 2)
                throw new ArgumentException("需要提供文件路径和内容参数");

            var filePath = args[0];
            var content = args[1].Replace("\\n", "\n");
            
            // 确保目录存在
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            File.WriteAllText(filePath, content);
            return "写入成功";
        }

        public static string RunTerminalCommand(string[] args)
        {
            if (args.Length == 0)
                throw new ArgumentException("需要提供命令参数");

            var command = args[0];
            
            var startInfo = new ProcessStartInfo
            {
                FileName = GetShell(),
                Arguments = GetShellArguments(command),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8,
                StandardErrorEncoding = System.Text.Encoding.UTF8
            };

            using var process = Process.Start(startInfo);
            if (process == null)
                return "EXIT_CODE:-1\n执行失败";

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            // 返回带有退出码的结构化文本，便于上层判断是否成功
            return $"EXIT_CODE:{process.ExitCode}\n{output}{error}";
        }

        private static string GetShell()
        {
            return OperatingSystem.IsWindows() ? "cmd.exe" : "/bin/bash";
        }

        private static string GetShellArguments(string command)
        {
            return OperatingSystem.IsWindows() ? $"/c {command}" : $"-c \"{command}\"";
        }
    }
}