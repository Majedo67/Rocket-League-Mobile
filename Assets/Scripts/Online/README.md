# Online multiplayer (no Photon)

This prototype uses **Unity Netcode for GameObjects + Unity Lobby + Unity Relay**. Players can search for a public lobby from inside the iPhone game; no website needs to be opened.

## Required Unity packages
Install compatible versions of:
- Netcode for GameObjects
- Unity Transport
- Authentication
- Core
- Lobby
- Relay

Enable these services for the Unity project in the Unity Dashboard. Unity services may require a Unity project linked to an account and may have usage limits or pricing; check current terms before release.

## Scene setup
1. Add a **NetworkManager** GameObject to the gameplay scene.
2. Add **UnityTransport** to that same GameObject and assign it as the NetworkManager's network transport.
3. Add an empty GameObject named `OnlineMatchmaking` and attach `UnityNetcodeMatchmaking.cs`.
4. Add a UI object with `OnlineMenuButton.cs` and assign the matchmaking object.
5. Connect the Online Play button to `OnFindMatchPressed` and the Leave button to `OnLeaveMatchPressed`.
6. Add the gameplay scene to Build Settings.

The matchmaking script searches public lobbies with free slots. If it finds one, it joins through its Relay join code; otherwise it creates a public lobby and hosts a Relay session.

## Still required for real multiplayer
- Add NetworkObject and NetworkTransform (or equivalent networked movement) to the car prefab.
- Spawn a car for each connected player.
- Synchronise ball physics, boost, goals, score and match timer.
- Add ownership checks, disconnect handling, server-authoritative scoring and abuse protection.
- Test on multiple devices and networks.

**Status:** integration starter only. This does not yet make the existing car and ball gameplay network-synchronised, and it is not an IPA or finished online game.
