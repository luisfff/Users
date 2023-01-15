using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using static System.Environment;

namespace Users.Api;
public static class Program
{
    public static void Main()
    {
        var configuration = BuildConfiguration();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        ConfigureWebHost(configuration).Build().Run();
    }

    private static IConfiguration BuildConfiguration()
        => new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, false)
            .Build();

    private static IWebHostBuilder ConfigureWebHost(
        IConfiguration configuration)
        => new WebHostBuilder()
            .UseStartup<Startup>()
            .UseConfiguration(configuration)
            .UseContentRoot(CurrentDirectory)
            .UseSerilog()
            .UseKestrel();
}