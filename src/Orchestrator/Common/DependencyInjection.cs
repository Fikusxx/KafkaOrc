using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Orchestrator.StateMachine.Scheduler;
using Serilog;

namespace Orchestrator.Common;

internal static class DependencyInjection
{
    public static void AddCommonServices(this IServiceCollection services, IConfiguration configuration,
        IHostBuilder hostBuilder)
    {
        hostBuilder.AddLogging();

        services.AddHealthCheckOptions(configuration);
        services.AddHealthChecks()
            .AddPostgresHealthCheck(configuration)
            .AddKafkaHealthChecks(configuration);
    }

    private static void AddLogging(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((ctx, cfg) => { cfg.ReadFrom.Configuration(ctx.Configuration); });
    }

    private static IHealthChecksBuilder AddPostgresHealthCheck(this IHealthChecksBuilder builder,
        IConfiguration configuration)
    {
        var options = configuration.GetSection(nameof(SchedulerOptions)).Get<SchedulerOptions>();
        
        if (options is null)
            throw new Exception($"{nameof(SchedulerOptions)} has not been resolved or configured.");

        builder.AddNpgSql(connectionString: options.Database.ConnectionString,
            name: "Postgres",
            failureStatus: HealthStatus.Unhealthy);

        return builder;
    }

    private static IHealthChecksBuilder AddKafkaHealthChecks(this IHealthChecksBuilder builder,
        IConfiguration configuration)
    {
        var options = configuration.GetSection(nameof(HealthCheckOptions)).Get<HealthCheckOptions>();

        if (options is null)
            throw new Exception($"{nameof(HealthCheckOptions)} has not been resolved or configured.");

        builder.AddKafka(config: options.KafkaOptions.ProducerConfig,
            name: "Kafka",
            failureStatus: HealthStatus.Unhealthy,
            timeout: TimeSpan.FromMilliseconds(options.KafkaOptions.TimeoutMs));

        return builder;
    }

    private static void AddHealthCheckOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<HealthCheckOptions>(configuration.GetSection(nameof(HealthCheckOptions)))
            .AddOptionsWithValidateOnStart<HealthCheckOptions>()
            .ValidateDataAnnotations();

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<HealthCheckOptions>>().Value;
            return options;
        });
    }
}