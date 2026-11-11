# Rocket League Mobile — original car-soccer prototype

An independent Unity prototype for iPhone, inspired by the car-soccer genre. It is not affiliated with Psyonix or Epic Games and does not use Rocket League game files, branding, or assets.

## Included
- Starter driving and boost physics
- Follow camera
- Goal trigger and match score/timer logic
- Procedural arena with goals and walls
- Runtime car and ball prototype spawner
- Basic on-screen touch controls

## Try it in Unity
1. Create a new 3D Core project in Unity 6.
2. Copy this repository's Assets folder into the Unity project root.
3. Open an empty scene, create an empty GameObject named Game, and add PrototypeSpawner.
4. Press Play. Landscape orientation is recommended.
5. For iPhone builds, install Unity iOS Build Support and use Unity's iOS build workflow with Xcode on a Mac.

## Current status
This repository contains Unity C# prototype scripts and setup notes, not a complete Unity project or compiled IPA. The practice opponent is stationary; proper AI and multiplayer are not implemented. An iOS IPA also requires the Unity iOS toolchain, code signing, and provisioning.