using MassTransit;
using Microsoft.Extensions.Logging;
using Orchestrator.Contracts;
using Orchestrator.StateMachine.Jobs;
using Push.Contracts;
using Quartz;

namespace Orchestrator.StateMachine.ActivitiesByEvent.PushDelivery;

internal sealed class SendSmsDeliveryTimeoutEventActivity
    : IStateMachineActivity<CascadingCommunicationState, PushDeliveryEvent>
{
    private readonly ILogger<SendSmsDeliveryTimeoutEventActivity> _logger;
    private readonly ISchedulerFactory _factory;

    public SendSmsDeliveryTimeoutEventActivity(ILogger<SendSmsDeliveryTimeoutEventActivity> logger,
        ISchedulerFactory factory)
    {
        _logger = logger;
        _factory = factory;
    }
    
    public void Probe(ProbeContext context) => context.CreateScope(nameof(SendSmsDeliveryTimeoutEventActivity));
    public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);

    public async Task Execute(BehaviorContext<CascadingCommunicationState, PushDeliveryEvent> context,
        IBehavior<CascadingCommunicationState, PushDeliveryEvent> next)
    {
        _logger.LogInformation("Scheduling {EventName} for {CommunicationId}.",
            nameof(SmsDeliveryTimeoutEvent), context.Saga.CommunicationId);

        var dataMap = new JobDataMap
        {
            { SendSmsDeliveryTimeoutEventJob.IdParameterName, context.Saga.CommunicationId }
        };

        var trigger = TriggerBuilder.Create()
            .ForJob(SendSmsDeliveryTimeoutEventJob.JobKey)
            .WithIdentity($"sms.delivery.timeout.{context.Saga.CommunicationId}")
            .UsingJobData(dataMap)
            .StartAt(DateBuilder.FutureDate(context.Saga.SmsDeliveryTimeoutDays, IntervalUnit.Day))
            .Build();

        var scheduler = await _factory.GetScheduler();
        await scheduler.ScheduleJob(trigger);

        await next.Execute(context);
    }

    public async Task Faulted<TException>(
        BehaviorExceptionContext<CascadingCommunicationState, PushDeliveryEvent, TException> context,
        IBehavior<CascadingCommunicationState, PushDeliveryEvent> next) where TException : Exception
    {
        await next.Faulted(context);
    }
}