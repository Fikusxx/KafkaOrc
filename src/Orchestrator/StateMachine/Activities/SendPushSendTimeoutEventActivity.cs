using MassTransit;
using Microsoft.Extensions.Logging;
using Orchestrator.Contracts;
using Orchestrator.StateMachine.Core;
using Orchestrator.StateMachine.Scheduler.Jobs;
using Quartz;

namespace Orchestrator.StateMachine.Activities;

/// <summary>
/// Activity to be executed upon entering sms branch state machine state.
/// </summary>
internal sealed class SendPushSendTimeoutEventActivity
    : IStateMachineActivity<CascadingCommunicationState, CascadingCommunicationRequestedEvent>
{
    private readonly ILogger<SendPushSendTimeoutEventActivity> _logger;
    private readonly ISchedulerFactory _factory;

    public SendPushSendTimeoutEventActivity(ILogger<SendPushSendTimeoutEventActivity> logger, ISchedulerFactory factory)
    {
        this._logger = logger;
        this._factory = factory;
    }

    public void Probe(ProbeContext context) => context.CreateScope(nameof(SendPushSendTimeoutEventActivity));
    public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);

    public async Task Execute(
        BehaviorContext<CascadingCommunicationState, CascadingCommunicationRequestedEvent> context,
        IBehavior<CascadingCommunicationState, CascadingCommunicationRequestedEvent> next)
    {
        _logger.LogInformation("Scheduling {EventName} for {CommunicationId}.",
            nameof(PushSendTimeoutEvent), context.Saga.CommunicationId);

        var dataMap = new JobDataMap
        {
            { SendPushSendTimeoutEventJob.IdParameterName, context.Saga.CommunicationId }
        };

        var trigger = TriggerBuilder.Create()
            .ForJob(SendPushSendTimeoutEventJob.JobKey)
            .WithIdentity($"push.send.timeout.{context.Saga.CommunicationId}")
            .UsingJobData(dataMap)
            .StartAt(DateBuilder.FutureDate(context.Saga.PushSendTimeoutSeconds, IntervalUnit.Second))
            .Build();

        var scheduler = await _factory.GetScheduler();
        await scheduler.ScheduleJob(trigger);

        await next.Execute(context);
    }

    public async Task Faulted<TException>(
        BehaviorExceptionContext<CascadingCommunicationState, CascadingCommunicationRequestedEvent, TException> context,
        IBehavior<CascadingCommunicationState, CascadingCommunicationRequestedEvent> next) where TException : Exception
    {
        await next.Faulted(context);
    }
}