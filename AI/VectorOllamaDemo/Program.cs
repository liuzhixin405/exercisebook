using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OllamaEmbedExample
{
    // 定义请求和响应的数据模型
    public class EmbedRequest
    {
        public string model { get; set; }
        public string prompt { get; set; }
    }

    public class EmbedResponse
    {
        public double[] embedding { get; set; }
    }

    class Program
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string OLLAMA_BASE_URL = "http://localhost:11434";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Ollama Nomic Embed Text 向量生成示例");
            Console.WriteLine("=====================================");

            try
            {
                // 测试文本列表
                string[] testTexts = {
                    "Hello, how are you today?",
                    "你好，今天天气怎么样？",
                    "The weather is nice today.",
                    "Machine learning is fascinating.",
                    "人工智能正在改变世界。"
                };

                // 为每个文本生成向量
                foreach (var text in testTexts)
                {
                    Console.WriteLine($"\n处理文本: \"{text}\"");
                    
                    var embedding = await GetEmbedding(text);
                    
                    if (embedding != null)
                    {
                        Console.WriteLine($"向量维度: {embedding.Length}");
                        Console.WriteLine($"向量前10个值: [{string.Join(", ", embedding[0..Math.Min(10, embedding.Length)])}...]");
                        
                        // 计算向量的L2范数
                        double norm = CalculateL2Norm(embedding);
                        Console.WriteLine($"L2 范数: {norm:F6}");
                    }
                    else
                    {
                        Console.WriteLine("获取向量失败");
                    }
                }

                // 计算两个文本之间的相似度
                Console.WriteLine("\n=== 计算文本相似度 ===");
                await CalculateSimilarity("Hello world", "Hi there");
                await CalculateSimilarity("Machine learning", "人工智能");
                await CalculateSimilarity("The weather is nice", "It's a beautiful day");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
            }
            finally
            {
                httpClient.Dispose();
            }

            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }

        /// <summary>
        /// 获取文本的向量表示
        /// </summary>
        /// <param name="text">输入文本</param>
        /// <returns>向量数组</returns>
        static async Task<double[]> GetEmbedding(string text)
        {
            try
            {
                var request = new EmbedRequest
                {
                    model = "nomic-embed-text:latest",
                    prompt = text
                };

                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{OLLAMA_BASE_URL}/api/embeddings", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var embedResponse = JsonSerializer.Deserialize<EmbedResponse>(responseContent);
                    return embedResponse?.embedding;
                }
                else
                {
                    Console.WriteLine($"API 请求失败: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"网络请求错误: {ex.Message}");
                Console.WriteLine("请确保 Ollama 服务正在运行 (http://localhost:11434)");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取向量时发生错误: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 计算向量的L2范数
        /// </summary>
        /// <param name="vector">向量</param>
        /// <returns>L2范数</returns>
        static double CalculateL2Norm(double[] vector)
        {
            double sum = 0;
            foreach (var value in vector)
            {
                sum += value * value;
            }
            return Math.Sqrt(sum);
        }

        /// <summary>
        /// 计算两个向量的余弦相似度
        /// </summary>
        /// <param name="vector1">向量1</param>
        /// <param name="vector2">向量2</param>
        /// <returns>余弦相似度值</returns>
        static double CalculateCosineSimilarity(double[] vector1, double[] vector2)
        {
            if (vector1.Length != vector2.Length)
            {
                throw new ArgumentException("向量维度不匹配");
            }

            double dotProduct = 0;
            double norm1 = 0;
            double norm2 = 0;

            for (int i = 0; i < vector1.Length; i++)
            {
                dotProduct += vector1[i] * vector2[i];
                norm1 += vector1[i] * vector1[i];
                norm2 += vector2[i] * vector2[i];
            }

            return dotProduct / (Math.Sqrt(norm1) * Math.Sqrt(norm2));
        }

        /// <summary>
        /// 计算两个文本之间的相似度
        /// </summary>
        /// <param name="text1">文本1</param>
        /// <param name="text2">文本2</param>
        static async Task CalculateSimilarity(string text1, string text2)
        {
            Console.WriteLine($"\n比较文本相似度:");
            Console.WriteLine($"文本1: \"{text1}\"");
            Console.WriteLine($"文本2: \"{text2}\"");

            var embedding1 = await GetEmbedding(text1);
            var embedding2 = await GetEmbedding(text2);

            if (embedding1 != null && embedding2 != null)
            {
                var similarity = CalculateCosineSimilarity(embedding1, embedding2);
                Console.WriteLine($"余弦相似度: {similarity:F6}");
                
                // 相似度解释
                if (similarity > 0.8)
                    Console.WriteLine("相似度: 很高");
                else if (similarity > 0.6)
                    Console.WriteLine("相似度: 较高");
                else if (similarity > 0.4)
                    Console.WriteLine("相似度: 中等");
                else if (similarity > 0.2)
                    Console.WriteLine("相似度: 较低");
                else
                    Console.WriteLine("相似度: 很低");
            }
            else
            {
                Console.WriteLine("无法计算相似度 - 向量获取失败");
            }
        }
    }
}