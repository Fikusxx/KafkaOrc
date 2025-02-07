using MassTransit;
using Microsoft.Extensions.Logging;
using Orchestrator.Contracts;
using Quartz;

namespace Orchestrator.StateMachine.Jobs;

internal sealed class SendPushDeliveryTimeoutEventJob : IJob
{
    private readonly ITopicProducer<long, PushDeliveryTimeoutEvent> _producer;
    private readonly ILogger<SendPushDeliveryTimeoutEventJob> _logger;
    public long Id { get; init; }
    public static string IdParameterName => nameof(Id); 
    public static readonly JobKey JobKey = new(nameof(SendPushDeliveryTimeoutEventJob));

    public SendPushDeliveryTimeoutEventJob(ITopicProducer<long, PushDeliveryTimeoutEvent> producer,
        ILogger<SendPushDeliveryTimeoutEventJob> logger)
    {
        _producer = producer;
        _logger = logger;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            _logger.LogInformation("Sending {EventName} for {CommunicationId}.",
                nameof(PushDeliveryTimeoutEvent), Id);
            await _producer.Produce(Id, new PushDeliveryTimeoutEvent { CommunicationId = Id });
        }
        catch (Exception e)
        {
            // possibly reschedule itself context.Scheduler.ScheduleJob()
            _logger.LogError(e, "Error executing {JobName}.", nameof(SendPushDeliveryTimeoutEventJob));
        }
    }
}