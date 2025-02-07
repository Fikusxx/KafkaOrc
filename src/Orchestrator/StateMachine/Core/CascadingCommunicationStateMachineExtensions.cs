using MassTransit;
using Orchestrator.Contracts;
using Push.Contracts;
using Sms.Contracts;

namespace Orchestrator.StateMachine.Core;

internal static class CascadingCommunicationStateMachineExtensions
{
    /// <summary>
    /// Initialize saga with initial communication data upon creation.
    /// </summary>
    public static EventActivityBinder<CascadingCommunicationState, CascadingCommunicationRequestedEvent> InitializeSaga(
        this EventActivityBinder<CascadingCommunicationState, CascadingCommunicationRequestedEvent> binder)
        => binder.Then(context =>
        {
            LogContext.Info?.Log("Initializing saga with {CommunicationId}.", context.Message.CommunicationId);

            context.Saga.CommunicationId = context.Message.CommunicationId;
            context.Saga.PushDeliveryTimeoutSeconds = context.Message.PushDeliveryTimeoutSeconds;

            context.Saga.SmsData = new SmsData
            {
                Priority = context.Message.SmsData.Priority,
                Text = context.Message.SmsData.Text,
                Title = context.Message.SmsData.Title,
                To = context.Message.SmsData.To
            };
        });

    /// <summary>
    /// Update sage when push send timeouts.
    /// </summary>
    public static EventActivityBinder<CascadingCommunicationState, PushSendTimeoutEvent> UpdateSagaWhenPushSendTimedOut(
        this EventActivityBinder<CascadingCommunicationState, PushSendTimeoutEvent> binder)
        => binder.Then(context =>
        {
            LogContext.Info?.Log("Push send with {CommunicationId} timed out.", context.Saga.CommunicationId);
        });
    
    /// <summary>
    /// Update saga when push delivery timeouts.
    /// </summary>
    public static EventActivityBinder<CascadingCommunicationState, PushDeliveryTimeoutEvent> UpdateSagaWhenPushDeliveryTimedOut(
        this EventActivityBinder<CascadingCommunicationState, PushDeliveryTimeoutEvent> binder)
        => binder.Then(context =>
        {
            LogContext.Info?.Log("Push delivery with {CommunicationId} timed out.", context.Saga.CommunicationId);
        });

    /// <summary>
    /// Update saga when push has not been sent successfully.
    /// </summary>
    public static EventActivityBinder<CascadingCommunicationState, PushSendEvent> UpdateSagaWhenPushSendNotDelivered(
        this EventActivityBinder<CascadingCommunicationState, PushSendEvent> binder)
        => binder.Then(context =>
        {
            LogContext.Info?.Log("Push with {CommunicationId} has not been sent successfully.",
                context.Saga.CommunicationId);
        });

    /// <summary>
    /// Update saga when push has been sent successfully.
    /// </summary>
    public static EventActivityBinder<CascadingCommunicationState, PushSendEvent> UpdateSagaWhenPushSendDelivered(
        this EventActivityBinder<CascadingCommunicationState, PushSendEvent> binder)
        => binder.Then(context =>
        {
            LogContext.Info?.Log("Push with {CommunicationId} has been sent successfully.",
                context.Saga.CommunicationId);
        });
    
    /// <summary>
    /// Update saga when push has not been delivered successfully.
    /// </summary>
    public static EventActivityBinder<CascadingCommunicationState, PushDeliveryEvent> UpdateSagaWhenPushDeliveryNotDelivered(
        this EventActivityBinder<CascadingCommunicationState, PushDeliveryEvent> binder)
        => binder.Then(context =>
        {
            LogContext.Info?.Log("Push with {CommunicationId} has not been delivered successfully.",
                context.Saga.CommunicationId);
        });
    
    /// <summary>
    /// Update saga when push has been delivered successfully.
    /// </summary>
    public static EventActivityBinder<CascadingCommunicationState, PushDeliveryEvent> UpdateSagaWhenPushDeliveryDelivered(
        this EventActivityBinder<CascadingCommunicationState, PushDeliveryEvent> binder)
        => binder.Then(context =>
        {
            context.Saga.DeliveryChannel = DeliveryChannel.Push;
            context.Saga.Success = true;
            
            LogContext.Info?.Log("Push with {CommunicationId} has been delivered successfully.",
                context.Saga.CommunicationId);
        });
    
    /// <summary>
    /// Update saga when sms delivery timeouts.
    /// </summary>
    public static EventActivityBinder<CascadingCommunicationState, SmsDeliveryTimeoutEvent> UpdateSagaWhenSmsDeliveryTimedOut(
        this EventActivityBinder<CascadingCommunicationState, SmsDeliveryTimeoutEvent> binder)
        => binder.Then(context =>
        {
            context.Saga.Success = false;
            
            LogContext.Info?.Log("Sms delivery with {CommunicationId} timed out.",
                context.Saga.CommunicationId);
        });
    
    /// <summary>
    /// Update saga when sms has not been sent successfully.
    /// </summary>
    public static EventActivityBinder<CascadingCommunicationState, SmsSendEvent> UpdateSagaWhenSmsSendNotDelivered(
        this EventActivityBinder<CascadingCommunicationState, SmsSendEvent> binder)
        => binder.Then(context =>
        {
            context.Saga.Success = false;
            
            LogContext.Info?.Log("Sms with {CommunicationId} has not been sent successfully.",
                context.Saga.CommunicationId);
        });

    /// <summary>
    /// Update saga when sms has been sent successfully.
    /// </summary>
    public static EventActivityBinder<CascadingCommunicationState, SmsSendEvent> UpdateSagaWhenSmsSendDelivered(
        this EventActivityBinder<CascadingCommunicationState, SmsSendEvent> binder)
        => binder.Then(context =>
        {
            LogContext.Info?.Log("Sms with {CommunicationId} has been sent successfully.",
                context.Saga.CommunicationId);
        });
    
    /// <summary>
    /// Update saga when sms has not been delivered successfully.
    /// </summary>
    public static EventActivityBinder<CascadingCommunicationState, SmsDeliveryEvent> UpdateSagaWhenSmsDeliveryNotDelivered(
        this EventActivityBinder<CascadingCommunicationState, SmsDeliveryEvent> binder)
        => binder.Then(context =>
        {
            context.Saga.Success = false;
            
            LogContext.Info?.Log("Sms with {CommunicationId} has not been delivered successfully.",
                context.Saga.CommunicationId);
        });
    
    /// <summary>
    /// Update saga when sms has been delivered successfully.
    /// </summary>
    public static EventActivityBinder<CascadingCommunicationState, SmsDeliveryEvent> UpdateSagaWhenSmsDeliveryDelivered(
        this EventActivityBinder<CascadingCommunicationState, SmsDeliveryEvent> binder)
        => binder.Then(context =>
        {
            context.Saga.DeliveryChannel = DeliveryChannel.Sms;
            context.Saga.Success = true;
            
            LogContext.Info?.Log("Sms with {CommunicationId} has been delivered successfully.",
                context.Saga.CommunicationId);
        });
    
    /// <summary>
    /// Update saga when has been completed.
    /// </summary>
    public static EventActivityBinder<CascadingCommunicationState> UpdateSagaWhenCompleted(
        this EventActivityBinder<CascadingCommunicationState> binder)
        => binder.Then(context =>
        {
            LogContext.Info?.Log("Saga with {CommunicationId} has been completed.",
                context.Saga.CommunicationId);
        });
}