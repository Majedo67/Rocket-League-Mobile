# Online multiplayer setup

This prototype uses **Photon Fusion 2** for online sessions. Players connect from inside the iPhone game; they do not need to open a website.

## Important
These are integration starter files, not a complete multiplayer implementation. The current car controller and ball are not network-synchronised yet. Do not publish this as a finished online game.

## Unity setup
1. Open the Unity project in the Unity Editor.
2. Add Photon Fusion 2 using the official Photon Fusion installation instructions.
3. Create a Photon account and a Fusion application, then configure its App ID in the Fusion project settings.
4. Create a NetworkRunner prefab with a NetworkRunner component and a NetworkSceneManagerDefault component.
5. Add an empty GameObject named `OnlineMatch` to the gameplay scene and attach `OnlineMatchBootstrap.cs`.
6. Assign the NetworkRunner prefab to the `Runner Prefab` field.
7. Add an Online Play button and a Leave Match button. Attach `OnlineMenuButton.cs` to a UI object and connect each button's OnClick event to the matching method.
8. Add the gameplay scene to Build Settings.

## Next networking work
- Create a networked car prefab with a NetworkObject.
- Spawn one car for each connected player.
- Synchronise car movement, boost, ball physics, goals, score and match timer.
- Add a proper public matchmaking flow, reconnect handling and server-authoritative scoring.
- Test on multiple devices before release.

Photon's free tier, service limits, terms and current pricing can change. Check Photon before release.
