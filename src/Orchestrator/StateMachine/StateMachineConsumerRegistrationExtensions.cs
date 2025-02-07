using MassTransit;
using Orchestrator.Contracts;
using Orchestrator.StateMachine.Core;
using Orchestrator.StateMachine.Core.Cloud;
using Orchestrator.StateMachine.Core.Options;
using Push.Contracts;
using Sms.Contracts;

namespace Orchestrator.StateMachine;

internal static class StateMachineConsumerRegistrationExtensions
{
    /// <summary>
    /// Endpoint for initial event that triggers state machine initialization.
    /// </summary>
    public static void AddCascadingCommunicationRequestedEndpoint(this IKafkaFactoryConfigurator cfg,
        IRiderRegistrationContext ctx, ConsumerOptions options)
    {
        cfg.TopicEndpoint<long, CascadingCommunicationRequestedEvent>(options.Topic,
            options.ConsumerConfig,
            e =>
            {
                e.CheckpointInterval = TimeSpan.FromMilliseconds(options.HighLevelConsumerOptions.CheckpointIntervalMs);
                e.CheckpointMessageCount = options.HighLevelConsumerOptions.CheckpointMessageCount;

                e.PrefetchCount = options.HighLevelConsumerOptions.PrefetchCount;
                e.ConcurrentMessageLimit = options.HighLevelConsumerOptions.ConcurrentMessageLimit;
                e.ConcurrentConsumerLimit = options.HighLevelConsumerOptions.ConcurrentConsumerLimit;
                e.ConcurrentDeliveryLimit = options.HighLevelConsumerOptions.ConcurrentDeliveryLimit;

                e.SetValueDeserializer(new CascadingCommunicationRequestedEventCloudEventJsonDeserializer());
                e.ConfigureSaga<CascadingCommunicationState>(ctx);
            });
    }

    /// <summary>
    /// Endpoint for receiving statuses for push sending.
    /// </summary>
    public static void AddPushSendEndpoint(this IKafkaFactoryConfigurator cfg,
        IRiderRegistrationContext ctx, ConsumerOptions options)
    {
        cfg.TopicEndpoint<long, PushSendEvent>(options.Topic,
            options.ConsumerConfig,
            e =>
            {
                e.CheckpointInterval = TimeSpan.FromMilliseconds(options.HighLevelConsumerOptions.CheckpointIntervalMs);
                e.CheckpointMessageCount = options.HighLevelConsumerOptions.CheckpointMessageCount;

                e.PrefetchCount = options.HighLevelConsumerOptions.PrefetchCount;
                e.ConcurrentMessageLimit = options.HighLevelConsumerOptions.ConcurrentMessageLimit;
                e.ConcurrentConsumerLimit = options.HighLevelConsumerOptions.ConcurrentConsumerLimit;
                e.ConcurrentDeliveryLimit = options.HighLevelConsumerOptions.ConcurrentDeliveryLimit;

                e.SetValueDeserializer(new PushSendEventCloudEventJsonDeserializer());
                e.ConfigureSaga<CascadingCommunicationState>(ctx);
            });
    }

    /// <summary>
    /// Endpoint for receiving statuses for push delivery to a client.
    /// </summary>
    public static void AddPushDeliveryEndpoint(this IKafkaFactoryConfigurator cfg,
        IRiderRegistrationContext ctx, ConsumerOptions options)
    {
        cfg.TopicEndpoint<long, PushDeliveryEvent>(options.Topic,
            options.ConsumerConfig,
            e =>
            {
                e.CheckpointInterval = TimeSpan.FromMilliseconds(options.HighLevelConsumerOptions.CheckpointIntervalMs);
                e.CheckpointMessageCount = options.HighLevelConsumerOptions.CheckpointMessageCount;

                e.PrefetchCount = options.HighLevelConsumerOptions.PrefetchCount;
                e.ConcurrentMessageLimit = options.HighLevelConsumerOptions.ConcurrentMessageLimit;
                e.ConcurrentConsumerLimit = options.HighLevelConsumerOptions.ConcurrentConsumerLimit;
                e.ConcurrentDeliveryLimit = options.HighLevelConsumerOptions.ConcurrentDeliveryLimit;

                e.SetValueDeserializer(new PushDeliveryEventCloudEventJsonDeserializer());
                e.ConfigureSaga<CascadingCommunicationState>(ctx);
            });
    }

    /// <summary>
    /// Endpoint for receiving timeout on receiving push send status.
    /// </summary>
    public static void AddPushSendTimeoutEndpoint(this IKafkaFactoryConfigurator cfg,
        IRiderRegistrationContext ctx, ConsumerOptions options)
    {
        cfg.TopicEndpoint<long, PushSendTimeoutEvent>(options.Topic,
            options.ConsumerConfig,
            e =>
            {
                e.CheckpointInterval = TimeSpan.FromMilliseconds(options.HighLevelConsumerOptions.CheckpointIntervalMs);
                e.CheckpointMessageCount = options.HighLevelConsumerOptions.CheckpointMessageCount;

                e.PrefetchCount = options.HighLevelConsumerOptions.PrefetchCount;
                e.ConcurrentMessageLimit = options.HighLevelConsumerOptions.ConcurrentMessageLimit;
                e.ConcurrentConsumerLimit = options.HighLevelConsumerOptions.ConcurrentConsumerLimit;
                e.ConcurrentDeliveryLimit = options.HighLevelConsumerOptions.ConcurrentDeliveryLimit;

                e.ConfigureSaga<CascadingCommunicationState>(ctx);
            });
    }

