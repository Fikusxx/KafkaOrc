using MassTransit;
using Orchestrator.Contracts;
using Orchestrator.StateMachine.ActivitiesByEvent.CascadingCommunicationRequested;
using Orchestrator.StateMachine.ActivitiesByEvent.Final;
using Push.Contracts;
using Sms.Contracts;

namespace Orchestrator.StateMachine.Core;

internal sealed class CascadingCommunicationStateMachine : MassTransitStateMachine<CascadingCommunicationState>
{
    public State? AwaitingPushSend { get; set; }
    public State? AwaitingPushDelivery { get; set; }
    public State? AwaitingSmsSend { get; set; }
    public State? AwaitingSmsDelivery { get; set; }

    public Event<CascadingCommunicationRequestedEvent>? CascadingCommunicationRequestedEvent { get; set; }
    public Event<PushSendTimeoutEvent>? PushSendTimeoutEvent { get; set; }
    public Event<PushDeliveryTimeoutEvent>? PushDeliveryTimeoutEvent { get; set; }
    public Event<SmsDeliveryTimeoutEvent>? SmsDeliveryTimeoutEvent { get; set; }
    public Event<PushSendEvent>? PushSendEvent { get; set; }
    public Event<PushDeliveryEvent>? PushDeliveryEvent { get; set; }
    public Event<SmsSendEvent>? SmsSendEvent { get; set; }
    public Event<SmsDeliveryEvent>? SmsDeliveryEvent { get; set; }

    public CascadingCommunicationStateMachine()
    {
        RegisterStates();
        CorrelateEvents();
        IgnoreEvents();

        Initially(
            When(CascadingCommunicationRequestedEvent)
                .InitializeSaga()
                .Activity(x => x.OfType<SendPushCommandActivity>())
                .Activity(x => x.OfType<SendPushSendTimeoutEventActivity>())
                .Activity(x => x.OfType<SendPushDeliveryTimeoutEventActivity>())
                .TransitionTo(AwaitingPushSend));

        During(AwaitingPushSend,
            When(PushSendEvent,
                    filter => filter.Message.DeliveryStatus == 0)
                .UpdateSagaWhenPushSendNotDelivered()
                // .Activity(x => x.OfType<SendSmsCommandActivity>())
                // .Activity(x => x.OfType<SendSmsDeliveryTimeoutEventActivity>())
                .TransitionTo(AwaitingSmsSend),
            When(PushSendEvent,
                    filter => filter.Message.DeliveryStatus == 1)
                .UpdateSagaWhenPushSendDelivered()
                .TransitionTo(AwaitingPushDelivery),
            When(PushSendTimeoutEvent)
                .UpdateSagaWhenPushSendTimedOut()
                // .Activity(x => x.OfType<ActivitiesByEvent.PushSendTimeout.SendSmsCommandActivity>())
                // .Activity(x => x.OfType<ActivitiesByEvent.PushSendTimeout.SendSmsDeliveryTimeoutEventActivity>())
                .TransitionTo(AwaitingSmsSend),
            When(PushDeliveryTimeoutEvent)
                .UpdateSagaWhenPushDeliveryTimedOut()
                // .Activity(x => x.OfType<ActivitiesByEvent.PushDeliveryTimeout.SendSmsCommandActivity>())
                // .Activity(x => x.OfType<ActivitiesByEvent.PushDeliveryTimeout.SendSmsDeliveryTimeoutEventActivity>())
                .TransitionTo(AwaitingSmsSend),
            When(PushDeliveryEvent,
                    filter => filter.Message.DeliveryStatus == 0)
                .UpdateSagaWhenPushDeliveryNotDelivered()
                // .Activity(x => x.OfType<ActivitiesByEvent.PushDelivery.SendSmsCommandActivity>())
                // .Activity(x => x.OfType<ActivitiesByEvent.PushDelivery.SendSmsDeliveryTimeoutEventActivity>())
                .TransitionTo(AwaitingSmsSend),
            When(PushDeliveryEvent,
                    filter => filter.Message.DeliveryStatus == 1)
                .UpdateSagaWhenPushDeliveryDelivered()
                .TransitionTo(Final)
        );

        During(AwaitingPushDelivery,
            When(PushDeliveryTimeoutEvent)
                .UpdateSagaWhenPushDeliveryTimedOut()
                // .Activity(x => x.OfType<ActivitiesByEvent.PushDeliveryTimeout.SendSmsCommandActivity>())
                // .Activity(x => x.OfType<ActivitiesByEvent.PushDeliveryTimeout.SendSmsDeliveryTimeoutEventActivity>())
                .TransitionTo(AwaitingSmsSend),
            When(PushDeliveryEvent,
                    filter => filter.Message.DeliveryStatus == 0)
                .UpdateSagaWhenPushDeliveryNotDelivered()
                // .Activity(x => x.OfType<ActivitiesByEvent.PushDelivery.SendSmsCommandActivity>())
                // .Activity(x => x.OfType<ActivitiesByEvent.PushDelivery.SendSmsDeliveryTimeoutEventActivity>())
                .TransitionTo(AwaitingSmsSend),
            When(PushDeliveryEvent,
                    filter => filter.Message.DeliveryStatus == 1)
                .UpdateSagaWhenPushDeliveryDelivered()
                .TransitionTo(Final)
        );

        WhenEnter(AwaitingSmsSend,
            activityCallback => activityCallback
                .Activity(x => x.OfType<ActivitiesByEvent.SendSmsCommandActivity>())
                .Activity(x => x.OfType<ActivitiesByEvent.SendSmsDeliveryTimeoutEventActivity>())
        );

        During(AwaitingSmsSend,
            When(SmsDeliveryTimeoutEvent)
                .UpdateSagaWhenSmsDeliveryTimedOut()
                .TransitionTo(Final),
            When(SmsSendEvent,
                    filter => filter.Message.DeliveryStatus == 0)
                .UpdateSagaWhenSmsSendNotDelivered()
                .TransitionTo(Final),
            When(SmsSendEvent,
                    filter => filter.Message.DeliveryStatus == 1)
                .UpdateSagaWhenSmsSendDelivered()
                .TransitionTo(AwaitingSmsDelivery),
            When(SmsDeliveryEvent,
                    filter => filter.Message.DeliveryStatus == 0)
                .UpdateSagaWhenSmsDeliveryNotDelivered()
                .TransitionTo(Final),
            When(SmsDeliveryEvent,
                    filter => filter.Message.DeliveryStatus == 1)
                .UpdateSagaWhenSmsDeliveryDelivered()
                .TransitionTo(Final)
        );

        During(AwaitingSmsDelivery,
            When(SmsDeliveryTimeoutEvent)
                .UpdateSagaWhenSmsDeliveryTimedOut()
                .TransitionTo(Final),
            When(SmsDeliveryEvent,
                    filter => filter.Message.DeliveryStatus == 0)
                .UpdateSagaWhenSmsDeliveryNotDelivered()
                .TransitionTo(Final),
            When(SmsDeliveryEvent,
                    filter => filter.Message.DeliveryStatus == 1)
                .UpdateSagaWhenSmsDeliveryDelivered()
                .TransitionTo(Final)
        );

        WhenEnter(Final,
            activityCallback => activityCallback
                .UpdateSagaWhenCompleted()
                .Activity(x => x.OfType<SendCascadingCommunicationCompletedEventActivity>())
                .Finalize());
    }

