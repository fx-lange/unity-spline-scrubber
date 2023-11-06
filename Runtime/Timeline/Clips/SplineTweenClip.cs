using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SplineScrubber.Timeline.Clips
{
    public class SplineTweenClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private ExposedReference<SplineJobController> _splineController;
        [SerializeField] private SplineTweenBehaviour _behaviour = new();

        public ClipCaps clipCaps => ClipCaps.ClipIn | ClipCaps.Extrapolation;

        public double Duration { 
            set => _behaviour.Duration = value;
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SplineTweenBehaviour>.Create(graph, _behaviour);
            var clone = playable.GetBehaviour();
            var splineController = _splineController.Resolve(graph.GetResolver());
            if (splineController)
            {
                clone.SplineController = splineController;
            }

            return playable;
        }
    }
}