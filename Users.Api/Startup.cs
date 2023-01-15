using EventStore;
using EventStore.ClientAPI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Users.Application;
using WebApi;
using Users.Api.Configuration;

namespace Users.Api;
public class Startup
{
    public Startup(
        IWebHostEnvironment environment,
        IConfiguration configuration
    )
    {
        Environment = environment;
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }
    private IWebHostEnvironment Environment { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var esConnection = EventStoreConnection.Create(
            Configuration["eventStore:connectionString"],
            ConnectionSettings.Create().KeepReconnecting(),
            Environment.ApplicationName
        );

        var documentStore = RavenDbConfiguration.Configure(
            Configuration["ravenDb:server"]
        );

        services.AddSingleton(esConnection);
        services.AddSingleton(documentStore);

        services.AddSingleton<IHostedService, EventStoreService>();

        services
            .AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme
            )
            .AddCookie();

        services
            .AddMvcCore(
                options => options.Conventions.Add(new CommandConvention())
            )
            .AddApplicationPart(GetType().Assembly)
            .AddUsersModule("Users")
            .AddApiExplorer();

        services.AddSpaStaticFiles(
            configuration =>
                configuration.RootPath = "ClientApp/dist"
        );

        services.AddSwaggerGen(
            c =>
                c.SwaggerDoc(
                    "v1",
                    new()
                    {
                        Title = "ClassifiedAds",
                        Version = "v1"
                    }
                )
        );
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseStaticFiles();
        app.UseSpaStaticFiles();
        app.UseSwagger();

        app.UseSwaggerUI(
            c => c.SwaggerEndpoint(
                "/swagger/v1/swagger.json", "ClassifiedAds v1"
            )
        );
    }
}