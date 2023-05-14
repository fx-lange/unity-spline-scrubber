using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SplineScrubber.Timeline
{
    [Serializable]
    public class SplineScrubberClip : PlayableAsset, ITimelineClipAsset
    {
        public ExposedReference<SplinePath> path;
        public SplineScrubberBehaviour template = new SplineScrubberBehaviour();

        public ClipCaps clipCaps => ClipCaps.ClipIn | ClipCaps.Looping;

        [HideInInspector] public float PathLength;

        public override double duration
        {
            get
            {
                var length = PathLength;
                return Mathf.Max(length / template.speed, 1f);
            }
        }

        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SplineScrubberBehaviour>.Create (graph, template);
            SplineScrubberBehaviour clone = playable.GetBehaviour ();
            clone.path = path.Resolve (graph.GetResolver ());
            return playable;
        }
    }
}
