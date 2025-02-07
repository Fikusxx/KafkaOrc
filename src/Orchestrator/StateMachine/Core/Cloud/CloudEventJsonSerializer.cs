using System.Text;
using System.Text.Json;
using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using Confluent.Kafka;

namespace Orchestrator.StateMachine.Core.Cloud;

internal sealed class CloudEventJsonSerializer : IAsyncSerializer<CloudEvent>
{
    private static readonly JsonEventFormatter Formatter = new();
    
    public Task<byte[]> SerializeAsync(CloudEvent data, SerializationContext context)
    {
        var element = Formatter.ConvertToJsonElement(data);
        var serializedData = JsonSerializer.Serialize(element);
        return Task.FromResult(Encoding.UTF8.GetBytes(serializedData));
    }
}