using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orchestrator.Common;
using Orchestrator.StateMachine;
using Orchestrator.StateMachine.Database;
using Orchestrator.StateMachine.Scheduler;

namespace Orchestrator;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostBuilder builder)
    {
        // Migrations should be first to run.
        services.AddMigrations(configuration);
        services.AddScheduler(configuration);
        services.AddOrchestration(configuration);
        
        services.AddCommonServices(configuration, builder);

        return services;
    }
}