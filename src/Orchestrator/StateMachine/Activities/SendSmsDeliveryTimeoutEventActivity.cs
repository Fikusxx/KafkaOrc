using MassTransit;
using Microsoft.Extensions.Logging;
using Orchestrator.Contracts;
using Orchestrator.StateMachine.Core;
using Orchestrator.StateMachine.Scheduler.Jobs;
using Quartz;

namespace Orchestrator.StateMachine.Activities;

internal sealed class SendSmsDeliveryTimeoutEventActivity
    : IStateMachineActivity<CascadingCommunicationState>
{
    private readonly ILogger<SendSmsDeliveryTimeoutEventActivity> _logger;
    private readonly ISchedulerFactory _factory;

    public SendSmsDeliveryTimeoutEventActivity(ILogger<SendSmsDeliveryTimeoutEventActivity> logger,
        ISchedulerFactory factory)
    {
        this._logger = logger;
        this._factory = factory;
    }
    
    public void Probe(ProbeContext context) => context.CreateScope(nameof(SendSmsDeliveryTimeoutEventActivity));
    public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);

    public async Task Execute(BehaviorContext<CascadingCommunicationState> context, IBehavior<CascadingCommunicationState> next)
    {
        await ScheduleJobAsync(context);
        await next.Execute(context);
    }

    public async Task Execute<T>(BehaviorContext<CascadingCommunicationState, T> context, IBehavior<CascadingCommunicationState, T> next) where T : class
    {
        await ScheduleJobAsync(context);
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

    private async Task ScheduleJobAsync(BehaviorContext<CascadingCommunicationState> context)
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
            // TODO restore when testing is done
            // .StartAt(DateBuilder.FutureDate(context.Saga.SmsDeliveryTimeoutDays, IntervalUnit.Day))
            .StartAt(DateBuilder.FutureDate(1, IntervalUnit.Minute))
            .Build();

        var scheduler = await _factory.GetScheduler();
        await scheduler.ScheduleJob(trigger);
    }
}