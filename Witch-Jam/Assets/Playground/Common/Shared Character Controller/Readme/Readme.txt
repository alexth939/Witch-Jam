Showcase of a Good Player Experience – Character Behavior

Note 1:
The code is written in a single script.
It is not structured or refactored.
Do not use it in production projects.

Note 2:
The original Melee_Attack animation had issues.
I fixed it (in the clone), see the reference for details.

Note 3:
Some animations had position keys!
They are automatically discarded by the Unity editor.
However, you should fix the animations manually by removing position keys,
because we do not use translation DOF here.

Note 4:
To test only the character in your scene, just drag and drop the Witch_Character_Prefab.
If you want to test the character with a camera, you can either:

Create your own camera, or

Copy the Cinemachine Brain, Camera, and Witch_Character_Prefab
objects from the example scene.