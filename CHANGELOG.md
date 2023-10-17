# Changelog

All notable changes to this package will be documented in this file. The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)

## UNRELEASED [0.3.2] 

### Added

* support for clip extrapolation
* support for linear acceleration & deceleration per clip

### Changed

* Renaming `SplineClipData` -> `SplineJobController`
* Renaming `SplineMoveHandler` -> `SplineJobsScheduler`
* UX: auto link `SplineContainer` reference for `SplineJobController`

### Fixed

* `SplineJobController` not updating when spline transform changes
* crash/exceptions on `SplineJobController` disable/enable during edit mode
* `SplineClipData::Length` for timeline starting on awake first frame

## [0.2.0] - 2023-07

### Added

### Changed