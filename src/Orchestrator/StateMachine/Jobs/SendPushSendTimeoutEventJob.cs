using MassTransit;
using Microsoft.Extensions.Logging;
using Orchestrator.Contracts;
using Quartz;

namespace Orchestrator.StateMachine.Jobs;

internal sealed class SendPushSendTimeoutEventJob : IJob
{
    private readonly ITopicProducer<long, PushSendTimeoutEvent> _producer;
    private readonly ILogger<SendPushSendTimeoutEventJob> _logger;
    public long Id { get; init; }
    public static string IdParameterName => nameof(Id); 
    public static readonly JobKey JobKey = new(nameof(SendPushSendTimeoutEventJob));

    public SendPushSendTimeoutEventJob(ITopicProducer<long, PushSendTimeoutEvent> producer,
        ILogger<SendPushSendTimeoutEventJob> logger)
    {
        _producer = producer;
        _logger = logger;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            _logger.LogInformation("Sending {EventName} for {CommunicationId}.",
                nameof(PushSendTimeoutEvent), Id);
            await _producer.Produce(Id, new PushSendTimeoutEvent { CommunicationId = Id });
        }
        catch (Exception e)
        {
            // possibly reschedule itself context.Scheduler.ScheduleJob()
            _logger.LogError(e, "Error executing {JobName}.", nameof(SendPushSendTimeoutEventJob));
        }
    }
}