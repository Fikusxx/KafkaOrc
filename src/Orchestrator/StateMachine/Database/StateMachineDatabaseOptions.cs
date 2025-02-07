using System.ComponentModel.DataAnnotations;

namespace Orchestrator.StateMachine.Database;

internal sealed class StateMachineDatabaseOptions
{
    [Required] public required string Connection { get; init; }
    
    [Required] public required string Schema { get; init; }

    [Required] [Range(1, 5)] public required int MaxRetryCount { get; init; }

    [Required] [Range(10, 100)] public required int MaxRetryDelayMs { get; init; }
}