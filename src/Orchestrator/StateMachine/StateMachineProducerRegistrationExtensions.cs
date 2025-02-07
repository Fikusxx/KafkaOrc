using CloudNative.CloudEvents;
using MassTransit;
using Orchestrator.Contracts;
using Orchestrator.StateMachine.Core.Cloud;
using Orchestrator.StateMachine.Core.Options;

namespace Orchestrator.StateMachine;

internal static class StateMachineProducerRegistrationExtensions
{
    public static void AddSendPushCommandProducer(this IRiderRegistrationConfigurator cfg,
        ProducerOptions options)
    {
        cfg.AddProducer<long, SendPushCommand>(options.Topic,
            options.ProducerConfig);
    }
    
    public static void AddSendSmsCommandProducer(this IRiderRegistrationConfigurator cfg,
        ProducerOptions options)
    {
        cfg.AddProducer<long, SendSmsCommand>(options.Topic,
            options.ProducerConfig);
    }
    
    public static void AddPushSendTimeoutProducer(this IRiderRegistrationConfigurator cfg,
        ProducerOptions options)
    {
        cfg.AddProducer<long, PushSendTimeoutEvent>(options.Topic,
            options.ProducerConfig);
    }
    
    public static void AddPushDeliveryTimeoutProducer(this IRiderRegistrationConfigurator cfg,
        ProducerOptions options)
    {
        cfg.AddProducer<long, PushDeliveryTimeoutEvent>(options.Topic,
            options.ProducerConfig);
    }
    
    public static void AddSmsDeliveryTimeoutProducer(this IRiderRegistrationConfigurator cfg,
        ProducerOptions options)
    {
        cfg.AddProducer<long, SmsDeliveryTimeoutEvent>(options.Topic,
            options.ProducerConfig);
    }
    
    public static void AddCommunicationCompletedProducer(this IRiderRegistrationConfigurator cfg,
        ProducerOptions options)
    {
        cfg.AddProducer<long, CloudEvent>(options.Topic,
            options.ProducerConfig,
            (_, producerCfg) =>
            {
                producerCfg.SetValueSerializer(new CloudEventJsonSerializer());
            });
    }
}