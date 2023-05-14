using System;
using UnityEngine;
using UnityEngine.Playables;

namespace SplineScrubber.Timeline
{
    [Serializable]
    public class SplineScrubberBehaviour : PlayableBehaviour
    {
        public SplinePath path;
    
        [Min(0.0001f)]public float speed = 1;
        public Vector3 offset = Vector3.zero;
        
        public bool backwards = false;
        public AnimationCurve curve = AnimationCurve.Linear(0,0,1,1);

        public override void OnPlayableCreate (Playable playable)
        {
        
        }
    }
}
