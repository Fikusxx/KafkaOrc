using System.Net.Mime;
using CloudNative.CloudEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using Orchestrator.Contracts;
using Orchestrator.StateMachine.Core;

namespace Orchestrator.StateMachine.ActivitiesByEvent.Final;

internal sealed class SendCascadingCommunicationCompletedEventActivity
    : IStateMachineActivity<CascadingCommunicationState>
{
    private readonly ILogger<SendCascadingCommunicationCompletedEventActivity> _logger;
    private readonly ITopicProducer<long, CloudEvent> _producer;

    public SendCascadingCommunicationCompletedEventActivity(ILogger<SendCascadingCommunicationCompletedEventActivity> logger,
        ITopicProducer<long, CloudEvent> producer)
    {
        this._logger = logger;
        this._producer = producer;
    }
    
    public void Probe(ProbeContext context) =>
        context.CreateScope(nameof(SendCascadingCommunicationCompletedEventActivity));

    public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);

    public async Task Execute(BehaviorContext<CascadingCommunicationState> context,
        IBehavior<CascadingCommunicationState> next)
    {
        await SendEventAsync(context);
        await next.Execute(context);
    }

    public async Task Execute<T>(BehaviorContext<CascadingCommunicationState, T> context,
        IBehavior<CascadingCommunicationState, T> next) where T : class
    {
        await SendEventAsync(context);
        await next.Execute(context);
    }

    public async Task Faulted<TException>(BehaviorExceptionContext<CascadingCommunicationState, TException> context,
        IBehavior<CascadingCommunicationState> next) where TException : Exception
    {
        await next.Faulted(context);
    }

    public async Task Faulted<T, TException>(
        BehaviorExceptionContext<CascadingCommunicationState, T, TException> context,
        IBehavior<CascadingCommunicationState, T> next) where T : class where TException : Exception
    {
        await next.Faulted(context);
    }

    private Task SendEventAsync(BehaviorContext<CascadingCommunicationState> context)
    {
        _logger.LogInformation("Sending {EventName} for {CommunicationId}.",
            nameof(CascadingCommunicationCompletedEvent), context.Saga.CommunicationId);
        
        var command = new CascadingCommunicationCompletedEvent
        {
            CommunicationId = context.Saga.CommunicationId,
            Success = context.Saga.Success,
            DeliveryChannel = GetDeliveryChannelAsString(context.Saga.DeliveryChannel),
            CompletedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
        };
        
        var cloudEvent = new CloudEvent
        {
            Id = Guid.NewGuid().ToString(),
            Type = "kaskad",
            Source = new Uri("https://cloudevents.io/"),
            Time = DateTimeOffset.UtcNow,
            DataContentType = MediaTypeNames.Application.Json,
            Data = command
        };

        return _producer.Produce(command.CommunicationId, cloudEvent);
    }

    private static string GetDeliveryChannelAsString(DeliveryChannel channel) =>
        channel switch
        {
            DeliveryChannel.Push => "push",
            DeliveryChannel.Sms  => "sms",
            DeliveryChannel.None => "not_delivered",
            _ => "not_delivered",
        };
}