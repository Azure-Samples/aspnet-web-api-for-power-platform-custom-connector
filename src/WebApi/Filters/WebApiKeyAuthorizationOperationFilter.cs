using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Filters
{
    public class WebApiKeyAuthorizationOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Get Authorize attribute
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var attributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                                    .Union(context.MethodInfo.GetCustomAttributes(true))
                                    .OfType<AuthorizeAttribute>();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            if (attributes.Any() == false)
            {
                operation.Security.Clear();
                return;
            }

            var attr = attributes.First();

            if (attr.AuthenticationSchemes != SecuritySchemeType.ApiKey.ToString())
            {
                return;
            }

            operation.Security = new List<OpenApiSecurityRequirement>()
            {
                new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "github_token",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new string[]{}
                    }
                }
            };
        }
    }
}