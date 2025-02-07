using System.Text.Json.Serialization;

namespace Sms.Contracts;

public sealed record SmsSendEvent
{
    [JsonPropertyName("sms_id")] public required long SmsId { get; init; }
    [JsonPropertyName("external_id")] public string? ExternalId { get; init; }
    [JsonPropertyName("delivery_status")] public required int DeliveryStatus { get; init; }
}