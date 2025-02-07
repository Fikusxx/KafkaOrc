using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace Orchestrator.StateMachine.Database;

internal sealed class AppDbContext : SagaDbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new CascadingCommunicationStateMap(); }
    }
}