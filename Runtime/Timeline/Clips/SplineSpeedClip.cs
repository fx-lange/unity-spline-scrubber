using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SplineScrubber.Timeline.Clips
{
    [Serializable]
    public class SplineSpeedClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private ExposedReference<SplineJobController> _splineController;
        [SerializeField] private SplineSpeedBehaviour _behaviour = new();
        
        public ClipCaps clipCaps => ClipCaps.ClipIn | ClipCaps.Looping | ClipCaps.Extrapolation;

        public override double duration => GetDuration();

        public float PathLength { get; set; }
        public bool InitialClipDurationSet { get; set; }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SplineSpeedBehaviour>.Create(graph, _behaviour);
            var clone = playable.GetBehaviour();
            var splineController = _splineController.Resolve(graph.GetResolver());
            if (splineController)
            {
                PathLength = splineController.Length;
                clone.SplineController = splineController;
            }

            clone.Duration = GetDuration();
            return playable;
        }

        private float GetDuration()
        {
            var mixDuration = _behaviour.AccTime + _behaviour.DecTime;
            if (mixDuration <= 0) return PathLength / _behaviour.Speed;

            //TODO simplified to be linear
            var mixInDistance = _behaviour.AccTime * _behaviour.Speed / 2f;
            var mixOutDistance = _behaviour.DecTime * _behaviour.Speed / 2f;

            var centerDistance = PathLength - mixInDistance - mixOutDistance;
            return mixDuration + (centerDistance / _behaviour.Speed);
        }

        private void OnValidate()
        {
            // Debug.Log("OnValidate");
            _behaviour.UpdateAccDecDistance();
        }
    }
}