namespace WebApi.Models
{
    public class ChatCompletionRequest
    {
        public virtual string? Prompt { get; set; }
    }

    public class ChatCompletionResponse
    {
        public virtual string? Completion { get; set; }
    }
}