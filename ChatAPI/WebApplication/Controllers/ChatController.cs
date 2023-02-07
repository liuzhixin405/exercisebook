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
        public IActionResult GetResult([FromBody] string prompt)
        {
            //your OpenAI API key
            string apiKey = "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";
            string answer = string.Empty;
            var openai = new OpenAIAPI(apiKey);
            CompletionRequest completion = new CompletionRequest();
            completion.Prompt = prompt;
            completion.Model = OpenAI_API.Models.Model.DavinciText;
            completion.MaxTokens = 4000;
            var result = openai.Completions.CreateCompletionAsync(completion);
            if (result != null)
            {
                foreach (var item in result.Result.Completions)
                {
                    answer = item.Text;
                }
                return Ok(answer);
            }
            else
            {
                return BadRequest("Not found");
            }
        }

        [HttpGet]
        [Route("")]
        public IActionResult Get( string q)
        {
            if (string.IsNullOrEmpty(q))
            {
                return Ok("你还没问问题呢。比如这样问:" +
                    "http://www.eiza.net/chat?q=今天天气如何?" +
                    "");
            }
            //your OpenAI API key
            string apiKey = "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";
            string answer = string.Empty;
            var openai = new OpenAIAPI(apiKey);
            CompletionRequest completion = new CompletionRequest();
            completion.Prompt = q;
            completion.Model = OpenAI_API.Models.Model.DavinciText;
            completion.MaxTokens = 4000;
            var result = openai.Completions.CreateCompletionAsync(completion);
            if (result != null)
            {
                foreach (var item in result.Result.Completions)
                {
                    answer = item.Text;
                }
                return Ok(answer);
            }
            else
            {
                return BadRequest("Not found");
            }
        }
    }
}
