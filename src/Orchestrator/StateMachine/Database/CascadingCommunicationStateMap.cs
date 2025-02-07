using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Orchestrator.StateMachine.Database;

internal sealed class CascadingCommunicationStateMap : SagaClassMap<CascadingCommunicationState>
{
    protected override void Configure(EntityTypeBuilder<CascadingCommunicationState> entity, ModelBuilder model)
    {
        entity.HasIndex(x => x.CommunicationId);
        
        entity.Property(x => x.CurrentState)
            .HasMaxLength(64);

        entity.ComplexProperty(x => x.SmsData);
        
        entity.Property(x => x.RowVersion)
            .IsRowVersion();
    }
}