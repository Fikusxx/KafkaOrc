using MassTransit;
using Microsoft.Extensions.Logging;
using Orchestrator.Contracts;
using Quartz;

namespace Orchestrator.StateMachine.Scheduler.Jobs;

internal sealed class SendSmsDeliveryTimeoutEventJob : IJob
{
    private readonly ITopicProducer<long, SmsDeliveryTimeoutEvent> _producer;
    private readonly ILogger<SendSmsDeliveryTimeoutEventJob> _logger;
    public long Id { get; init; }
    public static string IdParameterName => nameof(Id); 
    public static readonly JobKey JobKey = new(nameof(SendSmsDeliveryTimeoutEventJob));

    public SendSmsDeliveryTimeoutEventJob(ITopicProducer<long, SmsDeliveryTimeoutEvent> producer,
        ILogger<SendSmsDeliveryTimeoutEventJob> logger)
    {
        this._producer = producer;
        this._logger = logger;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            _logger.LogInformation("Sending {EventName} for {CommunicationId}.",
                nameof(SmsDeliveryTimeoutEvent), Id);
            await _producer.Produce(Id, new SmsDeliveryTimeoutEvent { CommunicationId = Id });
        }
        catch (Exception e)
        {
            // possibly reschedule itself context.Scheduler.ScheduleJob()
            _logger.LogError(e, "Error executing {JobName}.", nameof(SendSmsDeliveryTimeoutEventJob));
        }
    }
}