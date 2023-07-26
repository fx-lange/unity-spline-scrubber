using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SplineScrubber.Timeline
{
    [Serializable]
    public class SplineClipBehaviour : PlayableBehaviour
    {
        [SerializeField] private SplineClipData _splineData;
        [SerializeField] [NotKeyable] [Min(0.0001f)]
        private float _speed = 1;

        public SplineEvaluateHandler JobHandler => _splineData.JobHandler;
        public SplineClipData SplineData { get; set; }
        public TimelineClip Clip { private get; set; }
        public float Speed => _speed;

        public double EvaluateDistance(double time)
        {
            var mixInDur = Clip.mixInDuration;
            var mixOutDur = Clip.mixOutDuration;
            var mixOutStart = Clip.duration - mixOutDur;

            double mixInDistance = 0;
            if (mixInDur > 0)
            {
                var mixInTime = math.min(time, mixInDur);
                var mixInSpeed = mixInTime * Speed / mixInDur; 
                mixInDistance = mixInTime * mixInSpeed / 2f;
            }

            double mixOutDistance = 0;
            if (mixOutDur > 0)
            {
                var mixOutTime = mixOutDur - math.clamp(time - mixOutStart, 0, mixOutDur);
                var mixOutSpeed = mixOutTime * Speed / mixOutDur;
                mixOutDistance = mixOutDur * Speed/2f - mixOutTime * mixOutSpeed/2f;
            }

            var centerDistance = math.clamp(time - mixInDur, 0, mixOutStart - mixInDur) * Speed;
            return mixInDistance + centerDistance + mixOutDistance;
        }

        public override void OnPlayableCreate(Playable playable)
        {
        }
    }
}