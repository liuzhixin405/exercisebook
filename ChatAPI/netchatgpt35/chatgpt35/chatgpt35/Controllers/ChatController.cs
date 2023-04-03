using Microsoft.AspNetCore.Mvc;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.Interfaces;

namespace chatgpt35.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<string>> Chat(string q, [FromServices] IOpenAIService openAiService)
        {
           
            openAiService.SetDefaultModelId(Models.Davinci);
            var completionResult = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = new List<ChatMessage>
                                                {
                                                    ChatMessage.FromUser(q)
                                                },
                Model = Models.ChatGpt3_5Turbo,
                MaxTokens = 50//optional
            });
            if (completionResult.Successful)
            {
                return (completionResult.Choices.First().Message.Content);
            }
            return "sorry falied ! please retry leater";
        }
    }
}