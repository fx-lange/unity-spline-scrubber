using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace SplineScrubber.Timeline
{
    [Serializable]
    public class SplineScrubberBehaviour : PlayableBehaviour
    {
        [FormerlySerializedAs("PathContainer")] [FormerlySerializedAs("_pathContainer")]
        public SplinePathContainer path;
    
        [Min(0.0001f)]public float speed = 1;
        public Vector3 offset = Vector3.zero;
        
        public bool backwards = false;
        
        public override void OnPlayableCreate (Playable playable)
        {
        
        }
    }
}
