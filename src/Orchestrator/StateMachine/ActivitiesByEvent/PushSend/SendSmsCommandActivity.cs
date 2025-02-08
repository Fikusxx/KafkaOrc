using MassTransit;
using Microsoft.Extensions.Logging;
using Orchestrator.Contracts;
using Orchestrator.StateMachine.Core;
using Push.Contracts;

namespace Orchestrator.StateMachine.ActivitiesByEvent.PushSend;

internal sealed class SendSmsCommandActivity
    : IStateMachineActivity<CascadingCommunicationState, PushSendEvent>
{
    private readonly ILogger<SendSmsCommandActivity> _logger;
    private readonly ITopicProducer<long, SendSmsCommand> _producer;

    public SendSmsCommandActivity(ILogger<SendSmsCommandActivity> logger,
        ITopicProducer<long, SendSmsCommand> producer)
    {
        this._logger = logger;
        this._producer = producer;
    }

    public void Probe(ProbeContext context) => context.CreateScope(nameof(SendSmsCommandActivity));
    public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);

    public async Task Execute(BehaviorContext<CascadingCommunicationState, PushSendEvent> context,
        IBehavior<CascadingCommunicationState, PushSendEvent> next)
    {
        _logger.LogInformation("Sending {CommandName} for {CommunicationId}.",
            nameof(SendSmsCommand), context.Saga.CommunicationId);

        var command = new SendSmsCommand
        {
            SmsId = context.Saga.CommunicationId,
            To = context.Saga.SmsData.To,
            Text = context.Saga.SmsData.Text,
            Title = context.Saga.SmsData.Title,
            Priority = context.Saga.SmsData.Priority,
        };

        await _producer.Produce(command.SmsId, command);
        await next.Execute(context);
    }

    public async Task Faulted<TException>(
        BehaviorExceptionContext<CascadingCommunicationState, PushSendEvent, TException> context,
        IBehavior<CascadingCommunicationState, PushSendEvent> next) where TException : Exception
    {
        await next.Faulted(context);
    }
}