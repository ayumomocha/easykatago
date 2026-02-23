using System.Text.Json;
using Launcher.Bridge.Dispatch;
using Launcher.Bridge.Hosting;

namespace Launcher.Bridge.Tests.Hosting;

public sealed class BridgeHostTests
{
    [Fact]
    public async Task RunAsync_SettingsRead_WritesSuccessEnvelope()
    {
        using var scope = CreateDispatcherScope(
            """{"installRoot":"."}""",
            """{"defaultProfileId":null,"profiles":[]}""");
        var host = new BridgeHost(scope.Dispatcher);
        using var input = new StringReader("""{"command":"settings.read","payload":null}""");
        using var output = new StringWriter();

        await host.RunAsync(input, output);

        using var json = JsonDocument.Parse(output.ToString());
        Assert.True(json.RootElement.GetProperty("success").GetBoolean());
        Assert.Equal(".", json.RootElement.GetProperty("data").GetProperty("installRoot").GetString());
    }

    [Fact]
    public async Task RunAsync_InvalidJson_WritesInvalidRequestEnvelope()
    {
        using var scope = CreateDispatcherScope(
            """{"installRoot":"."}""",
            """{"defaultProfileId":null,"profiles":[]}""");
        var host = new BridgeHost(scope.Dispatcher);
        using var input = new StringReader("{bad-json");
        using var output = new StringWriter();

        await host.RunAsync(input, output);

        using var json = JsonDocument.Parse(output.ToString());
        Assert.False(json.RootElement.GetProperty("success").GetBoolean());
        Assert.Equal(
            "INVALID_REQUEST",
            json.RootElement.GetProperty("error").GetProperty("code").GetString());
    }

    private static DispatcherScope CreateDispatcherScope(string settingsJson, string profilesJson)
    {
        var root = Path.Combine(Path.GetTempPath(), $"launcher-bridge-host-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);

        var settingsPath = Path.Combine(root, "settings.json");
        var profilesPath = Path.Combine(root, "profiles.json");
        File.WriteAllText(settingsPath, settingsJson);
        File.WriteAllText(profilesPath, profilesJson);

        return new DispatcherScope(root, new CommandDispatcher(settingsPath, profilesPath));
    }

    private sealed class DispatcherScope(string root, CommandDispatcher dispatcher) : IDisposable
    {
        public CommandDispatcher Dispatcher { get; } = dispatcher;

        public void Dispose()
        {
            if (!Directory.Exists(root))
            {
                return;
            }

            try
            {
                Directory.Delete(root, true);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }
}
