using Microsoft.AspNetCore.Mvc;

using WebApi.Configurations;
using WebApi.Extensions;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IValidationService
    {
        ValidationResult<T> Validate<T>(IHeaderDictionary headers) where T : ApiRequestHeaders;
    }

    public class ValidationService : IValidationService
    {
        private readonly AuthSettings _settings;

        public ValidationService(AuthSettings settings)
        {
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public ValidationResult<T> Validate<T>(IHeaderDictionary requestHeaders) where T : ApiRequestHeaders
        {
            var headers = requestHeaders.ToObject<T>();
            var result = new ValidationResult<T>() { Headers = headers };
            //#if !DEBUG
            var apiKey = headers.ApiKey;
            if (string.IsNullOrWhiteSpace(apiKey) == true)
            {
                var error = new ErrorResponse() { Message = "Invalid API Key" };
                result.ActionResult = new UnauthorizedObjectResult(error);

                return result;
            }
            if (apiKey != this._settings.ApiKey)
            {
                var error = new ErrorResponse() { Message = "Invalid API Key" };
                result.ActionResult = new UnauthorizedObjectResult(error);

                return result;
            }
            //#endif
            if (headers is not GitHubApiRequestHeaders)
            {
                result.Validated = true;

                return result;
            }

            var gitHubToken = (headers as GitHubApiRequestHeaders).GitHubToken;

            if (string.IsNullOrWhiteSpace(gitHubToken) == true)
            {
                var error = new ErrorResponse() { Message = "Invalid GitHub Token" };
                result.ActionResult = new ObjectResult(error) { StatusCode = StatusCodes.Status403Forbidden };

                return result;
            }

            result.Validated = true;

            return result;
        }
    }
}