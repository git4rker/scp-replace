using System;
using LabApi.Events.CustomHandlers;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;

namespace SCPReplace;

internal class SCPReplace : Plugin<Config> {
  public override string Name { get; } = "SCP Replace";
  public override string Description { get; } = "Simple plugin to replace SCPs that leave the game.";
  public override string Author { get; } = "git4rker";
  public override Version Version { get; } = new Version(1, 0, 0, 0);
  public override Version RequiredApiVersion { get; } = new Version(LabApiProperties.CompiledVersion);

  public static SCPReplace instance = new SCPReplace();
  public ReplaceEventsHandler Events { get; } = new();

  public static SCPReplace Instance {
    get {
      return instance;
    }
  }

  public override void Enable() {
    instance = this;
    CustomHandlersManager.RegisterEventsHandler(Events);
  }

  public override void Disable() {
    CustomHandlersManager.UnregisterEventsHandler(Events);
  }
}
