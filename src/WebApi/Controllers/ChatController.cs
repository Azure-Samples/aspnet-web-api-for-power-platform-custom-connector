using System.Net.Http.Headers;

using Microsoft.AspNetCore.Mvc;

using WebApi.Configurations;
using WebApi.Models;
using WebApi.References.AzureOpenAI;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class ChatController : ControllerBase
    {
        private readonly AzureOpenAISettings _settings;
        private readonly HttpClient _http;
        private readonly ILogger<ChatController> _logger;

        public ChatController(AzureOpenAISettings settings, IHttpClientFactory httpClientFactory, ILogger<ChatController> logger)
        {
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this._http = (httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory))).CreateClient();
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // POST api/<ChatController>
        [HttpPost()]
        [ProducesResponseType(typeof(ChatCompletionResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Post([FromBody] ChatCompletionRequest value)
        {
            //var accessToken = this.Request.Headers["x-aoai-access-token"].ToString();
            var apiKey = this._settings?.ApiKey;
            //var apiKey = this.Request.Headers["x-aoai-api-key"].ToString();
            var deploymentId = this._settings?.DeploymentId;
            var apiVersion = this._settings?.ApiVersion;

            //if (CheckToken(accessToken, apiKey) == TokenExists.None)
            //{
            //    return new BadRequestObjectResult("Please provide a valid security measure");
            //}
            //if (CheckToken(accessToken, apiKey) == TokenExists.Both || CheckToken(accessToken, apiKey) == TokenExists.AccessToken)
            //{
            //    this._http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            //}
            //if (CheckToken(accessToken, apiKey) == TokenExists.ApiKey)
            //{
            this._http.DefaultRequestHeaders.Add("api-key", apiKey);
            //}

            var body = new Body3()
            {
                Messages = new List<Messages>()
                {
                    new Messages()
                    {
                        Role = MessagesRole.User,
                        Content = value.Prompt
                    },
                },
                Temperature = 0.7,
                N = 1,
                Max_tokens = 800,
            };
            var chatCompletion = new ChatCompletionsClient(this._http) { BaseUrl = this._settings?.BaseUrl };
            var response = await chatCompletion.CreateAsync(deployment_id: deploymentId, api_version: apiVersion, body: body);

            var result = response.Choices
                                 .FirstOrDefault()?
                                 .Message
                                 .Content;
            var res = new ChatCompletionResponse() { Completion = result };

            return new OkObjectResult(res);
        }

        private static TokenExists CheckToken(string accessToken, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(accessToken) != true && string.IsNullOrWhiteSpace(apiKey) != true)
            {
                return TokenExists.Both;
            }
            if (string.IsNullOrWhiteSpace(accessToken) != true)
            {
                return TokenExists.AccessToken;
            }
            if (string.IsNullOrWhiteSpace(apiKey) != true)
            {
                return TokenExists.ApiKey;
            }
            return TokenExists.None;
        }
    }

    internal enum TokenExists
    {
        None,
        AccessToken,
        ApiKey,
        Both
    }
}