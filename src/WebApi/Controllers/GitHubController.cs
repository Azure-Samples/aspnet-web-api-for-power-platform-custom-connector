using Microsoft.AspNetCore.Mvc;

using Octokit;

using WebApi.Configurations;
using WebApi.Extensions;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GitHubController : BaseController
    {
        private readonly GitHubSettings _settings;
        private readonly ILogger<GitHubController> _logger;

        public GitHubController(GitHubSettings settings, AuthSettings auth, OpenApiSettings openapi, ILogger<GitHubController> logger)
            : base(auth, openapi)
        {
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/<GitHubController>
        [HttpGet("issues")]
        public async Task<IActionResult> Get()
        {
            var headers = this.Request.Headers.ToObject<GitHubApiRequestHeaders>();
#if !DEBUG
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
            var gitHubToken = headers.GitHubToken;
            if (string.IsNullOrWhiteSpace(gitHubToken) == true)
            {
                var error = new ErrorResponse() { Message = "Invalid GitHub Token" };
                return new ObjectResult(error) { StatusCode = StatusCodes.Status403Forbidden };
            }

            var user = this._settings.User;
            var repository = this._settings.Repository;
            var credentials = new Credentials(gitHubToken, AuthenticationType.Bearer);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var agent = this.OpenApiSettings.Title
                            .Replace("GitHub", "", StringComparison.InvariantCultureIgnoreCase)
                            .Replace(" ", "")
                            .Trim();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var github = new GitHubClient(new ProductHeaderValue(agent))
            {
                Credentials = credentials
            };

            var issues = await github.Issue.GetAllForRepository(user, repository);
            var res = new GitHubIssueCollectionResponse()
            {
                Items = issues.Select(p => new GitHubIssueItemResponse()
                {
                    Id = p.Id,
                    Number = p.Number,
                    Title = p.Title,
                    Body = p.Body,
                })
            };

            return new OkObjectResult(res);
        }

        // GET api/<GitHubController>/5
        [HttpGet("issues/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var headers = this.Request.Headers.ToObject<GitHubApiRequestHeaders>();
#if !DEBUG
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
            var gitHubToken = headers.GitHubToken;
            if (string.IsNullOrWhiteSpace(gitHubToken) == true)
            {
                var error = new ErrorResponse() { Message = "Invalid GitHub Token" };
                return new ObjectResult(error) { StatusCode = StatusCodes.Status403Forbidden };
            }

            var user = this._settings.User;
            var repository = this._settings.Repository;
            var credentials = new Credentials(gitHubToken, AuthenticationType.Bearer);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var agent = this.OpenApiSettings.Title
                            .Replace("GitHub", "", StringComparison.InvariantCultureIgnoreCase)
                            .Replace(" ", "")
                            .Trim();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var github = new GitHubClient(new ProductHeaderValue(agent))
            {
                Credentials = credentials
            };

            var issue = await github.Issue.Get(user, repository, id);
            var res = new GitHubIssueItemResponse()
            {
                Id = issue.Id,
                Number = issue.Number,
                Title = issue.Title,
                Body = issue.Body,
            };

            return new OkObjectResult(res);
        }
    }
}