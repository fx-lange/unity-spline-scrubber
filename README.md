# Spline Scrubber

Timeline integration for splines.

## What?

Want your objects to follow a spline, driven by timeline? 

Try **Spline Scrubber**:

* create clips from splines
* duraction based on spline length
* change speed, direction and local offset
* works in play and edit mode

## Work in progress

Unity is about to release a new spline tool (part of 2022.1). 

In the meantime Spline Scrubber offers abstract base classes to allow integrations with different spline solutions.
But this will be deprecated in favor of a better integration with `com.unity.splines.`

In production we are currently using cinemachine dolly tracks and you can find the implementation in the cinemachine branch. 