    /// <summary>
    /// Endpoint for receiving timeout on receiving push delivery status.
    /// </summary>
    public static void AddPushDeliveryTimeoutEndpoint(this IKafkaFactoryConfigurator cfg,
        IRiderRegistrationContext ctx, ConsumerOptions options)
    {
        cfg.TopicEndpoint<long, PushDeliveryTimeoutEvent>(options.Topic,
            options.ConsumerConfig,
            e =>
            {
                e.CheckpointInterval = TimeSpan.FromMilliseconds(options.HighLevelConsumerOptions.CheckpointIntervalMs);
                e.CheckpointMessageCount = options.HighLevelConsumerOptions.CheckpointMessageCount;

                e.PrefetchCount = options.HighLevelConsumerOptions.PrefetchCount;
                e.ConcurrentMessageLimit = options.HighLevelConsumerOptions.ConcurrentMessageLimit;
                e.ConcurrentConsumerLimit = options.HighLevelConsumerOptions.ConcurrentConsumerLimit;
                e.ConcurrentDeliveryLimit = options.HighLevelConsumerOptions.ConcurrentDeliveryLimit;

                e.ConfigureSaga<CascadingCommunicationState>(ctx);
            });
    }

    /// <summary>
    /// Endpoint for receiving statuses for sms sending.
    /// </summary>
    public static void AddSmsSendEndpoint(this IKafkaFactoryConfigurator cfg,
        IRiderRegistrationContext ctx, ConsumerOptions options)
    {
        cfg.TopicEndpoint<long, SmsSendEvent>(options.Topic,
            options.ConsumerConfig,
            e =>
            {
                e.CheckpointInterval = TimeSpan.FromMilliseconds(options.HighLevelConsumerOptions.CheckpointIntervalMs);
                e.CheckpointMessageCount = options.HighLevelConsumerOptions.CheckpointMessageCount;

                e.PrefetchCount = options.HighLevelConsumerOptions.PrefetchCount;
                e.ConcurrentMessageLimit = options.HighLevelConsumerOptions.ConcurrentMessageLimit;
                e.ConcurrentConsumerLimit = options.HighLevelConsumerOptions.ConcurrentConsumerLimit;
                e.ConcurrentDeliveryLimit = options.HighLevelConsumerOptions.ConcurrentDeliveryLimit;

                e.SetValueDeserializer(new SmsSendEventCloudEventJsonDeserializer());
                e.ConfigureSaga<CascadingCommunicationState>(ctx);
            });
    }

    /// <summary>
    /// Endpoint for receiving statuses for sms delivery to a client.
    /// </summary>
    public static void AddSmsDeliveryEndpoint(this IKafkaFactoryConfigurator cfg,
        IRiderRegistrationContext ctx, ConsumerOptions options)
    {
        cfg.TopicEndpoint<long, SmsDeliveryEvent>(options.Topic,
            options.ConsumerConfig,
            e =>
            {
                e.CheckpointInterval = TimeSpan.FromMilliseconds(options.HighLevelConsumerOptions.CheckpointIntervalMs);
                e.CheckpointMessageCount = options.HighLevelConsumerOptions.CheckpointMessageCount;

                e.PrefetchCount = options.HighLevelConsumerOptions.PrefetchCount;
                e.ConcurrentMessageLimit = options.HighLevelConsumerOptions.ConcurrentMessageLimit;
                e.ConcurrentConsumerLimit = options.HighLevelConsumerOptions.ConcurrentConsumerLimit;
                e.ConcurrentDeliveryLimit = options.HighLevelConsumerOptions.ConcurrentDeliveryLimit;

                e.SetValueDeserializer(new SmsDeliveryEventCloudEventJsonDeserializer());
                e.ConfigureSaga<CascadingCommunicationState>(ctx);
            });
    }

    /// <summary>
    /// Endpoint for receiving timeout on receiving sms delivery status.
    /// </summary>
    public static void AddSmsDeliveryTimeoutEndpoint(this IKafkaFactoryConfigurator cfg,
        IRiderRegistrationContext ctx, ConsumerOptions options)
    {
        cfg.TopicEndpoint<long, SmsDeliveryTimeoutEvent>(options.Topic,
            options.ConsumerConfig,
            e =>
            {
                e.CheckpointInterval = TimeSpan.FromMilliseconds(options.HighLevelConsumerOptions.CheckpointIntervalMs);
                e.CheckpointMessageCount = options.HighLevelConsumerOptions.CheckpointMessageCount;

                e.PrefetchCount = options.HighLevelConsumerOptions.PrefetchCount;
                e.ConcurrentMessageLimit = options.HighLevelConsumerOptions.ConcurrentMessageLimit;
                e.ConcurrentConsumerLimit = options.HighLevelConsumerOptions.ConcurrentConsumerLimit;
                e.ConcurrentDeliveryLimit = options.HighLevelConsumerOptions.ConcurrentDeliveryLimit;

                e.ConfigureSaga<CascadingCommunicationState>(ctx);
            });
    }
}