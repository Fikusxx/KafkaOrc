using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Orchestrator;
using Orchestrator.Contracts;
using Push.Contracts;
using Sms.Contracts;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddYamlFile($"appsettings.{builder.Environment.EnvironmentName}.yaml", optional: true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration, builder.Host);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/healthz/ready", new HealthCheckOptions { AllowCachingResponses = true });
app.MapGet("/healthz/live", () => Results.Ok());

app.MapGet("communication.requested", async (
    [FromQuery] long id,
    [FromServices] ITopicProducer<long, CascadingCommunicationRequestedEvent> producer) =>
{
    // var cloudEventProducer = provider.GetProducer<long, CloudEvent>(new Uri("topic:start"));
    var @event = new CascadingCommunicationRequestedEvent
    {
        CommunicationId = id,
        PushDeliveryTimeoutSeconds = 30,
        PushData = new PushData
        {
            Body = "Push data",
            Title = "Push title",
            Type = "pohui",
            ClientCode = "123",
            SecondsToLive = 60
        },
        SmsData = new SmsData
        {
            Priority = 1,
            Text = "Sms data",
            Title = "Sms title",
            To = 79092970565
        }
    };

    await producer.Produce(@event.CommunicationId, @event);
    
    return Results.Ok();
});

app.MapGet("push.send", async (
    [FromQuery] long id,
    [FromQuery] int deliveryStatus,
    [FromServices] ITopicProducer<long, PushSendEvent> producer) =>
{
    var @event = new PushSendEvent
    {
        PushId = id,
        ExternalId = "123",
        DeliveryStatus = deliveryStatus
    };

    await producer.Produce(@event.PushId, @event);
    
    return Results.Ok();
});

app.MapGet("push.delivery", async (
    [FromQuery] long id,
    [FromQuery] int deliveryStatus,
    [FromServices] ITopicProducer<long, PushDeliveryEvent> producer) =>
{
    var @event = new PushDeliveryEvent
    {
        PushId = id,
        CompletedAt = "pohui",
        DeliveryStatus = deliveryStatus
    };

    await producer.Produce(@event.PushId, @event);
    
    return Results.Ok();
});

app.MapGet("sms.send", async (
    [FromQuery] long id,
    [FromQuery] int deliveryStatus,
    [FromServices] ITopicProducer<long, SmsSendEvent> producer) =>
{
    var @event = new SmsSendEvent
    {
        SmsId = id,
        ExternalId = "123",
        DeliveryStatus = deliveryStatus
    };

    await producer.Produce(@event.SmsId, @event);
    
    return Results.Ok();
});

app.MapGet("sms.delivery", async (
    [FromQuery] long id,
    [FromQuery] int deliveryStatus,
    [FromServices] ITopicProducer<long, SmsDeliveryEvent> producer) =>
{
    var @event = new SmsDeliveryEvent
    {
        SmsId = id,
        CompletedAt = "pohui",
        DeliveryStatus = deliveryStatus
    };

    await producer.Produce(@event.SmsId, @event);
    
    return Results.Ok();
});

await app.RunAsync();