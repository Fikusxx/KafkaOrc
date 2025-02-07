using System.Text.Json.Serialization;

namespace Push.Contracts;

public sealed record PushDeliveryEvent
{
    [JsonPropertyName("push_id")] public required long PushId { get; init; }
    [JsonPropertyName("delivery_status")] public required int DeliveryStatus { get; init; }

    /// <summary>
    /// DateTime UTC in ISO 8601 format - 2024-12-12T04:30:00.000Z
    /// </summary>
    [JsonPropertyName("completed_at")]
    public required string CompletedAt { get; init; }
}