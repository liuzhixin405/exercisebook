using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreadSamples
{
    public class AsyncIODemo
    {
        /// <summary>
        /// 异步写入文件
        /// </summary>
        /// <returns></returns>
        private static async Task WriteTextAsync()
        {
            var path = "temp.txt";
            var content = Guid.NewGuid().ToString();

            using (var fs = new FileStream(path,
                                           FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                var buffer = Encoding.UTF8.GetBytes(content);

                //var writeTask = fs.WriteAsync(buffer, 0, buffer.Length);
                //await writeTask;
                await fs.WriteAsync(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// 异步读取文本
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static async Task<string> ReadTextAsync(string fileName)
        {
            using (var fs = new FileStream(fileName,
                                           FileMode.OpenOrCreate, FileAccess.Read, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                var sb = new StringBuilder();
                var buffer = new byte[0x1000];  //十六进制 等于十进制的 4096
                var readLength = 0;

                while ((readLength = await fs.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    var text = Encoding.UTF8.GetString(buffer, 0, readLength);
                    sb.Append(text);
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// 异步写入多个文件
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private static async Task WriteMultiTextAsync(string folder)
        {
            var tasks = new List<Task>();
            var fileStreams = new List<FileStream>();

            try
            {
                for (int i = 1; i <= 10; i++)
                {
                    var fileName = Path.Combine(folder, $"{i}.txt");
                    var content = Guid.NewGuid().ToString();
                    var buffer = Encoding.UTF8.GetBytes(content);

                    var fs = new FileStream(fileName,
        FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, bufferSize: 4096, useAsync: true);
                    fileStreams.Add(fs);

                    var writeTask = fs.WriteAsync(buffer, 0, buffer.Length);
                    tasks.Add(writeTask);
                }

                await Task.WhenAll(tasks);
            }
            finally
            {
                foreach (var fs in fileStreams)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        public static void Main()
        {
            // 异步写入
            //WriteTextAsync().Wait();
            // 异步读取
            //Console.WriteLine(ReadTextAsync("temp.txt").Result);
            var folder = "temp";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            WriteMultiTextAsync(folder).Wait();
        }
    }
}
