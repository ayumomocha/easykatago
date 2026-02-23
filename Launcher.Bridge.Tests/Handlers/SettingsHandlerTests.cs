using System.Text.Json;
using Launcher.Bridge.Contracts;
using Launcher.Bridge.Dispatch;

namespace Launcher.Bridge.Tests.Handlers;

public sealed class SettingsHandlerTests
{
    [Fact]
    public async Task DispatchAsync_SettingsRead_ReturnsInstallRoot()
    {
        using var scope = TestDispatcherFactory.WithSampleSettings("""{"installRoot":"."}""");

        var response = await scope.Dispatcher.DispatchAsync(new BridgeRequest("settings.read", null));

        Assert.True(response.Success);
        Assert.Contains("installRoot", response.Data?.ToString());
    }

    [Fact]
    public async Task DispatchAsync_SettingsWrite_PersistsPayload()
    {
        using var scope = TestDispatcherFactory.WithSampleSettings("""{"installRoot":"."}""");
        var payload = JsonSerializer.Deserialize<object>("""{"installRoot":"C:\\KataGo"}""");

        var response = await scope.Dispatcher.DispatchAsync(new BridgeRequest("settings.write", payload));

        Assert.True(response.Success);

        using var json = JsonDocument.Parse(await File.ReadAllTextAsync(scope.SettingsPath));
        Assert.Equal(@"C:\KataGo", json.RootElement.GetProperty("installRoot").GetString());
    }
}

internal static class TestDispatcherFactory
{
    public static TestDispatcherScope WithSampleSettings(string settingsJson)
    {
        var root = CreateScopeRoot();
        var settingsPath = Path.Combine(root, "settings.json");
        var profilesPath = Path.Combine(root, "profiles.json");

        File.WriteAllText(settingsPath, settingsJson);
        File.WriteAllText(profilesPath, """{"defaultProfileId":"p1","profiles":[]}""");

        return new TestDispatcherScope(
            new CommandDispatcher(settingsPath, profilesPath),
            root,
            settingsPath,
            profilesPath);
    }

    public static TestDispatcherScope WithSampleProfiles(string profilesJson)
    {
        var root = CreateScopeRoot();
        var settingsPath = Path.Combine(root, "settings.json");
        var profilesPath = Path.Combine(root, "profiles.json");

        File.WriteAllText(settingsPath, """{"installRoot":"."}""");
        File.WriteAllText(profilesPath, profilesJson);

        return new TestDispatcherScope(
            new CommandDispatcher(settingsPath, profilesPath),
            root,
            settingsPath,
            profilesPath);
    }

    private static string CreateScopeRoot()
    {
        var root = Path.Combine(Path.GetTempPath(), $"launcher-bridge-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        return root;
    }
}

internal sealed class TestDispatcherScope(
    CommandDispatcher dispatcher,
    string root,
    string settingsPath,
    string profilesPath) : IDisposable
{
    public CommandDispatcher Dispatcher { get; } = dispatcher;
    public string SettingsPath { get; } = settingsPath;
    public string ProfilesPath { get; } = profilesPath;

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
            // Best effort cleanup for temporary files.
        }
        catch (UnauthorizedAccessException)
        {
            // Best effort cleanup for temporary files.
        }
    }
}
