using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orchestrator.StateMachine;
using Orchestrator.StateMachine.Database;
using Orchestrator.StateMachine.Scheduler;

namespace Orchestrator;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO do stuff below
        // add health checks
        // add logging and ensure logging is correct with MT's LogContext
        
        // Migrations should be first to run.
        services.AddMigrations(configuration);
        services.AddScheduler(configuration);
        services.AddOrchestration(configuration);

        return services;
    }
}