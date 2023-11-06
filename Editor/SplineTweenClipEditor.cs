using UnityEditor.Timeline;
using UnityEngine.Timeline;
using SplineScrubber.Timeline.Clips;

namespace SplineScrubber.Editor
{
    [CustomTimelineEditor(typeof(SplineTweenClip))]
    public class SplineTweenClipEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            base.OnClipChanged(clip);
            var asset = clip.asset as SplineTweenClip;

            asset.Duration = clip.duration + clip.clipIn;
        }
    }
}