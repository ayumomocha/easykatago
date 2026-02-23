using Launcher.Bridge.Contracts;
using Launcher.Bridge.Handlers;

namespace Launcher.Bridge.Dispatch;

public sealed class CommandDispatcher
{
    private readonly SettingsHandler _settingsHandler;
    private readonly ProfilesHandler _profilesHandler;

    public CommandDispatcher(string? settingsPath = null, string? profilesPath = null)
    {
        _settingsHandler = new SettingsHandler(
            settingsPath ?? Path.Combine(AppContext.BaseDirectory, "data", "settings.json"));
        _profilesHandler = new ProfilesHandler(
            profilesPath ?? Path.Combine(AppContext.BaseDirectory, "data", "profiles.json"));
    }

    public Task<BridgeResponse> DispatchAsync(BridgeRequest request, CancellationToken cancellationToken = default)
    {
        return request.Command switch
        {
            "settings.read" => _settingsHandler.ReadAsync(cancellationToken),
            "settings.write" => _settingsHandler.WriteAsync(request.Payload, cancellationToken),
            "profiles.read" => _profilesHandler.ReadAsync(cancellationToken),
            "profiles.write" => _profilesHandler.WriteAsync(request.Payload, cancellationToken),
            _ => Task.FromResult(BridgeResponse.Fail("UNKNOWN_COMMAND", $"Unsupported command: {request.Command}"))
        };
    }
}
