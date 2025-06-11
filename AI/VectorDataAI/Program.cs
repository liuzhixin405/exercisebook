using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;

namespace VectorDataAI;

class Program
{
    static async Task Main(string[] args)
    {
        // Run the example.
        await RunExampleAsync();
    }

    private static async Task RunExampleAsync()
    {
        List<CloudService> cloudServices =
        [
            new() {
                Key = 0,
                Name = "Azure App Service",
                Description = "Host .NET, Java, Node.js, and Python web applications and APIs in a fully managed Azure service. You only need to deploy your code to Azure. Azure takes care of all the infrastructure management like high availability, load balancing, and autoscaling."
            },
            new() {
                Key = 1,
                Name = "Azure Service Bus",
                Description = "A fully managed enterprise message broker supporting both point to point and publish-subscribe integrations. It's ideal for building decoupled applications, queue-based load leveling, or facilitating communication between microservices."
            },
            new() {
                Key = 2,
                Name = "Azure Blob Storage",
                Description = "Azure Blob Storage allows your applications to store and retrieve files in the cloud. Azure Storage is highly scalable to store massive amounts of data and data is stored redundantly to ensure high availability."
            },
            new() {
                Key = 3,
                Name = "Microsoft Entra ID",
                Description = "Manage user identities and control access to your apps, data, and resources."
            },
            new() {
                Key = 4,
                Name = "Azure Key Vault",
                Description = "Store and access application secrets like connection strings and API keys in an encrypted vault with restricted access to make sure your secrets and your application aren't compromised."
            },
            new() {
                Key = 5,
                Name = "Azure AI Search",
                Description = "Information retrieval at scale for traditional and conversational search applications, with security and options for AI enrichment and vectorization."
            }
        ];

        // Load the configuration values.
        IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
        string model = config["ModelName"] ?? "nomic-embed-text"; // Ollama embedding model
        string ollamaUrl = config["OllamaUrl"] ?? "http://localhost:11434"; // Ollama API URL

        // Create the Ollama embedding generator.
        var ollamaGenerator = new OllamaEmbeddingGenerator(ollamaUrl, model);

        // Create and populate the vector store.
        var vectorStore = new InMemoryVectorStore();
        VectorStoreCollection<int, CloudService> cloudServicesStore =
            vectorStore.GetCollection<int, CloudService>("cloudServices");
        await cloudServicesStore.EnsureCollectionExistsAsync();

        foreach (CloudService service in cloudServices)
        {
            service.Vector = await ollamaGenerator.GenerateEmbeddingAsync(service.Description);
            await cloudServicesStore.UpsertAsync(service);
        }

        // Convert a search query to a vector and search the vector store.
        string query = "Which Azure service should I use to store my Word documents?";
        ReadOnlyMemory<float> queryEmbedding = await ollamaGenerator.GenerateEmbeddingAsync(query);

        IAsyncEnumerable<VectorSearchResult<CloudService>> results =
            cloudServicesStore.SearchAsync(queryEmbedding, top: 1);

        await foreach (VectorSearchResult<CloudService> result in results)
        {
            Console.WriteLine($"Name: {result.Record.Name}");
            Console.WriteLine($"Description: {result.Record.Description}");
            Console.WriteLine($"Vector match score: {result.Score}");
        }
    }
}

// Custom Ollama Embedding Generator
public class OllamaEmbeddingGenerator
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _model;

    public OllamaEmbeddingGenerator(string baseUrl, string model)
    {
        _httpClient = new HttpClient();
        _baseUrl = baseUrl.TrimEnd('/');
        _model = model;
    }

    public async Task<ReadOnlyMemory<float>> GenerateEmbeddingAsync(string text)
    {
        var requestBody = new
        {
            model = _model,
            prompt = text
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/api/embeddings", content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseJson = JsonDocument.Parse(responseContent);

        var embeddingArray = responseJson.RootElement.GetProperty("embedding");
        var embeddings = new float[embeddingArray.GetArrayLength()];

        for (int i = 0; i < embeddings.Length; i++)
        {
            embeddings[i] = embeddingArray[i].GetSingle();
        }

        return new ReadOnlyMemory<float>(embeddings);
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

