using UnityEngine.Timeline;

namespace SplineScrubber.Timeline
{
    public class SpeedBlendingClip : SplineClip, ITimelineClipAsset
    {
        public ClipCaps clipCaps => ClipCaps.ClipIn | ClipCaps.Blending;
    }
}