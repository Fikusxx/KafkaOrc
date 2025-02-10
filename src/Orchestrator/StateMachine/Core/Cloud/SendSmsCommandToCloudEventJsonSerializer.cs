using System.Net.Mime;
using System.Text;
using System.Text.Json;
using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using Confluent.Kafka;
using Orchestrator.Contracts;

namespace Orchestrator.StateMachine.Core.Cloud;

internal sealed class SendSmsCommandToCloudEventJsonSerializer : IAsyncSerializer<SendSmsCommand>
{
    private static readonly JsonEventFormatter Formatter = new();

    public Task<byte[]> SerializeAsync(SendSmsCommand data, SerializationContext context)
    {
        var @event = new CloudEvent
        {
            Id = Guid.NewGuid().ToString(),
            Type = "sms",
            Source = new Uri("https://cloudevents.io/"),
            Time = DateTimeOffset.UtcNow,
            DataContentType = MediaTypeNames.Application.Json,
            Data = data
        };
        var element = Formatter.ConvertToJsonElement(@event);
        var serializedData = JsonSerializer.Serialize(element);

        return Task.FromResult(Encoding.UTF8.GetBytes(serializedData));
    }
}