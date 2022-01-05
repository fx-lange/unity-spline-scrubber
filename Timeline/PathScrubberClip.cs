using System;
using PathScrubber.Path;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PathScrubber.Timeline
{
    [Serializable]
    public class PathScrubberClip : PlayableAsset, ITimelineClipAsset
    {
        public ExposedReference<PathBase> path;
        public PathScrubberBehaviour template = new PathScrubberBehaviour();

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
            var playable = ScriptPlayable<PathScrubberBehaviour>.Create (graph, template);
            PathScrubberBehaviour clone = playable.GetBehaviour ();
            clone.path = path.Resolve (graph.GetResolver ());
            return playable;
        }
    }
}
