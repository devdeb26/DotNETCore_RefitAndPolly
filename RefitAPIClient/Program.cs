using Polly;
using Polly.Extensions.Http;
using Refit;
using RefitAPIClient.Services.Interface;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var authToken = "***WRITE YOUR API READ ACCESS TOKEN HERE***";
var refitSettings = new RefitSettings()
{
    AuthorizationHeaderValueGetter = (rq, ct) => Task.FromResult(authToken),
};

// var refitSettings = new RefitSettings()
// {
//     AuthorizationHeaderValueGetter = (rq, ct) => GetTokenAsync(),
// };

builder.Services.AddRefitClient<ITmdbAPI>(refitSettings)
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:5000"));

// Define retry policy
var retryPolicy = HttpPolicyExtensions
.HandleTransientHttpError()
.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

// Define circuit breaker policy
var circuitBreakerPolicy = HttpPolicyExtensions
.HandleTransientHttpError()
.CircuitBreakerAsync(2, TimeSpan.FromSeconds(30));

builder.Services.AddHttpClient("MyHttpClient")
.AddPolicyHandler(retryPolicy)
.AddPolicyHandler(circuitBreakerPolicy);

var app = builder.Build();

app.MapGet("/", async (IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient("MyHttpClient"); //this has been called in addhttpclient service where policyhandler retrypolicy and circuitbreakerpolicy is 
    var response = await client.GetAsync("https://example.com/api/data");
    return response.IsSuccessStatusCode ? "Success": "Failed";
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
