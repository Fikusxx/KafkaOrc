# Cascading Communication Orchestrator

Service is designed to orchestrate cascading communication that consists of SMS and Push messages.

## Usage

You need to configure the application with your:

- Scheduler configuration
- StateMachineDatabaseOptions configuration
- StateMachineOptions configuration
- Health Check configuration
- Logging configuration

### appSettings.json

- Scheduler options

```json
{
  "SchedulerOptions": {
    "SchedulerId": "AUTO",
    "SchedulerName": "name",
    "MaxBatchSize": 123,
    "BatchTriggerAcquisitionFireAheadTimeWindowMs": 100,
    "MaxConcurrency": 10,
    "Database": {
      "ConnectionString": "Host=localhost;Port=5432;Database=Sms;Username=postgres;Search Path=skk;Password=postgres",
      "TablePrefix": "skk.qrtz_",
      "Schema": "skk",
      "RetryIntervalMs": 100
    }
  }
}
```

- StateMachineDatabaseOptions options

```json
{
  "StateMachineDatabaseOptions": {
    "Connection": "Host=localhost;Port=5432;Database=Sms;Username=postgres;Password=postgres;Search Path=skk",
    "Schema": "skk",
    "MaxRetryCount": 3,
    "MaxRetryDelayMs": 10
  }
}
```

- StateMachineOptions options

