using SplineScrubber.Timeline;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace SplineScrubber.Editor
{
    [CustomTimelineEditor(typeof(SplineClip))]
    public class SplineClipEditor : ClipEditor
    {
        private bool _durationIsInit;

        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            base.OnCreate(clip, track, clonedFrom);
            _durationIsInit = clonedFrom != null;
        }

        //currently needed due to PathLength being updated after OnCreate
        public override void OnClipChanged(TimelineClip clip)
        {
            base.OnClipChanged(clip);
            
            if (_durationIsInit) return;
        
            var asset = clip.asset as SplineClip;
            if (!Mathf.Approximately(asset.PathLength, 0))
            {
                clip.duration = asset.duration - clip.clipIn;
                _durationIsInit = true;
            }
        }
    }
}