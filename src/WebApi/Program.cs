using Microsoft.AspNetCore.Rewrite;
using Microsoft.OpenApi.Models;

using WebApi.Configurations;
using WebApi.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "GitHub Issue Sentiment Analysis", Version = "v1" });

    option.OperationFilter<WebApiKeyAuthorizationOperationFilter>();
    // Add operation-level GitHub Token security requirement
    var gitHubSecuritySchemeReference = new OpenApiReference
    {
        Id = "github_token",
        Type = ReferenceType.SecurityScheme
    };
    var gitHubSecurityScheme = new OpenApiSecurityScheme
    {
        Name = "x-github-token",
        Description = "Please enter valid GitHub Token",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Reference = gitHubSecuritySchemeReference
    };
    option.AddSecurityDefinition(gitHubSecuritySchemeReference.Id, gitHubSecurityScheme);

    // Add global API Key security requirement
    var webApiKeySecuritySchemeReference = new OpenApiReference
    {
        Id = "api_key",
        Type = ReferenceType.SecurityScheme
    };
    var webApiKeySecurityScheme = new OpenApiSecurityScheme
    {
        Name = "x-webapi-key",
        Description = "Please enter valid API Key",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Reference = webApiKeySecuritySchemeReference
    };
    option.AddSecurityDefinition(webApiKeySecuritySchemeReference.Id, webApiKeySecurityScheme);
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = webApiKeySecuritySchemeReference
            },
            new string[]{}
        }
    });
});

builder.Services.AddHttpClient();

var aoaiSettings = new AzureOpenAISettings();
builder.Configuration.GetSection(AzureOpenAISettings.Name).Bind(aoaiSettings);
builder.Services.AddSingleton(aoaiSettings);

var authSettings = new AuthSettings();
builder.Configuration.GetSection(AuthSettings.Name).Bind(authSettings);
builder.Services.AddSingleton(authSettings);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // add a new rewrite option to redirect the root to /swagger
    app.UseRewriter(new RewriteOptions().AddRedirect("^$", "swagger"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();