using YandexDiskBridge.API.Services;

namespace YandexDiskBridge.API;

public class Startup
{
    public IConfiguration Configuration { get; }
    public IWebHostEnvironment Environment { get; }


    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
    }


    public void ConfigureServices(IServiceCollection services)
    {
        RegisterServices(services);

        SwaggerStartup.ConfigureServices(services);
    }


    private void RegisterServices(IServiceCollection services)
    {
        services.AddLogging(x => { x.AddConsole(); });

        services.AddHttpClient();
        services.AddControllers();

        services.AddScoped<IYandexDiskService, YandexDiskService>();
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        SwaggerStartup.Configure(app, env);

        app.UseRouting();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}