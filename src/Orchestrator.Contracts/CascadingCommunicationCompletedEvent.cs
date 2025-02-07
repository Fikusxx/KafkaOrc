using System.Text.Json.Serialization;

namespace Orchestrator.Contracts;

public sealed record CascadingCommunicationCompletedEvent
{
    /// <summary>
    /// Globally unique id of communication
    /// </summary>
    [JsonPropertyName("communication_id")]
    public required long CommunicationId { get; init; }
    
    /// <summary>
    /// If Cascading Communication completed successfully
    /// TODO add json prop name
    /// </summary>
    [JsonPropertyName("???")]
    public required bool Success { get; init; }
    
    /// <summary>
    /// Which channel Cascading Communication was delivered with, if any
    /// TODO add json prop name
    /// </summary>
    [JsonPropertyName("???")]
    public required int DeliveryChannel { get; init; }
}