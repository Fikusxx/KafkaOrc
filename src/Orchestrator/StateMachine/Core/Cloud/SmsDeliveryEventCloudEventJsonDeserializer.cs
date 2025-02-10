using Confluent.Kafka;
using Sms.Contracts;

namespace Orchestrator.StateMachine.Core.Cloud;

internal sealed class SmsDeliveryEventCloudEventJsonDeserializer
    : BaseCloudEventJsonDeserializer<SmsDeliveryEvent>,
        IDeserializer<SmsDeliveryEvent>
{
    public SmsDeliveryEvent Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        var result = BaseDeserialize(data);

        return result as SmsDeliveryEvent ??
               throw new InvalidOperationException(
                   $"CloudEvent data is not {nameof(SmsDeliveryEvent)}");
    }
}