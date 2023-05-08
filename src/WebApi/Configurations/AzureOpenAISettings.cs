namespace WebApi.Configurations
{
    public class AzureOpenAISettings
    {
        public const string Name = "AOAI";

        public virtual string? DeploymentId { get; set; }
        public virtual string? Endpoint { get; set; }
        public virtual string? ApiKey { get; set; }
    }
}