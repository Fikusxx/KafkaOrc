using System.Net.Mime;
using CloudNative.CloudEvents.SystemTextJson;

namespace Orchestrator.StateMachine.Core.Cloud;

internal abstract class BaseCloudEventJsonSerializer<T> where T : class
{
    private readonly ContentType _contentType = new(MediaTypeNames.Application.Json);
    private readonly JsonEventFormatter<T> _formatter = new();

    protected object? BaseDeserialize(ReadOnlySpan<byte> data)
    {
        var wrapper = _formatter.DecodeStructuredModeMessage(new MemoryStream(data.ToArray()), _contentType, null);
        return wrapper.Data;
    }
}