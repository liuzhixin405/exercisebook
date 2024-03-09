using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly string _webRootPath;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _webRootPath = hostingEnvironment.WebRootPath;
        }
        public class FileDto
        {
            public string Name { get; set; }
        }

        [HttpPost(Name = "GetWeatherForecast")]
        public async Task<string> Get(IFormCollection formCollection)
        {

            //FormCollectionת��ΪFormFileCollection
            var files = (FormFileCollection)formCollection.Files;

            //if (files.Any())
            var pars = files.Where(x => x.Name.ToLower().Equals("parameters")).FirstOrDefault();
            using (Stream stream = pars.OpenReadStream())
            {
                var dto = System.Text.Json.JsonSerializer.Deserialize<FileDto>(stream);
            }
            foreach (var file in files.Where(x => !x.Name.ToLower().Equals("parameters")).ToList())
            {
                string path = $"/Upload//{file.FileName}";

                string physicPath = $"{_webRootPath}/{path}";
                string dir = Path.GetDirectoryName(physicPath);
                //����ÿ�մ洢�ļ���
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                //�ж��ļ���С
                var fileSize = file.Length;

                if (fileSize > 1024 * 1024 * 10) //10M TODO:(1mb=1024X1024b)
                {
                    continue;
                }

                //�ļ�����
                //using (var fs = System.IO.File.Create(webRootPath + filePath + file.FileName))
                using (var fs = new FileStream(physicPath, FileMode.Create))
                {
                    await file.CopyToAsync(fs);
                    await fs.FlushAsync();
                }

                //�������ļ�·��

            }

            return "";

        }
    }
}