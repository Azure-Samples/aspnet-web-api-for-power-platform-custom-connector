namespace WebApi.Configurations
{
    public class GitHubSettings
    {
        public const string Name = "GitHub";

        public virtual string? User { get; set; }
        public virtual string? Repository { get; set; }
    }
}