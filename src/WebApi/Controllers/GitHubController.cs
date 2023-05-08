using Microsoft.AspNetCore.Mvc;

using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GitHubController : ControllerBase
    {
        private readonly IValidationService _validation;
        private readonly IGitHubService _github;
        private readonly IOpenAIService _openai;
        private readonly ILogger<GitHubController> _logger;

        public GitHubController(IValidationService validation, IGitHubService github, IOpenAIService openai, ILogger<GitHubController> logger)
        {
            this._validation = validation ?? throw new ArgumentNullException(nameof(validation));
            this._github = github ?? throw new ArgumentNullException(nameof(github));
            this._openai = openai ?? throw new ArgumentNullException(nameof(openai));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("issues")]
        [ProducesResponseType(typeof(GitHubIssueCollectionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetIssues()
        {
            var validation = this._validation.Validate<GitHubApiRequestHeaders>(this.Request.Headers);
            if (validation.Validated != true)
            {
                return await Task.FromResult(validation.ActionResult);
            }

            var res = await this._github.GetIssuesAsync(validation.Headers);

            return new OkObjectResult(res);
        }

        [HttpGet("issues/{id}")]
        [ProducesResponseType(typeof(GitHubIssueItemResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetIssue(int id)
        {
            var validation = this._validation.Validate<GitHubApiRequestHeaders>(this.Request.Headers);
            if (validation.Validated != true)
            {
                return await Task.FromResult(validation.ActionResult);
            }

            var res = await this._github.GetIssueAsync(id, validation.Headers);

            return new OkObjectResult(res);
        }

        [HttpGet("issues/{id}/summary")]
        [ProducesResponseType(typeof(GitHubIssueItemSummaryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetIssueSummary(int id)
        {
            var validation = this._validation.Validate<GitHubApiRequestHeaders>(this.Request.Headers);
            if (validation.Validated != true)
            {
                return await Task.FromResult(validation.ActionResult);
            }

            var res = await this._github.GetIssueSummaryAsync(id, validation.Headers);

            return new OkObjectResult(res);
        }
    }
}