using MassTransit;
using MassTransit.RetryPolicies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Orchestrator.StateMachine.Database;

internal sealed class MigrationHostedService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MigrationHostedService> _logger;
    private readonly string _schema;

    public MigrationHostedService(IServiceScopeFactory scopeFactory, ILogger<MigrationHostedService> logger, StateMachineDatabaseOptions options)
    {
        this._scopeFactory = scopeFactory;
        this._logger = logger;
        this._schema = options.Schema;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return ExecuteMigrationsAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task ExecuteMigrationsAsync(CancellationToken cancellationToken)
    {
        await Retry.Exponential(10, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15))
            .Retry(async () =>
            {
                try
                {
                    await using var scope = _scopeFactory.CreateAsyncScope();
                    var context = scope.ServiceProvider.GetRequiredService<DbContext>();
                    var command = $"CREATE SCHEMA IF NOT EXISTS {_schema}";
                    await context.Database.ExecuteSqlRawAsync(command, cancellationToken);
                    await context.Database.MigrateAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while migrating the database.");
                    throw;
                }
            }, cancellationToken);
    }
}