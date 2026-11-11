# Rocket League Mobile

An independent car-soccer prototype for iPhone using original code and assets.

## Unity project
- Unity Editor: 6000.0.42f1
- Main scene: Assets/Scenes/Main.unity
- Runtime prototype: procedural arena, two cars, ball, touch controls and follow camera
- Online starter: Unity Netcode for GameObjects + Unity Lobby + Unity Relay (no Photon)

Open this repository folder with Unity Hub. Unity will resolve packages from Packages/manifest.json.

## iOS build
GitHub Actions workflow: .github/workflows/build.yml

Run it from **Actions → iOS IPA Build → Run workflow**. Add these repository secrets before running:
- UNITY_LICENSE
- UNITY_EMAIL
- UNITY_PASSWORD

The workflow exports an iOS Xcode project. An installable IPA additionally needs a valid Apple signing certificate and provisioning profile for the app's bundle ID. This repository does not contain Apple credentials.

## Online multiplayer status
Online scripts are starter code and still need scene/prefab NetworkObject configuration, Unity Gaming Services project setup, and device testing. The local practice prototype is separate from fully working online matchmaking.

This is an independent project and is not affiliated with Psyonix or Epic Games.
