using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using CarAuctionManagementSystem.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRepositories();
builder.Services.AddServices();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.UseSwagger();
app.UseSwaggerUI();

await app.RunAsync();

[ExcludeFromCodeCoverage]
public partial class Program
{
}
