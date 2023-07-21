using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SplineScrubber.Timeline
{
    [Serializable]
    public class SplineClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private ExposedReference<SplineClipData> _splineData;
        [SerializeField] private SplineClipBehaviour _behaviour = new();

        public ClipCaps clipCaps => ClipCaps.ClipIn | ClipCaps.Looping | ClipCaps.Blending;

        public bool InitialClipDurationSet { get; set; }
        public float PathLength { get; set; }
        public ExposedReference<SplineClipData> SplineData => _splineData;
        public override double duration
        {
            get
            {
                return PathLength / _behaviour.Speed;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SplineClipBehaviour>.Create(graph, _behaviour);
            var clone = playable.GetBehaviour();
            var splineClipData = _splineData.Resolve(graph.GetResolver());
            if (splineClipData)
            {
                PathLength = splineClipData.Length;
                clone.SplineData = splineClipData;
            }
            return playable;
        }
    }
}