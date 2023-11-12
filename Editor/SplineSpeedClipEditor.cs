using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;
using SplineScrubber.Timeline.Clips;

namespace SplineScrubber.Editor
{
    [CustomTimelineEditor(typeof(SplineSpeedClip))]
    public class SplineSpeedClipEditor : ClipEditor
    {
        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            var asset = clip.asset as SplineSpeedClip;
            base.OnCreate(clip, track, clonedFrom);
            asset.InitialClipDurationSet = clonedFrom != null;
        }

        public override void OnClipChanged(TimelineClip clip)
        {
            base.OnClipChanged(clip);
            var asset = clip.asset as SplineSpeedClip;
            
            if (asset.InitialClipDurationSet) return;
        
            if (!Mathf.Approximately(asset.PathLength, 0))
            {
                clip.duration = asset.duration - clip.clipIn;
                asset.InitialClipDurationSet = true;
            }
        }
    }
}