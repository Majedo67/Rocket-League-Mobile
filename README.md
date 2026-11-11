# Rocket League Mobile

An independent car-football prototype for iPhone, built with original code and assets.

## Current prototype code
- 3D car driving and boost controller
- Touchscreen driving controls
- Follow camera
- Goal trigger and local match timer/score
- Unity Netcode for GameObjects online car controller
- Unity Lobby + Relay public matchmaking starter
- Networked ball, goals, replicated score and timer starter
- Online match HUD

## Online multiplayer
The project uses **Unity Netcode for GameObjects + Unity Lobby + Unity Relay**. It does not use Photon. Players will be able to find public matches from inside the game without opening a website once the Unity scene, prefabs, services and networking setup are completed.

See [Online multiplayer setup](Assets/Scripts/Online/README.md).

## Important project status
This repository currently contains prototype scripts and setup notes, not a complete Unity project or a finished game. Unity scenes, prefabs, project settings, package configuration, testing, and the iOS build/signing process still need to be completed before an installable IPA can be produced.

This is an independent project and is not affiliated with or endorsed by Psyonix or Epic Games. It will use original code and assets.
