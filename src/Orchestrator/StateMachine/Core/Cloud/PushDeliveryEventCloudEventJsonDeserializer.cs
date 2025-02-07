using Confluent.Kafka;
using Push.Contracts;

namespace Orchestrator.StateMachine.Core.Cloud;

internal sealed class PushDeliveryEventCloudEventJsonDeserializer
    : BaseCloudEventJsonSerializer<PushDeliveryEvent>,
        IDeserializer<PushDeliveryEvent>
{
    public PushDeliveryEvent Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        var result = BaseDeserialize(data);

        return result as PushDeliveryEvent ??
               throw new InvalidOperationException(
                   $"CloudEvent data is not {nameof(PushDeliveryEvent)}");
    }
}