using System.Net.Mime;
using System.Text;
using System.Text.Json;
using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using Confluent.Kafka;
using Orchestrator.Contracts;
using Push.Contracts;
using Sms.Contracts;

namespace Orchestrator.StateMachine;

// TODO delete when testing is done
internal sealed class CascadingCommunicationRequestedEvent2 : IAsyncSerializer<CascadingCommunicationRequestedEvent>
{
    private static readonly JsonEventFormatter Formatter = new();
    
    public Task<byte[]> SerializeAsync(CascadingCommunicationRequestedEvent data, SerializationContext context)
    {
        var cloudEvent = new CloudEvent
        {
            Id = Guid.NewGuid().ToString(),
            Type = "type",
            Source = new Uri("https://cloudevents.io/"),
            Time = DateTimeOffset.Now,
            DataContentType = MediaTypeNames.Application.Json,
            Data = data
        };
        var element = Formatter.ConvertToJsonElement(cloudEvent);
        var serializedData = JsonSerializer.Serialize(element);
        return Task.FromResult(Encoding.UTF8.GetBytes(serializedData));
    }
}

internal sealed class PushSendEvent2 : IAsyncSerializer<PushSendEvent>
{
    private static readonly JsonEventFormatter Formatter = new();
    
    public Task<byte[]> SerializeAsync(PushSendEvent data, SerializationContext context)
    {
        var cloudEvent = new CloudEvent
        {
            Id = Guid.NewGuid().ToString(),
            Type = "type",
            Source = new Uri("https://cloudevents.io/"),
            Time = DateTimeOffset.Now,
            DataContentType = MediaTypeNames.Application.Json,
            Data = data
        };
        var element = Formatter.ConvertToJsonElement(cloudEvent);
        var serializedData = JsonSerializer.Serialize(element);
        return Task.FromResult(Encoding.UTF8.GetBytes(serializedData));
    }
}

internal sealed class PushDeliveryEvent2 : IAsyncSerializer<PushDeliveryEvent>
{
    private static readonly JsonEventFormatter Formatter = new();
    
    public Task<byte[]> SerializeAsync(PushDeliveryEvent data, SerializationContext context)
    {
        var cloudEvent = new CloudEvent
        {
            Id = Guid.NewGuid().ToString(),
            Type = "type",
            Source = new Uri("https://cloudevents.io/"),
            Time = DateTimeOffset.Now,
            DataContentType = MediaTypeNames.Application.Json,
            Data = data
        };
        var element = Formatter.ConvertToJsonElement(cloudEvent);
        var serializedData = JsonSerializer.Serialize(element);
        return Task.FromResult(Encoding.UTF8.GetBytes(serializedData));
    }
}

internal sealed class SmsSendEvent2 : IAsyncSerializer<SmsSendEvent>
{
    private static readonly JsonEventFormatter Formatter = new();
    
    public Task<byte[]> SerializeAsync(SmsSendEvent data, SerializationContext context)
    {
        var cloudEvent = new CloudEvent
        {
            Id = Guid.NewGuid().ToString(),
            Type = "type",
            Source = new Uri("https://cloudevents.io/"),
            Time = DateTimeOffset.Now,
            DataContentType = MediaTypeNames.Application.Json,
            Data = data
        };
        var element = Formatter.ConvertToJsonElement(cloudEvent);
        var serializedData = JsonSerializer.Serialize(element);
        return Task.FromResult(Encoding.UTF8.GetBytes(serializedData));
    }
}

internal sealed class SmsDeliveryEvent2 : IAsyncSerializer<SmsDeliveryEvent>
{
    private static readonly JsonEventFormatter Formatter = new();
    
    public Task<byte[]> SerializeAsync(SmsDeliveryEvent data, SerializationContext context)
    {
        var cloudEvent = new CloudEvent
        {
            Id = Guid.NewGuid().ToString(),
            Type = "type",
            Source = new Uri("https://cloudevents.io/"),
            Time = DateTimeOffset.Now,
            DataContentType = MediaTypeNames.Application.Json,
            Data = data
        };
        var element = Formatter.ConvertToJsonElement(cloudEvent);
        var serializedData = JsonSerializer.Serialize(element);
        return Task.FromResult(Encoding.UTF8.GetBytes(serializedData));
    }
}