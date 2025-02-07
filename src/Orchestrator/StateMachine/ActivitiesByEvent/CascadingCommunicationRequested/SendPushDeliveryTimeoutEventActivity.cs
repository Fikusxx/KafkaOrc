using MassTransit;
using Microsoft.Extensions.Logging;
using Orchestrator.Contracts;
using Orchestrator.StateMachine.Core;
using Orchestrator.StateMachine.Scheduler.Jobs;
using Quartz;

namespace Orchestrator.StateMachine.ActivitiesByEvent.CascadingCommunicationRequested;

internal sealed class SendPushDeliveryTimeoutEventActivity
    : IStateMachineActivity<CascadingCommunicationState, CascadingCommunicationRequestedEvent>
{
    private readonly ILogger<SendPushDeliveryTimeoutEventActivity> _logger;
    private readonly ISchedulerFactory _factory;

    public SendPushDeliveryTimeoutEventActivity(ILogger<SendPushDeliveryTimeoutEventActivity> logger,
        ISchedulerFactory factory)
    {
        _logger = logger;
        _factory = factory;
    }

    public void Probe(ProbeContext context) => context.CreateScope(nameof(SendPushDeliveryTimeoutEventActivity));
    public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);

    public async Task Execute(BehaviorContext<CascadingCommunicationState, CascadingCommunicationRequestedEvent> context,
        IBehavior<CascadingCommunicationState, CascadingCommunicationRequestedEvent> next)
    {
        _logger.LogInformation("Scheduling {EventName} for {CommunicationId}.",
            nameof(PushDeliveryTimeoutEvent), context.Saga.CommunicationId);

        var dataMap = new JobDataMap
        {
            { SendPushDeliveryTimeoutEventJob.IdParameterName, context.Saga.CommunicationId }
        };

        var trigger = TriggerBuilder.Create()
            .ForJob(SendPushDeliveryTimeoutEventJob.JobKey)
            .WithIdentity($"push.delivery.timeout.{context.Saga.CommunicationId}")
            .UsingJobData(dataMap)
            .StartAt(DateBuilder.FutureDate(context.Saga.PushDeliveryTimeoutSeconds, IntervalUnit.Second))
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