using SplineScrubber.Timeline;
using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace SplineScrubber.Editor
{
    [CustomTimelineEditor(typeof(SplineClip))]
    public class SplineClipEditor : ClipEditor
    {
        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            var asset = clip.asset as SplineClip;
            clip.duration = asset.duration - clip.clipIn;
        }

        public override void OnClipChanged(TimelineClip clip)
        {
            // var asset = clip.asset as PathScrubberClip;
            // clip.duration = asset.duration - clip.clipIn;
        }

    }
}