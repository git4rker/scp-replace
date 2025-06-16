using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using System.Linq;
using System.Collections.Generic;
using LabApi.Features.Extensions;
using PlayerRoles;

namespace SCPReplace;

public class ReplaceEventsHandler : CustomEventsHandler {
  // The OnPlayerLeft event does not provide the player's role
  // before they left (only the Destroyed role which
  // is given to all disconnected players).
  // So here we have to track the roles ourselves...
  // DISGUSTING!!! northwood, fix pls
  //
  // P.S. we can't use the role change event either
  // as it's not fired when a player leaves

  private Dictionary<string, RoleTypeId> roles = new();
  private Queue<RoleTypeId> spawnQueue = new();

  // notify a player that they were chosen to
  // replace an SCP
  private void notifyPlayer(Player player) {
    player.SendHint(SCPReplace.Instance.Config.ReplaceMessage, SCPReplace.Instance.Config.ReplaceMessageDuration); 
  }

  public override void OnPlayerChangingRole(PlayerChangingRoleEventArgs ev) {
    // manually tracking player roles
    roles[ev.Player.UserId] = ev.NewRole;

    // if there's a new spectator
    // and the spawn queue isn't empty,
    // assign them a role on the queue
    if (ev.NewRole == RoleTypeId.Spectator
        && spawnQueue.Any()) {
      RoleTypeId role = spawnQueue.Dequeue();

      ev.NewRole = role;
      ev.Player.DisableAllEffects();
      notifyPlayer(ev.Player);
    }
  }

  public override void OnPlayerLeft(PlayerLeftEventArgs ev) {
    RoleTypeId role = roles[ev.Player.UserId];

    // only handle players who were SCPs before they left
    if (!role.IsScp()) return;

    var spectators = Player.ReadyList.Where(p => p.Role == RoleTypeId.Spectator);

    // if there are spectators, make a random one
    // an SCP
    if (spectators.Any()) {
      // pick the first spectator in the list
      // TODO (or not): pick a random spectator
      var chosenPlayer = spectators.First();

      chosenPlayer.SetRole(role);
      notifyPlayer(chosenPlayer);
    } else {
      // otherwise, put that SCP on the spawn queue
      // to be spawned when a new spectator appears
      spawnQueue.Enqueue(role);
    }
  }
}
