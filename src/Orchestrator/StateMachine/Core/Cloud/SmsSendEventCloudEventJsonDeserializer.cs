using Confluent.Kafka;
using Sms.Contracts;

namespace Orchestrator.StateMachine.Core.Cloud;

internal sealed class SmsSendEventCloudEventJsonDeserializer
    : BaseCloudEventJsonDeserializer<SmsSendEvent>,
        IDeserializer<SmsSendEvent>
{
    public SmsSendEvent Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        var result = BaseDeserialize(data);

        return result as SmsSendEvent ??
               throw new InvalidOperationException(
                   $"CloudEvent data is not {nameof(SmsSendEvent)}");
    }
}