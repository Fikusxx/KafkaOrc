using System.ComponentModel.DataAnnotations;
using Confluent.Kafka;

namespace Orchestrator.Common;

internal sealed class HealthCheckOptions
{
    [Required] public required KafkaOptions KafkaOptions { get; init; }
}

internal sealed class KafkaOptions
{
    [Required] public required ProducerConfig ProducerConfig { get; init; }
    [Required] public required int TimeoutMs { get; init; }
}