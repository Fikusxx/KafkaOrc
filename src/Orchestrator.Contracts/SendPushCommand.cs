using System.Text.Json.Serialization;

namespace Orchestrator.Contracts;

public sealed record SendPushCommand
{
    [JsonPropertyName("PushID")] public required long PushId { get; init; }
    [JsonPropertyName("type")] public required string Type { get; init; }
    [JsonPropertyName("clientCode")] public required string ClientCode { get; init; }
    [JsonPropertyName("title")] public required string Title { get; init; }
    [JsonPropertyName("body")] public required string Body { get; init; }
    [JsonPropertyName("secondsToLive")] public required int SecondsToLive { get; init; }
    [JsonPropertyName("marketingInfoId")] public string? MarketingInfoId { get; init; }
    [JsonPropertyName("transactionLink")] public string? TransactionLink { get; init; }
}