using MassTransit;
using Microsoft.Extensions.Logging;
using Orchestrator.Contracts;

namespace Orchestrator.StateMachine.ActivitiesByEvent.Final;

internal sealed class SendCascadingCommunicationCompletedEventActivity
    : IStateMachineActivity<CascadingCommunicationState>
{
    private readonly ILogger<SendCascadingCommunicationCompletedEventActivity> _logger;
    private readonly ITopicProducer<long, CascadingCommunicationCompletedEvent> _producer;

    public SendCascadingCommunicationCompletedEventActivity(ILogger<SendCascadingCommunicationCompletedEventActivity> logger,
        ITopicProducer<long, CascadingCommunicationCompletedEvent> producer)
    {
        this._logger = logger;
        this._producer = producer;
    }
    
    public void Probe(ProbeContext context) =>
        context.CreateScope(nameof(SendCascadingCommunicationCompletedEventActivity));

    public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);

    public Task Execute(BehaviorContext<CascadingCommunicationState> context,
        IBehavior<CascadingCommunicationState> next)
    {
        return SendEventAsync(context);
    }

    public Task Execute<T>(BehaviorContext<CascadingCommunicationState, T> context,
        IBehavior<CascadingCommunicationState, T> next) where T : class
    {
        return SendEventAsync(context);
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
            DeliveryChannel = (int)context.Saga.DeliveryChannel
        };

        return _producer.Produce(command.CommunicationId, command);
    }
}