    private void RegisterStates()
        => InstanceState(x => x.CurrentState);

    private void IgnoreEvents()
    {
        // Ignore only initial event that can trigger entire state machine.
        // All push events are useful, as they were (could be) sent by this point.
        // Sms events didnt trigger yet.
        During(AwaitingPushSend,
            Ignore(CascadingCommunicationRequestedEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(AwaitingPushSend),
                    nameof(CascadingCommunicationRequestedEvent));
                return true;
            }));

        // Ignore initial event that can trigger entire state machine.
        // Push send + timeout are ignored, cause state machine is past that state.
        // Sms events didnt trigger yet.
        During(AwaitingPushDelivery,
            Ignore(CascadingCommunicationRequestedEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(AwaitingPushDelivery),
                    nameof(CascadingCommunicationRequestedEvent));
                return true;
            }),
            Ignore(PushSendEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(AwaitingPushDelivery),
                    nameof(PushSendEvent));
                return true;
            }),
            Ignore(PushSendTimeoutEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(AwaitingPushDelivery),
                    nameof(PushSendTimeoutEvent));
                return true;
            }));

        // Ignore initial event that can trigger entire state machine.
        // Ignore all push events, cause state machine is past that state.
        // Sms events are useful, as they were (could be) sent by this point.
        During(AwaitingSmsSend,
            Ignore(CascadingCommunicationRequestedEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(AwaitingSmsSend),
                    nameof(CascadingCommunicationRequestedEvent));
                return true;
            }),
            Ignore(PushSendEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(AwaitingSmsSend),
                    nameof(PushSendEvent));
                return true;
            }),
            Ignore(PushSendTimeoutEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(AwaitingSmsSend),
                    nameof(PushSendTimeoutEvent));
                return true;
            }),
            Ignore(PushDeliveryEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(AwaitingSmsSend),
                    nameof(PushDeliveryEvent));
                return true;
            }),
            Ignore(PushDeliveryTimeoutEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(AwaitingSmsSend),
                    nameof(PushDeliveryTimeoutEvent));
                return true;
            }));

        // Ignore initial event that can trigger entire state machine.
        // Ignore all push events.
        // Sms send + timeout are ignored, cause state machine is past that state.
        During(AwaitingSmsDelivery,
            Ignore(CascadingCommunicationRequestedEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(AwaitingSmsDelivery),
                    nameof(CascadingCommunicationRequestedEvent));
                return true;
            }),
            Ignore(PushSendEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(AwaitingSmsDelivery),
                    nameof(PushSendEvent));
                return true;
            }),
            Ignore(PushSendTimeoutEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(AwaitingSmsDelivery),
                    nameof(PushSendTimeoutEvent));
                return true;
            }),
            Ignore(PushDeliveryEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(AwaitingSmsDelivery),
                    nameof(PushDeliveryEvent));
                return true;
            }),
            Ignore(PushDeliveryTimeoutEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(AwaitingSmsDelivery),
                    nameof(PushDeliveryTimeoutEvent));
                return true;
            }),
            Ignore(SmsSendEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(AwaitingSmsDelivery),
                    nameof(SmsSendEvent));
                return true;
            }));

        // Ignore all, state machine has been finished by this time.
        During(Final,
            Ignore(CascadingCommunicationRequestedEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(Final),
                    nameof(CascadingCommunicationRequestedEvent));
                return true;
            }),
            Ignore(PushSendEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(Final),
                    nameof(PushSendEvent));
                return true;
            }),
            Ignore(PushSendTimeoutEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(Final),
                    nameof(PushSendTimeoutEvent));
                return true;
            }),
            Ignore(PushDeliveryEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(Final),
                    nameof(PushDeliveryEvent));
                return true;
            }),
            Ignore(PushDeliveryTimeoutEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(Final),
                    nameof(PushDeliveryTimeoutEvent));
                return true;
            }),
            Ignore(SmsSendEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(Final),
                    nameof(SmsSendEvent));
                return true;
            }),
            Ignore(SmsDeliveryEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(Final),
                    nameof(SmsDeliveryEvent));
                return true;
            }),
            Ignore(SmsDeliveryTimeoutEvent, filter =>
            {
                LogContext.Info?.Log(
                    "State Machine with {CommunicationId} during {State} received duplicate {EventName}.",
                    filter.Saga.CommunicationId, nameof(Final),
                    nameof(SmsDeliveryTimeoutEvent));
                return true;
            }));
    }

    private void CorrelateEvents()
    {
        Event(() => CascadingCommunicationRequestedEvent,
            x => x
                .CorrelateById(m => m.CommunicationId,
                    m => m.Message.CommunicationId)
                .SelectId(_ => NewId.NextGuid())
                .OnMissingInstance(m => m.Discard()));

        #region Push Events

        Event(() => PushSendEvent,
            x => x
                .CorrelateById(m => m.CommunicationId,
                    m => m.Message.PushId)
                .OnMissingInstance(m => m.Fault()));

        Event(() => PushDeliveryEvent,
            x => x
                .CorrelateById(m => m.CommunicationId,
                    m => m.Message.PushId)
                .OnMissingInstance(m => m.Fault()));

        Event(() => PushSendTimeoutEvent,
            x => x
                .CorrelateById(m => m.CommunicationId,
                    m => m.Message.CommunicationId)
                .OnMissingInstance(m => m.Fault()));

        Event(() => PushDeliveryTimeoutEvent,
            x => x
                .CorrelateById(m => m.CommunicationId,
                    m => m.Message.CommunicationId)
                .OnMissingInstance(m => m.Fault()));

        #endregion

        #region Sms Events

        Event(() => SmsSendEvent,
            x => x
                .CorrelateById(m => m.CommunicationId,
                    m => m.Message.SmsId)
                .OnMissingInstance(m => m.Discard()));

        Event(() => SmsDeliveryEvent,
            x => x
                .CorrelateById(m => m.CommunicationId,
                    m => m.Message.SmsId)
                .OnMissingInstance(m => m.Discard()));

        Event(() => SmsDeliveryTimeoutEvent,
            x => x
                .CorrelateById(m => m.CommunicationId,
                    m => m.Message.CommunicationId)
                .OnMissingInstance(m => m.Discard()));

        #endregion
    }
}