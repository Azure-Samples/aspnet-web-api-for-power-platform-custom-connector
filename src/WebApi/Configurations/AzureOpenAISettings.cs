namespace WebApi.Configurations
{
    public class AzureOpenAISettings
    {
        public const string Name = "AOAI";

        public virtual string? ApiVersion { get; set; } = "2023-03-15-preview";
        public virtual string? DeploymentId { get; set; }
        public virtual string? BaseUrl { get; set; }
        public virtual string? ApiKey { get; set; }
    }
}