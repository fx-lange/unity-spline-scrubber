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