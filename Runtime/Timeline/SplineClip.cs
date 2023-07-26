using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SplineScrubber.Timeline
{
    [Serializable]
    public abstract class SplineClip : PlayableAsset
    {
        [SerializeField] private ExposedReference<SplineClipData> _splineData;
        [SerializeField] private SplineClipBehaviour _behaviour = new();
        [HideInInspector][SerializeField] private TimelineClip _clip;

        public override double duration => GetDuration(Clip);

        public float PathLength { get; set; }
        public bool InitialClipDurationSet { get; set; }

        public TimelineClip Clip
        {
            private get { return _clip; }
            set { _clip = value; }
        }

        private float GetDuration(TimelineClip clip)
        {
            if (clip == null) return PathLength / _behaviour.Speed;
            
            var mixDuration = clip.mixInDuration + clip.mixOutDuration;
            if (mixDuration <= 0) return PathLength / _behaviour.Speed;
            
            //TODO simplified to be linear
            float mixInDistance = (float)clip.mixInDuration * _behaviour.Speed / 2f;
            float mixOutDistance = (float)clip.mixOutDuration * _behaviour.Speed / 2f;
            
            float centerDistance = PathLength - mixInDistance - mixOutDistance;
            return (float)mixDuration + (centerDistance / _behaviour.Speed);
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

            clone.Clip = Clip;
            return playable;
        }
    }
}