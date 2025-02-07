using Confluent.Kafka;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orchestrator.Contracts;
using Orchestrator.StateMachine.Core;
using Orchestrator.StateMachine.Core.Options;
using Orchestrator.StateMachine.Database;
using Push.Contracts;
using Sms.Contracts;

namespace Orchestrator.StateMachine;

internal static class DependencyInjection
{
    public static IServiceCollection AddOrchestration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStateMachineOptions(configuration)
            ;
        services.AddMassTransit(massTransit =>
        {
            massTransit.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));

            using var sp = massTransit.BuildServiceProvider();
            var stateMachineOptions = sp.GetRequiredService<StateMachineOptions>();

            massTransit.AddRider(rider =>
            {
                rider
                    .AddSagaStateMachine<CascadingCommunicationStateMachine, CascadingCommunicationState>(
                        (_, sagaCfg) =>
                        {
                            sagaCfg.UseMessageRetry(config =>
                            {
                                config.Interval(stateMachineOptions.RetryCount,
                                    stateMachineOptions.RetryIntervalMs);
                            });
                        })
                    .AddStateMachinePersistence();

                rider.AddSendPushCommandProducer(stateMachineOptions.SendPushCommandProducerOptions);
                rider.AddSendSmsCommandProducer(stateMachineOptions.SendSmsCommandProducerOptions);
                rider.AddPushSendTimeoutProducer(stateMachineOptions.PushSendTimeoutProducerOptions);
                rider.AddPushDeliveryTimeoutProducer(stateMachineOptions.PushDeliveryTimeoutProducerOptions);
                rider.AddSmsDeliveryTimeoutProducer(stateMachineOptions.SmsDeliveryTimeoutProducerOptions);
                rider.AddCommunicationCompletedProducer(stateMachineOptions
                    .CascadingCommunicationCompletedProducerOptions);

                #region TBD

                rider.AddProducer<long, CascadingCommunicationRequestedEvent>("communication.requested",
                    new ProducerConfig { BootstrapServers = "localhost:9092" },
                    (_, producerCfg) =>
                    {
                        producerCfg.SetValueSerializer(new CascadingCommunicationRequestedEvent2());
                    });
                
                rider.AddProducer<long, PushSendEvent>("push.send",
                    new ProducerConfig { BootstrapServers = "localhost:9092" },
                    (_, producerCfg) =>
                    {
                        producerCfg.SetValueSerializer(new PushSendEvent2());
                    });
                
                rider.AddProducer<long, PushDeliveryEvent>("push.delivery",
                    new ProducerConfig { BootstrapServers = "localhost:9092" },
                    (_, producerCfg) =>
                    {
                        producerCfg.SetValueSerializer(new PushDeliveryEvent2());
                    });
                
                rider.AddProducer<long, SmsSendEvent>("sms.send",
                    new ProducerConfig { BootstrapServers = "localhost:9092" },
                    (_, producerCfg) =>
                    {
                        producerCfg.SetValueSerializer(new SmsSendEvent2());
                    });
                
                rider.AddProducer<long, SmsDeliveryEvent>("sms.delivery",
                    new ProducerConfig { BootstrapServers = "localhost:9092" },
                    (_, producerCfg) =>
                    {
                        producerCfg.SetValueSerializer(new SmsDeliveryEvent2());
                    });

                #endregion

                // add temp producers for testing
                // rider.AddProducer<Guid, SendSmsEvent>("sms");
                // rider.AddProducer<Guid, SmsStatusEvent>("sms-status");

                rider.UsingKafka(stateMachineOptions.ClientConfig,
                    (ctx, cfg) =>
                    {
                        cfg.AddCascadingCommunicationRequestedEndpoint(ctx,
                            stateMachineOptions.CascadingCommunicationRequestedConsumerOptions);
                        cfg.AddPushSendEndpoint(ctx, stateMachineOptions.PushSendConsumerOptions);
                        cfg.AddPushDeliveryEndpoint(ctx, stateMachineOptions.PushDeliveryConsumerOptions);
                        cfg.AddPushSendTimeoutEndpoint(ctx, stateMachineOptions.PushSendTimeoutConsumerOptions);
                        cfg.AddPushDeliveryTimeoutEndpoint(ctx, stateMachineOptions.PushDeliveryTimeoutConsumerOptions);
                        cfg.AddSmsSendEndpoint(ctx, stateMachineOptions.SmsSendConsumerOptions);
                        cfg.AddSmsDeliveryEndpoint(ctx, stateMachineOptions.SmsDeliveryConsumerOptions);
                        cfg.AddSmsDeliveryTimeoutEndpoint(ctx, stateMachineOptions.SmsDeliveryTimeoutConsumerOptions);
                    });
            });
        });

        return services;
    }

    private static void AddStateMachineOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<StateMachineOptions>(configuration.GetSection(nameof(StateMachineOptions)))
            .AddOptionsWithValidateOnStart<StateMachineOptions>()
            .ValidateDataAnnotations();

        services.AddSingleton<StateMachineOptions>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<StateMachineOptions>>();
            return options.Value;
        });
    }
}