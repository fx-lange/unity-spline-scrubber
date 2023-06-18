using SplineScrubber.Timeline;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace SplineScrubber.Editor
{
    [CustomTimelineEditor(typeof(SplineScrubberClip))]
    public class SplineScrubberClipEditor : ClipEditor
    {
        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            Debug.Log("ClipEditor::Create");
            var asset = clip.asset as SplineScrubberClip;
            clip.duration = asset.duration - clip.clipIn;
        }

        public override void OnClipChanged(TimelineClip clip)
        {
            // var asset = clip.asset as PathScrubberClip;
            // clip.duration = asset.duration - clip.clipIn;
        }

    }
}