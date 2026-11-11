# Online multiplayer — Unity Netcode (no Photon)

The online prototype uses **Netcode for GameObjects + Unity Lobby + Unity Relay**. Players search for a public match inside the iPhone game; no website is needed.

## Install packages
In Unity Package Manager, install compatible versions of:
- Netcode for GameObjects
- Unity Transport
- Authentication
- Core
- Lobby
- Relay

Link the Unity project to a Unity Dashboard project and enable Lobby and Relay. Check Unity's current service limits and terms before release.

## Scene setup
1. Add a `NetworkManager` to the gameplay scene.
2. Add `UnityTransport` to that same object and assign it as NetworkManager's transport.
3. Add an `OnlineMatchmaking` object with `UnityNetcodeMatchmaking.cs`.
4. Add `OnlineMenuButton.cs` to a UI object, assign the matchmaking object, and connect the Online Play and Leave buttons.
5. Add a scene object with `NetworkObject` and `NetworkedMatchState.cs`.
6. Create a car Player Prefab with `NetworkObject`, `NetworkTransform`, `NetworkRigidbody`, `Rigidbody`, and `NetworkedCarController.cs`. Assign it to NetworkManager > Player Prefab. The NetworkManager will spawn one owned car per joining player.
7. Add `NetworkedPlayerSetup.cs` to the car prefab if you want to enable a local-only camera. Assign the camera and local-only behaviours.
8. Create the ball prefab with `NetworkObject`, `NetworkTransform`, `NetworkRigidbody`, `Rigidbody`, the Ball tag, and `NetworkedBall.cs`. Spawn it as a network object from the server or place it as a scene NetworkObject.
9. Add `NetworkedGoal.cs` to each goal trigger. Set scoringTeam to 0 for blue or 1 for orange, and assign the scene match state and ball.
10. Optional: add `NetworkedMatchHUD.cs` to a UI object and assign the score and timer Text labels.
11. Add the gameplay scene to Build Settings. Test with multiple devices.

## Mobile controls
For each touch button, attach `MobileHoldButton.cs`, choose its Control value, and assign the local player's `NetworkedCarController` to Network Target. The local camera and controls should only be enabled for the owning player.

## Important implementation notes
- The server simulates car and ball physics; NetworkTransform / NetworkRigidbody replicate the state.
- This is a starter implementation. Add a robust player spawn point/team assignment, reconnection, latency handling, anti-cheat checks, match cleanup and production UI before release.
- Verify that the installed Netcode package version supports the APIs used by the scripts. The code uses Unity 6 Rigidbody.linearVelocity.
- Unity Lobby/Relay requires Unity services setup and internet access. Usage limits and pricing can change.
- This repository still needs a complete Unity project (scenes, prefabs, ProjectSettings and package manifest) and an iOS build/signing process before it can produce an installable IPA.
