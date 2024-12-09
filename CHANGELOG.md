# Changelog

All notable changes to this package will be documented in this file. The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)

## [0.3.4] - 2024-11

### Added

* New TweenClip additional to the speed based clip
* Support for linear acceleration & deceleration per speed based clips
* Support for clip extrapolation

### Changed

* Renaming `SplineClipData` -> `SplineJobController`
* Renaming `SplineMoveHandler` -> `SplineJobsScheduler`
* UX: auto link `SplineContainer` reference for `SplineJobController`
* Moved evaluation call from mixer in to `SplineCart` for better extendability
* Use `Unity.mathematics.quaternion` for better burst
* Calculate rotation from tan&up in `SplineEvaluate` Job
* Spline localToWorld now handled in `SplineEvaluate` not via `NativeSpline` construction

### Bug Fixes

* Fixed `SplineJobController` not updating when spline transform changes
* Fixed crash/exceptions on `SplineJobController` disable/enable during edit mode
* Fixed `SplineClipData::Length` for timeline starting on awake first frame
* Fixed life-cycle issue when disabling/deleting `SplineJobController`

## [0.2.0] - 2023-07

### Added

### Changed