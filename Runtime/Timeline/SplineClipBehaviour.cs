using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SplineScrubber.Timeline
{
    [Serializable]
    public class SplineClipBehaviour : PlayableBehaviour
    {
        [SerializeField] private SplineClipData _splineData;
        [SerializeField] [NotKeyable]
        [Min(0.0001f)]private float _speed = 1;
        
        public SplineEvaluateHandler JobHandler => _splineData.JobHandler;
        public SplineClipData SplineData { get; set; }
        public float Speed => _speed;
        
        public override void OnPlayableCreate (Playable playable)
        {
        
        }
    }
}