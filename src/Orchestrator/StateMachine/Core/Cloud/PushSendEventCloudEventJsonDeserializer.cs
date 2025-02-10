using Confluent.Kafka;
using Push.Contracts;

namespace Orchestrator.StateMachine.Core.Cloud;

internal sealed class PushSendEventCloudEventJsonDeserializer
    : BaseCloudEventJsonDeserializer<PushSendEvent>,
        IDeserializer<PushSendEvent>
{
    public PushSendEvent Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        var result = BaseDeserialize(data);

        return result as PushSendEvent ??
               throw new InvalidOperationException(
                   $"CloudEvent data is not {nameof(PushSendEvent)}");
    }
}