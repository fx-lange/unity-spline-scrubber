# Spline Scrubber

**Spline Scrubber** is a Unity plugin that integrates Unity Spline package with the Unity Timeline to choreograph spline-driven animations.
It offers custom timeline tracks, enabling precise animation sequencing along splines.

By leveraging the performance benefits of the C# job system and burst compiler, the plugin efficiently evaluates splines and updates transforms in a multithreaded manner, at runtime and during edit mode.

## Dependencies

* Unity 2022.3
* com.unity.splines
* com.unity.timeline
* com.unity.collections

## Installation

Install via Package Manager
* [Add package via git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html):
* `https://github.com/fx-lange/unity-spline-scrubber.git`

## How to use

To choreograph spline-driven animations with Spline Scrubber:

* `SplineCart`: Attach this to the game objects you wish to animate. It serves as the track binding for the SplineTrack.
* `SplineTrack`: For each object, add this custom timeline track to your timeline assets.
* `SplineClip`: Use this custom timeline clip within the SplineTrack to dictate the object's movement along the spline for the clip's duration.
* `SplineJobController`: For every Spline(Container), include this component to pre-process and cache spline data, optimizing job execution.
* `SplineJobsScheduler`: Ensure one instance of this singleton exists in your scene. It manages the real-time evaluation of splines and updates object transforms each frame.