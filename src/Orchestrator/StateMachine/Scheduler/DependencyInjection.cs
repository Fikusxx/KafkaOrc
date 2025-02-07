using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orchestrator.StateMachine.Scheduler.Jobs;
using Quartz;

namespace Orchestrator.StateMachine.Scheduler;

internal static class DependencyInjection
{
    public static IServiceCollection AddScheduler(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSchedulerOptions(configuration);
        var schedulerOptions = configuration
            .GetSection(nameof(SchedulerOptions))
            .Get<SchedulerOptions>();
        
        services.AddQuartz(opt =>
        {
            opt.SchedulerId = schedulerOptions!.SchedulerId;
            opt.SchedulerName = schedulerOptions.SchedulerName;
            opt.MaxBatchSize = schedulerOptions.MaxBatchSize;
            opt.BatchTriggerAcquisitionFireAheadTimeWindow = TimeSpan.FromMilliseconds(schedulerOptions.BatchTriggerAcquisitionFireAheadTimeWindowMs);

            opt.AddDurableJobs();

            opt.UseDefaultThreadPool(maxConcurrency: schedulerOptions.MaxConcurrency);

            opt.UsePersistentStore(options =>
            {
                options.UseClustering();

                options.UsePostgres(cfg =>
                {
                    cfg.ConnectionString = schedulerOptions.Database.ConnectionString;
                    cfg.TablePrefix = schedulerOptions.Database.TablePrefix;
                }, dataSourceName: schedulerOptions.Database.Schema);

                options.UseSystemTextJsonSerializer();
                options.RetryInterval = TimeSpan.FromMilliseconds(schedulerOptions.Database.RetryIntervalMs);
            });
        });

        services.AddQuartzHostedService(opt => { opt.WaitForJobsToComplete = true; });

        return services;
    }

    private static void AddDurableJobs(this IServiceCollectionQuartzConfigurator cfg)
    {
        cfg.AddJob<SendPushSendTimeoutEventJob>(x =>
        {
            x.StoreDurably();
            x.WithIdentity(SendPushSendTimeoutEventJob.JobKey);
        });

        cfg.AddJob<SendPushDeliveryTimeoutEventJob>(x =>
        {
            x.StoreDurably();
            x.WithIdentity(SendPushDeliveryTimeoutEventJob.JobKey);
        });

        cfg.AddJob<SendSmsDeliveryTimeoutEventJob>(x =>
        {
            x.StoreDurably();
            x.WithIdentity(SendSmsDeliveryTimeoutEventJob.JobKey);
        });
    }

    private static void AddSchedulerOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SchedulerOptions>(configuration.GetSection(nameof(SchedulerOptions)))
            .AddOptionsWithValidateOnStart<SchedulerOptions>()
            .ValidateDataAnnotations();

        services.AddSingleton<SchedulerOptions>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<SchedulerOptions>>();
            return options.Value;
        });
    }
}