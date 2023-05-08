using Microsoft.AspNetCore.Mvc;

using WebApi.Configurations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    public class BaseController : ControllerBase
    {
        public BaseController(AuthSettings settings)
        {
            this.AuthSettings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.OpenApiSettings = new OpenApiSettings();
        }

        public BaseController(AuthSettings settings, OpenApiSettings openapi)
        {
            this.AuthSettings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.OpenApiSettings = openapi ?? throw new ArgumentNullException(nameof(openapi));
        }

        protected AuthSettings AuthSettings { get; }
        protected OpenApiSettings OpenApiSettings { get; }
    }
}