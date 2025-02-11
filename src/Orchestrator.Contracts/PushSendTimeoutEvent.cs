using System.Text.Json.Serialization;

namespace Orchestrator.Contracts;

public sealed record PushSendTimeoutEvent
{
    /// <summary>
    /// Globally unique id of communication.
    /// </summary>
    [JsonPropertyName("communication_id")]
    public required long CommunicationId { get; init; }

}