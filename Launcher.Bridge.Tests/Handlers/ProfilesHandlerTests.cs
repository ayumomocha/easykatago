using System.Text.Json;
using Launcher.Bridge.Contracts;

namespace Launcher.Bridge.Tests.Handlers;

public sealed class ProfilesHandlerTests
{
    [Fact]
    public async Task DispatchAsync_ProfilesRead_ReturnsDefaultProfileId()
    {
        using var scope = TestDispatcherFactory.WithSampleProfiles(
            """{"defaultProfileId":"default","profiles":[{"profileId":"default"}]}""");

        var response = await scope.Dispatcher.DispatchAsync(new BridgeRequest("profiles.read", null));

        Assert.True(response.Success);
        Assert.Contains("defaultProfileId", response.Data?.ToString());
    }

    [Fact]
    public async Task DispatchAsync_ProfilesWrite_PersistsPayload()
    {
        using var scope = TestDispatcherFactory.WithSampleProfiles("""{"defaultProfileId":null,"profiles":[]}""");
        var payload = JsonSerializer.Deserialize<object>(
            """{"defaultProfileId":"new-default","profiles":[{"profileId":"new-default"}]}""");

        var response = await scope.Dispatcher.DispatchAsync(new BridgeRequest("profiles.write", payload));

        Assert.True(response.Success);

        using var json = JsonDocument.Parse(await File.ReadAllTextAsync(scope.ProfilesPath));
        Assert.Equal("new-default", json.RootElement.GetProperty("defaultProfileId").GetString());
    }
}
