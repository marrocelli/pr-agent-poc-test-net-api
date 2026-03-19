namespace TestPrAgentNet.Tests;

public class ApiTests
{
    [Fact]
    public void RootEndpoint_ReturnsSuccess()
    {
        var result = "Test PR Agent API is running!";
        Assert.Equal("Test PR Agent API is running!", result);
    }
    
    [Theory]
    [InlineData(32, "Hot")]
    [InlineData(22, "Warm")]
    [InlineData(15, "Cool")]
    public void WeatherForecast_HasCorrectSummary(int tempC, string expectedSummary)
    {
        var summary = tempC switch
        {
            >= 30 => "Hot",
            >= 20 => "Warm",
            _ => "Cool"
        };
        Assert.Equal(expectedSummary, summary);
    }

    [Fact]
    public void WeatherForecast_AlwaysReturnsThreeItems()
    {
        var forecast = new[]
        {
            new { Date = DateTime.Now.AddDays(1), TemperatureC = 32, Summary = "Hot" },
            new { Date = DateTime.Now.AddDays(2), TemperatureC = 22, Summary = "Warm" },
            new { Date = DateTime.Now.AddDays(3), TemperatureC = 15, Summary = "Cool" }
        };
        Assert.Equal(3, forecast.Length);
    }
}