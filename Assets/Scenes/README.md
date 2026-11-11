# Unity scene setup

Create a 3D Unity project, then create a scene named `MainArena` in this folder.

Quick prototype setup:
1. Add a large Plane for the arena and surround it with walls.
2. Add a car GameObject with a Rigidbody and Collider; attach `CarController`.
3. Add a Camera and attach `FollowCamera`; assign the car as its target.
4. Add a Sphere with a Rigidbody and Collider, then tag it `Ball`.
5. Add trigger BoxColliders behind each goal and attach `GoalTrigger`.
6. Add an EventSystem and Canvas. Create five UI buttons (accelerate, reverse, left, right, boost), attach `MobileHoldButton` to each, set its Control value, and assign the car controller.

This repository currently contains starter scripts, not a compiled Unity project or IPA.
