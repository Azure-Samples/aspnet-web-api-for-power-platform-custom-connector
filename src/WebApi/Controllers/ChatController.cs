using Azure;
using Azure.AI.OpenAI;

using Microsoft.AspNetCore.Mvc;

using WebApi.Configurations;
using WebApi.Extensions;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class ChatController : BaseController
    {
        private readonly AzureOpenAISettings _settings;
        private readonly ILogger<ChatController> _logger;

        public ChatController(AzureOpenAISettings settings, AuthSettings auth, ILogger<ChatController> logger)
            : base(auth)
        {
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // POST api/<ChatController>
        [HttpPost("completions")]
        [ProducesResponseType(typeof(ChatCompletionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Post([FromBody] ChatCompletionRequest req)
        {
#if !DEBUG
            var headers = this.Request.Headers.ToObject<ChatCompletionRequestHeaders>();
            var apiKey = headers.ApiKey;
            if (string.IsNullOrWhiteSpace(apiKey) == true)
            {
                var error = new ErrorResponse() { Message = "Invalid API Key" };
                return new UnauthorizedObjectResult(error);
            }
            if (apiKey != this.AuthSettings.ApiKey)
            {
                var error = new ErrorResponse() { Message = "Invalid API Key" };
                return new UnauthorizedObjectResult(error);
            }
#endif

#pragma warning disable CS8604 // Possible null reference argument.
            var endpoint = new Uri(this._settings?.Endpoint);
            var credential = new AzureKeyCredential(this._settings?.ApiKey);
            var deploymentId = this._settings?.DeploymentId;
#pragma warning restore CS8604 // Possible null reference argument.

            var client = new OpenAIClient(endpoint, credential);

            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System, "You are a helpful assistant."),
                    new ChatMessage(ChatRole.User, req.Prompt)
                },
                MaxTokens = 800,
                Temperature = 0.7f,
                ChoicesPerPrompt = 1,
            };

            var result = await client.GetChatCompletionsAsync(deploymentId, chatCompletionsOptions);
            var content = result.Value.Choices[0].Message.Content;

            var res = new ChatCompletionResponse() { Completion = content };

            return new OkObjectResult(res);
        }
    }
}