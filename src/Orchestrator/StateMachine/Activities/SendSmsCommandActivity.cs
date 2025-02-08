using MassTransit;
using Microsoft.Extensions.Logging;
using Orchestrator.Contracts;
using Orchestrator.StateMachine.Core;

namespace Orchestrator.StateMachine.Activities;

internal sealed class SendSmsCommandActivity
    : IStateMachineActivity<CascadingCommunicationState>
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

    public async Task Execute(BehaviorContext<CascadingCommunicationState> context, IBehavior<CascadingCommunicationState> next)
    {
        await SendEventAsync(context);
        await next.Execute(context);
    }

    public async Task Execute<T>(BehaviorContext<CascadingCommunicationState, T> context, IBehavior<CascadingCommunicationState, T> next) where T : class
    {
        await SendEventAsync(context);
        await next.Execute(context);
    }

    public async Task Faulted<TException>(BehaviorExceptionContext<CascadingCommunicationState, TException> context, IBehavior<CascadingCommunicationState> next) where TException : Exception
    {
        await next.Faulted(context);
    }

    public async Task Faulted<T, TException>(BehaviorExceptionContext<CascadingCommunicationState, T, TException> context, IBehavior<CascadingCommunicationState, T> next) where T : class where TException : Exception
    {
        await next.Faulted(context);
    }
    
    private Task SendEventAsync(BehaviorContext<CascadingCommunicationState> context)
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

        return _producer.Produce(command.SmsId, command);
    }
}