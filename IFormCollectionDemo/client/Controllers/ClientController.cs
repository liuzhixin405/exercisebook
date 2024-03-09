using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using System.IO;

namespace client.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientController : ControllerBase
    {

        private readonly ILogger<ClientController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public ClientController(ILogger<ClientController> logger,IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public class FileDto     //存放多文件上传相关信息,比如存放路径等等
        {
            public string Name { get; set; }
        }
        [HttpPost(Name = "Client")]
        public async Task Get([FromForm]FileDto dto,IFormCollection collection)
        {
            
            var mfdc = new MultipartFormDataContent();

            foreach (var item in collection.Files)
            {
                using (Stream stream = item.OpenReadStream())
                {
                    byte[] bytes = new byte[stream.Length];

                    stream.Read(bytes, 0, bytes.Length);
                    // 设置当前流的位置为流的开始 
                    stream.Seek(0, SeekOrigin.Begin);
                    //content.Add(new ByteArrayContent(bytes));
                    mfdc.Add(new ByteArrayContent(bytes), "file", item.FileName);
                }
            };
            mfdc.Add(new StringContent(System.Text.Json.JsonSerializer.Serialize(dto)),"parameters", "parameters"); //dto
           using var httpClient = _httpClientFactory.CreateClient();
            var postResponse = await httpClient.PostAsync("https://localhost:7127/WeatherForecast", mfdc);
        }
    }
}
