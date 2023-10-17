# Changelog

All notable changes to this package will be documented in this file. The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)

## UNRELEASED 

### Added

* Support for clip extrapolation
* Support for linear acceleration & deceleration per clip

### Changed

* Renaming `SplineClipData` -> `SplineJobController`
* Renaming `SplineMoveHandler` -> `SplineJobsScheduler`
* UX: auto link `SplineContainer` reference for `SplineJobController`

### Bug Fixes

* Fixed `SplineJobController` not updating when spline transform changes
* Fixed crash/exceptions on `SplineJobController` disable/enable during edit mode
* Fixed `SplineClipData::Length` for timeline starting on awake first frame

## [0.2.0] - 2023-07

### Added

### Changed