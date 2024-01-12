using System.Diagnostics;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddApiVersioning()
    .AddMvc()
    .AddApiExplorer(opt =>
    {
        opt.FormatGroupName = (name, version) => $"{name}_{version}";
    });

builder.Services.ConfigureOptions<SwaggerGenOptionsConfigurator>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        foreach (var apiVersionDescription in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            opt.SwaggerEndpoint($"/swagger/{apiVersionDescription.GroupName}/swagger.json", apiVersionDescription.GroupName);
        }
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


class SwaggerGenOptionsConfigurator : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider;

    public SwaggerGenOptionsConfigurator(IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        _apiVersionDescriptionProvider = apiVersionDescriptionProvider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        // fails
        Assert( _apiVersionDescriptionProvider.ApiVersionDescriptions.Count == 1);

        foreach (var apiVersionDescription in _apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            Debug.WriteLine(apiVersionDescription.GroupName);

            options.SwaggerDoc(apiVersionDescription.GroupName, new OpenApiInfo());
        }
    }

    private static void Assert(bool condition)
    {
        if (!condition)
        {
            throw new ArgumentException("Assertion failed.", nameof(condition));
        }
    }
}
