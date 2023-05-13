using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class ChatCompletionRequest : ApiPayload
    {
        [Required]
        public virtual string? Prompt { get; set; }
    }
}