using Launcher.Bridge.Dispatch;
using Launcher.Bridge.Hosting;

var dispatcher = new CommandDispatcher();
var host = new BridgeHost(dispatcher);

await host.RunAsync(Console.In, Console.Out);
