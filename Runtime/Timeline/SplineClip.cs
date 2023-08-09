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
        [HideInInspector] [SerializeField] private TimelineClip _clip;

        public ClipCaps clipCaps => ClipCaps.ClipIn | ClipCaps.Looping | ClipCaps.Extrapolation;

        public override double duration => GetDuration();

        public float PathLength { get; set; }
        public bool InitialClipDurationSet { get; set; }

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

            clone.Duration = GetDuration();
            return playable;
        }

        private float GetDuration()
        {
            var mixDuration = _behaviour.AccTime + _behaviour.DecTime;
            if (mixDuration <= 0) return PathLength / _behaviour.Speed;

            //TODO simplified to be linear
            float mixInDistance = _behaviour.AccTime * _behaviour.Speed / 2f;
            float mixOutDistance = _behaviour.DecTime * _behaviour.Speed / 2f;

            float centerDistance = PathLength - mixInDistance - mixOutDistance;
            return mixDuration + (centerDistance / _behaviour.Speed);
        }
    }
}