using MassTransit;
using Microsoft.Extensions.Logging;
using Orchestrator.Contracts;
using Orchestrator.StateMachine.Core;

namespace Orchestrator.StateMachine.ActivitiesByEvent.CascadingCommunicationRequested;

internal sealed class SendPushCommandActivity
    : IStateMachineActivity<CascadingCommunicationState, CascadingCommunicationRequestedEvent>
{
    private readonly ILogger<SendPushCommandActivity> _logger;
    private readonly ITopicProducer<long, SendPushCommand> _producer;

    public SendPushCommandActivity(ILogger<SendPushCommandActivity> logger,
        ITopicProducer<long, SendPushCommand> producer)
    {
        this._logger = logger;
        this._producer = producer;
    }

    public void Probe(ProbeContext context) => context.CreateScope(nameof(SendPushCommandActivity));

    public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);

    public async Task Execute(BehaviorContext<CascadingCommunicationState, CascadingCommunicationRequestedEvent> context,
        IBehavior<CascadingCommunicationState, CascadingCommunicationRequestedEvent> next)
    {
        _logger.LogInformation("Sending {CommandName} for {CommunicationId}.",
            nameof(SendPushCommand), context.Saga.CommunicationId);

        var command = new SendPushCommand
        {
            PushId = context.Message.CommunicationId,
            Type = context.Message.PushData.Type,
            ClientCode = context.Message.PushData.ClientCode,
            Title = context.Message.PushData.Title,
            Body = context.Message.PushData.Body,
            SecondsToLive = context.Message.PushData.SecondsToLive,
            MarketingInfoId = context.Message.PushData.MarketingInfoId,
            TransactionLink = context.Message.PushData.TransactionLink,
        };

        await _producer.Produce(command.PushId, command);
        
        await next.Execute(context);
    }

    public async Task Faulted<TException>(
        BehaviorExceptionContext<CascadingCommunicationState, CascadingCommunicationRequestedEvent, TException> context,
        IBehavior<CascadingCommunicationState, CascadingCommunicationRequestedEvent> next) where TException : Exception
    {
        await next.Faulted(context);
    }
}