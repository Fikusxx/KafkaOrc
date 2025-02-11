using System.Text.Json.Serialization;

namespace Orchestrator.Contracts;

public sealed record CascadingCommunicationCompletedEvent
{
    /// <summary>
    /// Globally unique id of communication.
    /// </summary>
    [JsonPropertyName("communication_id")]
    public required long CommunicationId { get; init; }
    
    /// <summary>
    /// If Cascading Communication completed successfully.
    /// </summary>
    [JsonPropertyName("success_status")]
    public required bool Success { get; init; }
    
    /// <summary>
    /// Which channel Cascading Communication was delivered with, if any.
    /// </summary>
    [JsonPropertyName("delivery_channel")]
    public required string DeliveryChannel { get; init; }
    
    /// <summary>
    /// UTC ISO8601 time at which business process is completed.
    /// </summary>
    [JsonPropertyName("completed_at")]
    public required string CompletedAt { get; init; }
}