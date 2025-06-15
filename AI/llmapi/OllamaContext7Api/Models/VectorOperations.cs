using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OllamaContext7Api.Models
{
    public static class VectorOperations
    {
        public static async Task<List<float>> GetEmbeddingAsync(HttpClient httpClient, string text, string modelUrl, string embeddingModelName)
        {
            var request = new
            {
                model = embeddingModelName,
                prompt = text
            };
            var response = await httpClient.PostAsJsonAsync(modelUrl, request);
            response.EnsureSuccessStatusCode();

            var embeddingResponse = await response.Content.ReadFromJsonAsync<OllamaEmbeddingResponse>();
            return embeddingResponse?.Embedding ?? new List<float>();
        }

        public static double CosineSimilarity(List<float> vector1, List<float> vector2)
        {
            if (vector1 == null || vector2 == null || vector1.Count == 0 || vector1.Count != vector2.Count)
            {
                return 0.0;
            }

            double dotProduct = 0.0;
            double magnitude1 = 0.0;
            double magnitude2 = 0.0;

            for (int i = 0; i < vector1.Count; i++)
            {
                dotProduct += vector1[i] * vector2[i];
                magnitude1 += Math.Pow(vector1[i], 2);
                magnitude2 += Math.Pow(vector2[i], 2);
            }

            magnitude1 = Math.Sqrt(magnitude1);
            magnitude2 = Math.Sqrt(magnitude2);

            if (magnitude1 == 0 || magnitude2 == 0)
            {
                return 0.0;
            }

            return dotProduct / (magnitude1 * magnitude2);
        }
    }
} 