using System.Text.Json;
using System.Text.Json.Serialization;
using ApiResponse.Poc;
using ApiResponse.Poc.Factories;
using FluentValidation;
using Scalar.AspNetCore;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    var jsonOptions = options.SerializerOptions;
    jsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    jsonOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    jsonOptions.Converters.Add(new JsonStringEnumConverter());
    jsonOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    jsonOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    jsonOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
});
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation(c =>
{
    c.EnablePathBindingSourceAutomaticValidation = true;
    c.OverrideDefaultResultFactoryWith<CustomValidationResultFactory>();
});

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/docs", options =>
    {
        options.WithTitle("My API Documentation")
            .WithTheme(ScalarTheme.Saturn)
            .EnableDarkMode()
            .ExpandAllTags()
            .SortTagsAlphabetically()
            .SortOperationsByMethod()
            .PreserveSchemaPropertyOrder();
    });
}

app.UseRouting();
app.MapControllers();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

await app.RunAsync();