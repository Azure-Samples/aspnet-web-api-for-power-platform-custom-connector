namespace WebApi.Models
{
    public class GitHubIssueItemSummaryResponse : GitHubIssueItemResponse
    {
        public virtual string? Summary { get; set; }
    }
}