using System.Text.Json.Serialization;

namespace Orchestrator.Contracts;

public sealed record CascadingCommunicationRequestedEvent
{
    /// <summary>
    /// Globally unique id of communication.
    /// </summary>
    [JsonPropertyName("communication_id")]
    public required long CommunicationId { get; init; }

    /// <summary>
    /// Timeout for push to be delivered to a client.
    /// </summary>
    [JsonPropertyName("push_delivery_timeout_seconds")]
    public required int PushDeliveryTimeoutSeconds { get; init; }

    /// <summary>
    /// Sms details.
    /// </summary>
    [JsonPropertyName("sms_data")]
    public required SmsData SmsData { get; init; }

    /// <summary>
    /// Push details.
    /// </summary>
    [JsonPropertyName("push_data")]
    public required PushData PushData { get; init; }
}

public sealed record SmsData
{
    [JsonPropertyName("priority")] public required int Priority { get; init; }
    [JsonPropertyName("to")] public required long To { get; init; }
    [JsonPropertyName("text")] public required string Text { get; init; }
    [JsonPropertyName("title")] public required string Title { get; init; }
}

public sealed record PushData
{
    [JsonPropertyName("type")] public required string Type { get; init; }
    [JsonPropertyName("clientCode")] public required string ClientCode { get; init; }
    [JsonPropertyName("title")] public required string Title { get; init; }
    [JsonPropertyName("body")] public required string Body { get; init; }
    [JsonPropertyName("secondsToLive")] public required int SecondsToLive { get; init; }
    [JsonPropertyName("marketingInfoId")] public string? MarketingInfoId { get; init; }
    [JsonPropertyName("transactionLink")] public string? TransactionLink { get; init; }
}

// "data" : {
//     "communication_id" : 123, --- id коммуникации / id каскадного сообщения 
//     "push_delivery_timeout_seconds": 50,  --- таймаут для ожидания отправки смс в секундах, для дефолтного значения передать null 
//     "sms_data" : {  --- все данные по смс
//         "priority" : 1,
//         "to" : 89803539114, 
//         "title" : "", 
//         "text" : ""
//     },
//     "push_data" : { --- все данные по пушу
//         "type" : "type1", 
//         "client_code": "123", 
//         "title" : "", 
//         "body" : "", 
//         "seconds_to_live" : 60, 
//         "marketing_info_id" : "123123", 
//         "transaction_link" : "http://mcb.ru"
//     }    
// }