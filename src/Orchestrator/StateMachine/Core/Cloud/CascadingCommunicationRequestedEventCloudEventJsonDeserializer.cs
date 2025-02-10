using Confluent.Kafka;
using Orchestrator.Contracts;

namespace Orchestrator.StateMachine.Core.Cloud;

internal sealed class CascadingCommunicationRequestedEventCloudEventJsonDeserializer
    : BaseCloudEventJsonDeserializer<CascadingCommunicationRequestedEvent>,
        IDeserializer<CascadingCommunicationRequestedEvent>
{
    public CascadingCommunicationRequestedEvent Deserialize(ReadOnlySpan<byte> data, bool isNull,
        SerializationContext context)
    {
        var result = BaseDeserialize(data);

        return result as CascadingCommunicationRequestedEvent ??
               throw new InvalidOperationException(
                   $"CloudEvent data is not {nameof(CascadingCommunicationRequestedEvent)}");
    }
}