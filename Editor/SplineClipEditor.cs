using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;
using SplineScrubber.Timeline;

namespace SplineScrubber.Editor
{
    [CustomTimelineEditor(typeof(SplineClip))]
    public class SplineClipEditor : ClipEditor
    {
        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            base.OnCreate(clip, track, clonedFrom);
            var asset = clip.asset as SplineClip;
            asset.InitialClipDurationSet = clonedFrom != null;
        }

        public override void OnClipChanged(TimelineClip clip)
        {
            base.OnClipChanged(clip);
            var asset = clip.asset as SplineClip;
            
            if (asset.InitialClipDurationSet) return;
        
            if (!Mathf.Approximately(asset.PathLength, 0))
            {
                clip.duration = asset.duration - clip.clipIn;
                asset.InitialClipDurationSet = true;
            }
        }
    }
}