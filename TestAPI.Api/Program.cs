var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Test PR Agent API is running.");

app.MapGet("/weatherforecast", () =>
{
    var forecast = new[]
    {
        new { Date = DateTime.Now.AddDays(1), TemperatureC = 32, Summary = "Hot" },
        new { Date = DateTime.Now.AddDays(2), TemperatureC = 22, Summary = "Warm" },
        new { Date = DateTime.Now.AddDays(3), TemperatureC = 15, Summary = "Cool" }
    };
    
    return Results.Ok(forecast);
});

app.Run();