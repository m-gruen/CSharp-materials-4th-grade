using System.Text.Json;
using System.Text.Json.Serialization;
using NodaTime.Serialization.SystemTextJson;
using TankMuseum.Core.Exhibits;
using TankMuseum.Core.News;

namespace TankMuseum.Core;

public static class Setup
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton<IClock>(SystemClock.Instance);
        services.AddSingleton<IDataStorage, DataStorage>();

        services.AddScoped<INewsService, NewsService>();
        services.AddScoped<IExhibitService, ExhibitService>();
    }

    public static void ConfigureServices(this IServiceCollection services, bool isDevelopment)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.WriteIndented = isDevelopment;
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.SerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        });
    }

    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(Const.CorsPolicyName, policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });
    }
}
