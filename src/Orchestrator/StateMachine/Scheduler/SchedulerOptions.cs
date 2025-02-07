using System.ComponentModel.DataAnnotations;

namespace Orchestrator.StateMachine.Scheduler;

internal sealed class SchedulerOptions
{
    /// <summary>
    /// Must be unique within a cluster.
    /// </summary>
    [Required]
    public required string SchedulerId { get; init; }

    /// <summary>
    /// Must be the same within a cluster.
    /// </summary>
    [Required]
    public required string SchedulerName { get; init; }

    /// <summary>
    /// The maximum number of triggers that a scheduler node is allowed to acquire (for firing) at once. Default value is 1.
    /// The larger the number, the more efficient firing is (in situations where there are very many triggers needing to be fired all at once) - but at the cost of possible imbalanced load between cluster nodes.
    /// </summary>
    [Required]
    public required int MaxBatchSize { get; init; }

    /// <summary>
    /// The amount of time that a trigger is allowed to be acquired and fired ahead of its scheduled fire time.
    /// Defaults to TimeSpan.Zero.
    /// The larger the number, the more likely batch acquisition of triggers to fire will be able to select and fire more than 1 trigger at a time -at the cost of trigger schedule not being honored precisely (triggers may fire this amount early).
    /// This may be useful (for performance’s sake) in situations where the scheduler has very large numbers of triggers that need to be fired at or near the same time.
    /// </summary>
    [Required]
    public required int BatchTriggerAcquisitionFireAheadTimeWindowMs { get; init; }

    /// <summary>
    /// The maximum number of thread pool tasks which can be executing in parallel.
    /// </summary>
    [Required]
    public required int MaxConcurrency { get; init; }
    
    /// <summary>
    /// Database options for persistent store.
    /// </summary>
    [Required]
    public required Database Database { get; init; }
}

internal sealed class Database
{
    [Required] public required string ConnectionString { get; init; }

    /// <summary>
    /// The prefix that should be pre-pended to all table names, defaults to qrtz_.
    /// Should be schema + prefix, i.e "schema_name.qrtz_".
    /// </summary>
    [Required] public required string TablePrefix { get; init; }

    [Required] public required string Schema { get; init; }

    /// <summary>
    /// Sets the database retry interval.
    /// </summary>
    [Required]
    public required int RetryIntervalMs { get; init; }
}