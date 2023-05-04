using Microsoft.AspNetCore.Rewrite;
using Microsoft.OpenApi.Models;

using WebApi.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
    option.AddSecurityDefinition("access_token", new OpenApiSecurityScheme
    {
        Name = "x-aoai-access-token",
        Description = "Please enter valid access token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    });
    option.AddSecurityDefinition("api_key", new OpenApiSecurityScheme
    {
        Name = "x-aoai-api-key",
        Description = "Please enter valid API Key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "access_token"
                }
            },
            new string[]{}
        }
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "api_key"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddHttpClient();

var settings = new AzureOpenAISettings();
builder.Configuration.GetSection(AzureOpenAISettings.Name).Bind(settings);
builder.Services.AddSingleton(settings);

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