using MassTransit;

namespace Orchestrator.StateMachine.Core;

/// <summary>
/// CascadingCommunicationStateMachine state.
/// </summary>
internal sealed class CascadingCommunicationState : SagaStateMachineInstance
{
    /// <summary>
    /// PK of state machine.
    /// </summary>
    public Guid CorrelationId { get; set; }
    
    /// <summary>
    /// Current state of state machine, aka status.
    /// </summary>
    public required string CurrentState { get; set; }
    
    /// <summary>
    /// Globally unique id of business process, correlates state machine.
    /// </summary>
    public required long CommunicationId { get; set; }
    
    /// <summary>
    /// Sms data required for sending sms.
    /// </summary>
    public required SmsData SmsData { get; set; }

    /// <summary>
    /// Seconds to wait for push to be sent to a client.
    /// Hardcoded value, do not change.
    /// </summary>
    public required int PushSendTimeoutSeconds { get; set; } = 5;
    
    /// <summary>
    /// Seconds to wait for push to be delivered to a client.
    /// Contained within first event that starts state machine, updated on creation.
    /// </summary>
    public required int PushDeliveryTimeoutSeconds { get; set; }
    
    /// <summary>
    /// Days to wait for sms to be delivered to a client.
    /// Hardcoded value, do not change.
    /// </summary>
    public required int SmsDeliveryTimeoutDays { get; set; } = 2;
    
    /// <summary>
    /// Represents which delivery channel was state machine completed with.
    /// </summary>
    public required DeliveryChannel DeliveryChannel { get; set; }
    
    /// <summary>
    /// Represents if communication was successful.
    /// </summary>
    public required bool Success { get; set; }
    
    /// <summary>
    /// For optimistic pg concurrency.
    /// </summary>
    public required uint RowVersion { get; set; }
}

internal sealed class SmsData
{
    public required int Priority { get; init; }
    public required long To { get; init; }
    public required string Text { get; init; }
    public required string Title { get; init; }
}