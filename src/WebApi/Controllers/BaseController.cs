using Azure;
using Azure.AI.OpenAI;

using Microsoft.AspNetCore.Mvc;

using WebApi.Configurations;
using WebApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    public class BaseController : ControllerBase
    {
        public BaseController(AuthSettings settings, IHttpClientFactory httpClientFactory)
        {
            this.AuthSettings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.HttpClient = (httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory))).CreateClient();
        }

        protected AuthSettings AuthSettings { get; }
        protected HttpClient HttpClient { get; }
    }
}