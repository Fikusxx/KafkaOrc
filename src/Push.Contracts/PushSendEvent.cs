using System.Text.Json.Serialization;

namespace Push.Contracts;

public sealed record PushSendEvent
{
    [JsonPropertyName("push_id")] public required long PushId { get; init; }
    [JsonPropertyName("delivery_status")] public required int DeliveryStatus { get; init; }
    [JsonPropertyName("external_id")] public string? ExternalId { get; init; }
}