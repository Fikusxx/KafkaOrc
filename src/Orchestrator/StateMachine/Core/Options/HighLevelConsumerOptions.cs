using System.ComponentModel.DataAnnotations;

namespace Orchestrator.StateMachine.Core.Options;

internal sealed class HighLevelConsumerOptions
{
    [Required] [Range(1, int.MaxValue)] public required int PrefetchCount { get; init; }
    [Required] [Range(1, int.MaxValue)] public required int ConcurrentMessageLimit { get; init; }
    [Required] [Range(1, int.MaxValue)] public required ushort ConcurrentConsumerLimit { get; init; }
    [Required] [Range(1, int.MaxValue)] public required int ConcurrentDeliveryLimit { get; init; }
    [Required] [Range(1, ushort.MaxValue)] public required ushort CheckpointMessageCount { get; init; }
    [Required] [Range(1000, int.MaxValue)] public required int CheckpointIntervalMs { get; init; }
}