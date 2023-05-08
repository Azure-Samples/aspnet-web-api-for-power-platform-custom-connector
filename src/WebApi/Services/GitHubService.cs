using Octokit;

using WebApi.Configurations;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IGitHubService
    {
        Task<GitHubIssueCollectionResponse> GetIssuesAsync(GitHubApiRequestHeaders headers);

        Task<GitHubIssueItemResponse> GetIssueAsync(int id, GitHubApiRequestHeaders headers);

        Task<GitHubIssueItemSummaryResponse> GetIssueSummaryAsync(int id, GitHubApiRequestHeaders headers);
    }

    public class GitHubService : IGitHubService
    {
        private readonly GitHubSettings _settings;
        private readonly IOpenAIHelper _helper;

        public GitHubService(GitHubSettings settings, IOpenAIHelper helper)
        {
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this._helper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        public async Task<GitHubIssueCollectionResponse> GetIssuesAsync(GitHubApiRequestHeaders headers)
        {
            var user = this._settings.User;
            var repository = this._settings.Repository;

            var github = this.GetGitHubClient(headers);

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

            return res;
        }

        public async Task<GitHubIssueItemResponse> GetIssueAsync(int id, GitHubApiRequestHeaders headers)
        {
            var user = this._settings.User;
            var repository = this._settings.Repository;

            var github = this.GetGitHubClient(headers);

            var issue = await github.Issue.Get(user, repository, id);
            var res = new GitHubIssueItemResponse()
            {
                Id = issue.Id,
                Number = issue.Number,
                Title = issue.Title,
                Body = issue.Body,
            };

            return res;
        }

        public async Task<GitHubIssueItemSummaryResponse> GetIssueSummaryAsync(int id, GitHubApiRequestHeaders headers)
        {
            var issue = await this.GetIssueAsync(id, headers);
            var prompt = issue.Body;
            var completion = await this._helper.GetChatCompletionAsync(prompt);

            var res = new GitHubIssueItemSummaryResponse()
            {
                Id = issue.Id,
                Number = issue.Number,
                Title = issue.Title,
                Body = issue.Body,
                Summary = completion.Completion,
            };

            return res;
        }

        private IGitHubClient GetGitHubClient(GitHubApiRequestHeaders headers)
        {
            var accessToken = headers.GitHubToken;
            var credentials = new Credentials(accessToken, AuthenticationType.Bearer);
            var agent = this._settings.Agent.Replace(" ", "").Trim();
            var github = new GitHubClient(new ProductHeaderValue(agent))
            {
                Credentials = credentials
            };

            return github;
        }
    }
}