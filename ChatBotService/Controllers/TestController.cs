using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace ChatBotService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController(IConfiguration configuration) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;

        [HttpGet("ai")]
        public async Task<string> TestAI()
        {
            var client = new AzureOpenAIClient(
                new Uri(_configuration["AzureOpenAI:Endpoint"]),
                new ApiKeyCredential(_configuration["AzureOpenAI:OpenAiKey"]))
                .GetChatClient("gpt-4o-mini");

            var response = await client.CompleteChatAsync(
            [
                new SystemChatMessage("You are a concise, helpful assistant."),
                new UserChatMessage("Say 'this is a test' and stop.")
            ]);

            return response.Value.Content[0].Text;
        }
    }
}
