using MassTransit;
using Microsoft.Extensions.Logging;
using Orchestrator.Contracts;
using Push.Contracts;
using Sms.Contracts;

namespace Orchestrator.StateMachine.ActivitiesByEvent.PushSendTimeout;

internal sealed class SendSmsCommandActivity
    : IStateMachineActivity<CascadingCommunicationState, PushSendTimeoutEvent>
{
    private readonly ILogger<SendSmsCommandActivity> _logger;
    private readonly ITopicProducer<long, PushSendEvent> _producer;

    public SendSmsCommandActivity(ILogger<SendSmsCommandActivity> logger,
        ITopicProducer<long, PushSendEvent> producer)
    {
        _logger = logger;
        _producer = producer;
    }

    public void Probe(ProbeContext context) => context.CreateScope(nameof(SendSmsCommandActivity));
    public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);

    public Task Execute(BehaviorContext<CascadingCommunicationState, PushSendTimeoutEvent> context,
        IBehavior<CascadingCommunicationState, PushSendTimeoutEvent> next)
    {
        _logger.LogInformation("Sending {CommandName} for {CommunicationId}.",
            nameof(SendPushCommand), context.Saga.CommunicationId);

        var command = new SendSmsCommand
        {
            SmsId = context.Saga.CommunicationId,
            To = context.Saga.SmsData.To,
            Text = context.Saga.SmsData.Text,
            Title = context.Saga.SmsData.Title,
            Priority = context.Saga.SmsData.Priority,
        };

        return _producer.Produce(command.SmsId, command);
    }

    public async Task Faulted<TException>(
        BehaviorExceptionContext<CascadingCommunicationState, PushSendTimeoutEvent, TException> context,
        IBehavior<CascadingCommunicationState, PushSendTimeoutEvent> next) where TException : Exception
    {
        await next.Faulted(context);
    }
}