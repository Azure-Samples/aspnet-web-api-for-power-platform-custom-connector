using Azure.AI.OpenAI;
using Azure;

using WebApi.Configurations;
using WebApi.Models;

namespace WebApi.Helpers
{
    public interface IOpenAIHelper
    {
        Task<ChatCompletionResponse> GetChatCompletionAsync(string prompt);
    }

    public class OpenAIHelper : IOpenAIHelper
    {
        private readonly AzureOpenAISettings _settings;

        public OpenAIHelper(AzureOpenAISettings settings)
        {
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<ChatCompletionResponse> GetChatCompletionAsync(string prompt)
        {
            var client = this.GetOpenAIClient();

            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System, "You are a helpful assistant. You are very good at summarizing the given text into 2-3 bullet points."),
                    new ChatMessage(ChatRole.User, prompt)
                },
                MaxTokens = 800,
                Temperature = 0.7f,
                ChoicesPerPrompt = 1,
            };

            var deploymentId = this._settings?.DeploymentId;
            var result = await client.GetChatCompletionsAsync(deploymentId, chatCompletionsOptions);
            var content = result.Value.Choices[0].Message.Content;

            var res = new ChatCompletionResponse() { Completion = content };

            return res;
        }

        private OpenAIClient GetOpenAIClient()
        {
            var endpoint = new Uri(this._settings?.Endpoint);
            var credential = new AzureKeyCredential(this._settings?.ApiKey);

            var client = new OpenAIClient(endpoint, credential);

            return client;
        }
    }
}