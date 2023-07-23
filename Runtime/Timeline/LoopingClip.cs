using UnityEngine.Timeline;

namespace SplineScrubber.Timeline
{
    public class LoopingClip : SplineClip, ITimelineClipAsset
    {
        public ClipCaps clipCaps => ClipCaps.ClipIn | ClipCaps.Looping; 
    }
}