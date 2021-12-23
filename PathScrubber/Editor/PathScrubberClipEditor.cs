using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace PathScrubber.Editor
{
    [CustomTimelineEditor(typeof(PathScrubberClip))]
    public class PathScrubberClipEditor : ClipEditor
    {
        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            var asset = clip.asset as PathScrubberClip;
            clip.duration = asset.duration - clip.clipIn;
        }

        public override void OnClipChanged(TimelineClip clip)
        {
            // var asset = clip.asset as PathScrubberClip;
            // clip.duration = asset.duration - clip.clipIn;
        }

    }
}