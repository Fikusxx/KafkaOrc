using System.ComponentModel.DataAnnotations;
using Confluent.Kafka;

namespace Orchestrator.StateMachine.Core.Options;

internal sealed class StateMachineOptions
{
    // Bus
    [Required] public required ClientConfig ClientConfig { get; init; }
    
    // State Machine
    [Required] [Range(1, int.MaxValue)] public required int RetryCount { get; init; }
    [Required] [Range(10, int.MaxValue)] public required int RetryIntervalMs { get; init; }
    
    // Producers
    [Required] public required ProducerOptions SendPushCommandProducerOptions { get; init; }
    [Required] public required ProducerOptions SendSmsCommandProducerOptions { get; init; }
    [Required] public required ProducerOptions PushSendTimeoutProducerOptions { get; init; }
    [Required] public required ProducerOptions PushDeliveryTimeoutProducerOptions { get; init; }
    [Required] public required ProducerOptions SmsDeliveryTimeoutProducerOptions { get; init; }
    [Required] public required ProducerOptions CascadingCommunicationCompletedProducerOptions { get; init; }
    
    // Consumers
    [Required] public required ConsumerOptions CascadingCommunicationRequestedConsumerOptions { get; init; }
    [Required] public required ConsumerOptions PushSendConsumerOptions { get; init; }
    [Required] public required ConsumerOptions PushDeliveryConsumerOptions { get; init; }
    [Required] public required ConsumerOptions PushSendTimeoutConsumerOptions { get; init; }
    [Required] public required ConsumerOptions PushDeliveryTimeoutConsumerOptions { get; init; }
    [Required] public required ConsumerOptions SmsSendConsumerOptions { get; init; }
    [Required] public required ConsumerOptions SmsDeliveryConsumerOptions { get; init; }
    [Required] public required ConsumerOptions SmsDeliveryTimeoutConsumerOptions { get; init; }
}

internal sealed class ProducerOptions
{
    [Required] public required string Topic { get; init; }
    [Required] public required ProducerConfig ProducerConfig { get; init; }
}

internal sealed class ConsumerOptions
{
    [Required] public required string Topic { get; init; }
    [Required] public required ConsumerConfig ConsumerConfig { get; init; }
    [Required] public required HighLevelConsumerOptions HighLevelConsumerOptions { get; init; }
}