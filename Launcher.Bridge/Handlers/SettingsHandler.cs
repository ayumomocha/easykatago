using System.Text.Json;
using Launcher.Bridge.Contracts;

namespace Launcher.Bridge.Handlers;

public sealed class SettingsHandler(string settingsPath)
{
    public async Task<BridgeResponse> ReadAsync(CancellationToken cancellationToken = default)
    {
        var json = await LoadRawAsync(cancellationToken);
        return BridgeResponse.Ok(ParseJson(json));
    }

    public async Task<BridgeResponse> WriteAsync(object? payload, CancellationToken cancellationToken = default)
    {
        if (payload is null)
        {
            return BridgeResponse.Fail("INVALID_PAYLOAD", "settings.write requires payload");
        }

        var json = SerializePayload(payload);
        Directory.CreateDirectory(Path.GetDirectoryName(settingsPath) ?? ".");
        await File.WriteAllTextAsync(settingsPath, json, cancellationToken);
        return BridgeResponse.Ok(ParseJson(json));
    }

    public async Task<string> LoadRawAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(settingsPath))
        {
            return "{}";
        }

        return await File.ReadAllTextAsync(settingsPath, cancellationToken);
    }

    private static object ParseJson(string json)
    {
        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }

    private static string SerializePayload(object payload)
    {
        return payload is JsonElement element
            ? element.GetRawText()
            : JsonSerializer.Serialize(payload);
    }
}