```json
{
  "StateMachineOptions" : {
    "RetryCount" : 3,
    "RetryIntervalMs": 100,
    "ClientConfig": {
      "BootstrapServers": "localhost:9092"
    },
    "SendPushCommandProducerOptions": {
      "Topic": "push",
      "ProducerConfig": {
        "BootstrapServers": "localhost:9092",
        "Acks" : -1,
        "ClientId": "123",
        "RetryBackoffMs": 100,
        "RetryBackoffMaxMs": 1000,
        "MessageSendMaxRetries": 100,
        "MaxInFlight": 5,
        "LingerMs": 50,
        "QueueBufferingMaxMessages": 100000,
        "QueueBufferingMaxKbytes": 1048576,
        "MessageMaxBytes": 1000000,
        "BatchSize": 1000000,
        "BatchNumMessages": 10000,
        "RequestTimeoutMs": 30000,
        "SocketTimeoutMs": 60000,
        "MessageTimeoutMs": 0,
        "SocketConnectionSetupTimeoutMs": 30000,
        "ApiVersionRequestTimeoutMs": 10000
      }
    },
    "SendSmsCommandProducerOptions": {
      "Topic": "sms",
      "ProducerConfig": {
        "BootstrapServers": "localhost:9092"
      }
    },
    "PushSendTimeoutProducerOptions": {
      "Topic": "push.send.timeout",
      "ProducerConfig": {
        "BootstrapServers": "localhost:9092"
      }
    },
    "PushDeliveryTimeoutProducerOptions": {
      "Topic": "push.delivery.timeout",
      "ProducerConfig": {
        "BootstrapServers": "localhost:9092"
      }
    },
    "SmsDeliveryTimeoutProducerOptions": {
      "Topic": "sms.delivery.timeout",
      "ProducerConfig": {
        "BootstrapServers": "localhost:9092"
      }
    },
    "CascadingCommunicationCompletedProducerOptions": {
      "Topic": "communication.completed",
      "ProducerConfig": {
        "BootstrapServers": "localhost:9092"
      }
    },
    "CascadingCommunicationRequestedConsumerOptions": {
      "Topic": "communication.requested",
      "ConsumerConfig": {
        "BootstrapServers": "localhost:9092",
        "AutoOffsetReset": 1,
        "EnableAutoOffsetStore": false,
        "EnableAutoCommit": false,
        "ClientId": "123",
        "GroupId": "communication.requested",
        "QueuedMinMessages": 100000,
        "QueuedMaxMessagesKbytes": 65536,
        "FetchErrorBackoffMs": 500,
        "FetchQueueBackoffMs": 1000,
        "MessageMaxBytes": 1000000,
        "MaxPartitionFetchBytes": 1048576,
        "FetchMinBytes": 1,
        "FetchMaxBytes": 52428800,
        "FetchWaitMaxMs": 500,
        "RetryBackoffMs": 100,
        "RetryBackoffMaxMs": 1000,
        "HeartbeatIntervalMs": 3000,
        "SessionTimeoutMs": 45000,
        "SocketTimeoutMs": 60000,
        "SocketConnectionSetupTimeoutMs": 30000,
        "ApiVersionRequestTimeoutMs": 10000
      },
      "HighLevelConsumerOptions": {
        "PrefetchCount": 500,
        "ConcurrentMessageLimit": 10,
        "ConcurrentConsumerLimit": 1,
        "ConcurrentDeliveryLimit": 1,
        "CheckpointMessageCount": 1,
        "CheckpointIntervalMs": 5000,
      }
    },
    "PushSendConsumerOptions": {
      "Topic": "push.send",
      "ConsumerConfig": {
        "BootstrapServers": "localhost:9092",
        "AutoOffsetReset": 1,
        "EnableAutoOffsetStore": false,
        "EnableAutoCommit": false,
        "GroupId" : "push.send"
      },
      "HighLevelConsumerOptions": {
        "PrefetchCount": 500,
        "ConcurrentMessageLimit": 10,
        "ConcurrentConsumerLimit": 1,
        "ConcurrentDeliveryLimit": 1,
        "CheckpointMessageCount": 1,
        "CheckpointIntervalMs": 5000,
      }
    },
    "PushDeliveryConsumerOptions": {
      "Topic": "push.delivery",
      "ConsumerConfig": {
        "BootstrapServers": "localhost:9092",
        "AutoOffsetReset": 1,
        "EnableAutoOffsetStore": false,
        "EnableAutoCommit": false,
        "GroupId" : "push.delivery"
      },
      "HighLevelConsumerOptions": {
        "PrefetchCount": 500,
        "ConcurrentMessageLimit": 10,
        "ConcurrentConsumerLimit": 1,
        "ConcurrentDeliveryLimit": 1,
        "CheckpointMessageCount": 1,
        "CheckpointIntervalMs": 5000,
      }
    },
    "PushSendTimeoutConsumerOptions": {
      "Topic": "push.send.timeout",
      "ConsumerConfig": {
        "BootstrapServers": "localhost:9092",
        "AutoOffsetReset": 1,
        "EnableAutoOffsetStore": false,
        "EnableAutoCommit": false,
        "GroupId" : "push.send.timeout"
      },
      "HighLevelConsumerOptions": {
        "PrefetchCount": 500,
        "ConcurrentMessageLimit": 10,
        "ConcurrentConsumerLimit": 1,
        "ConcurrentDeliveryLimit": 1,
        "CheckpointMessageCount": 1,
        "CheckpointIntervalMs": 5000,
      }
    },
    "PushDeliveryTimeoutConsumerOptions": {
      "Topic": "push.delivery.timeout",
      "ConsumerConfig": {
        "BootstrapServers": "localhost:9092",
        "AutoOffsetReset": 1,
        "EnableAutoOffsetStore": false,
        "EnableAutoCommit": false,
        "GroupId" : "push.delivery.timeout"
      },
      "HighLevelConsumerOptions": {
        "PrefetchCount": 500,
        "ConcurrentMessageLimit": 10,
        "ConcurrentConsumerLimit": 1,
        "ConcurrentDeliveryLimit": 1,
        "CheckpointMessageCount": 1,
        "CheckpointIntervalMs": 5000,
      }
    },
    "SmsSendConsumerOptions": {
      "Topic": "sms.send",
      "ConsumerConfig": {
        "BootstrapServers": "localhost:9092",
        "AutoOffsetReset": 1,
        "EnableAutoOffsetStore": false,
        "EnableAutoCommit": false,
        "GroupId" : "sms.send"
      },
      "HighLevelConsumerOptions": {
        "PrefetchCount": 500,
        "ConcurrentMessageLimit": 10,
        "ConcurrentConsumerLimit": 1,
        "ConcurrentDeliveryLimit": 1,
        "CheckpointMessageCount": 1,
        "CheckpointIntervalMs": 5000,
      }
    },
    "SmsDeliveryConsumerOptions": {
      "Topic": "sms.delivery",
      "ConsumerConfig": {
        "BootstrapServers": "localhost:9092",
        "AutoOffsetReset": 1,
        "EnableAutoOffsetStore": false,
        "EnableAutoCommit": false,
        "GroupId" : "sms.delivery"
      },
      "HighLevelConsumerOptions": {
        "PrefetchCount": 500,
        "ConcurrentMessageLimit": 10,
        "ConcurrentConsumerLimit": 1,
        "ConcurrentDeliveryLimit": 1,
        "CheckpointMessageCount": 1,
        "CheckpointIntervalMs": 5000,
      }
    },
    "SmsDeliveryTimeoutConsumerOptions": {
      "Topic": "sms.delivery.timeout",
      "ConsumerConfig": {
        "BootstrapServers": "localhost:9092",
        "AutoOffsetReset": 1,
        "EnableAutoOffsetStore": false,
        "EnableAutoCommit": false,
        "GroupId" : "sms.delivery.timeout"
      },
      "HighLevelConsumerOptions": {
        "PrefetchCount": 500,
        "ConcurrentMessageLimit": 10,
        "ConcurrentConsumerLimit": 1,
        "ConcurrentDeliveryLimit": 1,
        "CheckpointMessageCount": 1,
        "CheckpointIntervalMs": 5000,
      }
    }
  }
}
```

- Health Check options

```json
{
  "HealthCheckOptions" : {
    "KafkaOptions": {
      "TimeoutMs": 2000,
      "ProducerConfig": {
        "BootstrapServers": "localhost:9092"
      }
    }
  }
}
```

- Logging options

```json
{
  "Serilog": {
    "Using": [
      "Serilog.Sinks.Console",
      "Serilog.Sinks.File"
    ],
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "Logs/log.txt",
          "rollingInterval": "Minute",
          "fileSizeLimitBytes": 10000,
          "rollOnFileSizeLimit": true,
          // "formatter": "Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact"
          "formatter": "Serilog.Formatting.Json.JsonFormatter"
        }
      }
    ],
    "Enrich": [
      "FromLogContext",
      "WithThreadId",
      "WithProcessId",
      "WithMachineName",
      "WithEnvironmentName"
    ],
    "Properties": {
      "Application": "Cascading Communication Orchestrator API"
    }
  }
}
```

## Support

If you encounter any bugs or issues, please report them to...

## Acknowledgment

This get-started file gives a high-level overview of the Cascading Communication Orchestrator service.
For more detailed documentation, please refer to the code comments
and the associated documentation in jira/....