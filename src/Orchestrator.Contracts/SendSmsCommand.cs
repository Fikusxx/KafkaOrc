using System.Text.Json.Serialization;

namespace Orchestrator.Contracts;

public sealed record SendSmsCommand
{
    [JsonPropertyName("smsId")] public required long SmsId { get; init; }
    [JsonPropertyName("to")] public required long To { get; init; }
    [JsonPropertyName("text")] public required string Text { get; init; }
    [JsonPropertyName("title")] public required string Title { get; init; }
    [JsonPropertyName("priority")] public required int Priority { get; init; }
}