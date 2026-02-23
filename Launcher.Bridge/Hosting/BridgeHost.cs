using System.Text.Json;
using Launcher.Bridge.Contracts;
using Launcher.Bridge.Dispatch;

namespace Launcher.Bridge.Hosting;

public sealed class BridgeHost(CommandDispatcher dispatcher)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task RunAsync(
        TextReader input,
        TextWriter output,
        CancellationToken cancellationToken = default)
    {
        var requestJson = await input.ReadToEndAsync(cancellationToken);
        BridgeResponse response;

        try
        {
            var request = JsonSerializer.Deserialize<BridgeRequest>(requestJson, JsonOptions);
            if (request is null || string.IsNullOrWhiteSpace(request.Command))
            {
                response = BridgeResponse.Fail("INVALID_REQUEST", "Missing command in bridge request.");
            }
            else
            {
                response = await dispatcher.DispatchAsync(request, cancellationToken);
            }
        }
        catch (JsonException ex)
        {
            response = BridgeResponse.Fail("INVALID_REQUEST", $"Bridge request JSON is invalid: {ex.Message}");
        }

        var responseJson = JsonSerializer.Serialize(response, JsonOptions);
        await output.WriteAsync(responseJson);
        await output.FlushAsync(cancellationToken);
    }
}
