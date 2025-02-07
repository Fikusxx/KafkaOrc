using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orchestrator.StateMachine.Core;

namespace Orchestrator.StateMachine.Database;

internal static class DependencyInjection
{
    public static IServiceCollection AddMigrations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStateMachineDatabaseOptions(configuration);
        services.AddHostedService<MigrationHostedService>();

        return services;
    }

    public static ISagaRegistrationConfigurator<CascadingCommunicationState> AddStateMachinePersistence(
        this ISagaRegistrationConfigurator<CascadingCommunicationState> saga)
    {
        saga.EntityFrameworkRepository(opt =>
        {
            opt.ConcurrencyMode = ConcurrencyMode.Optimistic;

            opt.AddDbContext<DbContext, AppDbContext>((provider, builder) =>
            {
                var options = provider.GetRequiredService<StateMachineDatabaseOptions>();

                builder.UseNpgsql(
                    options.Connection,
                    m =>
                    {
                        m.MigrationsHistoryTable("__EFMigrationsHistory", options.Schema);
                        m.EnableRetryOnFailure(maxRetryCount: options.MaxRetryCount,
                            maxRetryDelay: TimeSpan.FromMilliseconds(options.MaxRetryDelayMs),
                            null);
                    });
            });

            opt.UsePostgres();
        });

        return saga;
    }

    private static void AddStateMachineDatabaseOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<StateMachineDatabaseOptions>(configuration.GetSection(nameof(StateMachineDatabaseOptions)))
            .AddOptionsWithValidateOnStart<StateMachineDatabaseOptions>()
            .ValidateDataAnnotations();

        services.AddSingleton<StateMachineDatabaseOptions>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<StateMachineDatabaseOptions>>();
            return options.Value;
        });
    }
}