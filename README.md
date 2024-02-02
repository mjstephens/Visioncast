# Visioncast

Line of sight simulation package for Unity. **Vision sources** will collect **vision targets** within their sight range, then raycast to determine if the object is obstructed or visible. Use this data to simulate NPCs seeing important objects, security cameras, etc.

**WebGL demo** available here: https://mjstephens.github.io/VisioncastBuild/

![Screenshot 2024-02-02 at 10 07 54 AM](https://github.com/mjstephens/Visioncast/assets/4731148/df78fd9f-9168-4c5a-8d8e-cf4de3568471)


## How It Works

Visioncast operates in two general phases:

In the **broadphase**, we use _OvererlapSphereNonAlloc_ to gather all potential vision targets within the source's specified range. Vision targets can also be filtered by layer. Once all objects within range are gathered, we further filter them by checking their angle relative to the source's heading (usually the local forward direction, but this can be overridden). The result of the broadphase is a list of vision targets that are within range, on a valid layer, and within the field of view of the vision source.

We then send these results to the **narrowphase**, which performs raycasts to each of the potential targets to check for obstructions (an object that passes the broadphase may still be behind a wall, or a group of objects, and thus shouldn't be treated as "visible".) Obstruction layers can be defined per-source. We batch all vision sources and vision targets together, performing the raycasts asynchronously using _RaycastCommand_. The results are then redistributed to individual vision sources where they can be interpreted as needed.


![Screenshot 2024-02-02 at 10 09 35 AM](https://github.com/mjstephens/Visioncast/assets/4731148/41bb7cb0-d85b-497f-8cc1-bab3c09be73c)
_The orange box behind the wall has passed the broadphase, but failed the narrowphase. It is within the range and field of view of the source, but all visible points on the target are obstructed by the blue wall. Thus this object is considered "not visible"._



## Implementation Components

### VisioncastSource
The base vision component that exposes the object to the visioncast system. Create an override of this class, or use one of the included overrides. **VisioncastSourceSimple** provides a rudimentary implementation that still provides detailed results. **VisioncastSourceFiltered** is similar, but will automatically filter the results into more details, defining targets that are newly seen, newly lost, as well as the "key" target (ie the visible target most directly in front of the source).

![Screenshot 2024-02-02 at 10 06 37 AM](https://github.com/mjstephens/Visioncast/assets/4731148/c624d7b7-f30b-4488-a56e-39111e717d27)


### IVisioncastTargetable
The base interface that allows an object to be "seen" by the visioncast system. Implementations must define an array of _VisiblePoints_ on the object which will be raycasted against when determining whether or not the object is obstructed. Define your own implementation to manually set these points however you wish, or use/override the included **VisionTarget** class, which automatically creates visible points around the object's collider. It's important that whatever you use, your visible points are distributed such that if the object is "poking out" from behind a wall, it will still count as visible. (Of course you can define visible points however you need for your project's use case).

![Screenshot 2024-02-02 at 10 17 57 AM](https://github.com/mjstephens/Visioncast/assets/4731148/4a93882b-79f3-46ed-90dc-7f96afbe1a01)
_An example distribtion of visible points (red spheres) for an irregularly-shaped object. Visible points define the raycast targets for the visiocast narrowphase, so we want to make sure all major object edges are covered._


## Samples

Download samples by selecting the visioncast package in the package manager, then navigating to the "samples" tab.

### Core

This sample demonstrates basic implementations of all major components, including vision sources, vision targets, and realtime debug visualization of line of sight as well as narrowphase (raycast) results. 
