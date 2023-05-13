using Microsoft.AspNetCore.Mvc;

using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class ChatController : ControllerBase
    {
        private readonly IValidationService _validation;
        private readonly IOpenAIService _openai;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IValidationService validation, IOpenAIService openai, ILogger<ChatController> logger)
        {
            this._validation = validation ?? throw new ArgumentNullException(nameof(validation));
            this._openai = openai ?? throw new ArgumentNullException(nameof(openai));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("completions", Name = "ChatCompletions")]
        [ProducesResponseType(typeof(ChatCompletionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Post([FromBody] ChatCompletionRequest req)
        {
            var validation = this._validation.ValidateHeaders<ChatCompletionRequestHeaders>(this.Request.Headers);
            if (validation.Validated != true)
            {
                return await Task.FromResult(validation.ActionResult);
            }

            var pvr = this._validation.ValidatePayload(req);
            if (pvr.Validated != true)
            {
                return await Task.FromResult(pvr.ActionResult);
            }

            var res = await this._openai.GetChatCompletionAsync(pvr.Payload.Prompt);

            return new OkObjectResult(res);
        }
    }
}