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
        // TODO
        // add health checks
        // add logging
        
        services.AddMigrations(configuration);
        services.AddScheduler(configuration);
        services.AddOrchestration(configuration);

        return services;
    }
}