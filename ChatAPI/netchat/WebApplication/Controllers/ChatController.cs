using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenAI_API.Completions;
using OpenAI_API;

namespace Chat.Controllers
{
   
    [Route("[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        [HttpPost]
        [Route("getanswer")]
        public async Task<string> GetResult([FromBody] string prompt)
        {
            //your OpenAI API key
            string apiKey = "xxxx";
            string answer = string.Empty;
            var openai = new OpenAIAPI(apiKey);
            CompletionRequest completion = new CompletionRequest();
            completion.Prompt = prompt;
            completion.Model = OpenAI_API.Models.Model.DavinciText;

            completion.MaxTokens = 4000;
            var result =await openai.Completions.CreateCompletionAsync(completion);
            if (result != null)
            {
                foreach (var item in result.Completions)
                {
                    answer = item.Text;
                }
                return (answer);
            }
            else
            {
                return ("Not found");
            }
        }

        [HttpGet]
        [Route("")]
        public async Task<string> Get( string q)
        {
            if (string.IsNullOrEmpty(q))
            {
                return ("你还没问问题呢。比如这样问:" +
                    "http://www.eiza.net/chat?q=今天天气如何?" +
                    "");
            }
            //your OpenAI API key
            string apiKey = "xxxxxxxxxxxxxxxxx";
            string answer = string.Empty;
            var openai = new OpenAIAPI(apiKey);
            CompletionRequest completion = new CompletionRequest();
            completion.Prompt = q;
            completion.Model = OpenAI_API.Models.Model.DavinciText;
            completion.MaxTokens = 4000;
            var result =await openai.Completions.CreateCompletionAsync(completion);
            if (result != null)
            {
                foreach (var item in result.Completions)
                {
                    answer = item.Text;
                }
                return (answer);
            }
            else
            {
                return ("Not found");
            }
        }
    }
}
