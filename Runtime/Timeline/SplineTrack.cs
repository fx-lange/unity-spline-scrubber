using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using SplineScrubber.Timeline.Clips;

namespace SplineScrubber.Timeline
{
    [TrackClipType(typeof(SplineSpeedClip))]
    [TrackClipType(typeof(SplineTweenClip))]
    [TrackBindingType(typeof(SplineCart))]
    [TrackColor(0f, 0.4150943f, 0.8301887f)]
    [DisplayName("SplineScrubber/Track")]
    public partial class SplineTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<SplineMixerBehaviour>.Create(graph, inputCount);
        }
    }
}
