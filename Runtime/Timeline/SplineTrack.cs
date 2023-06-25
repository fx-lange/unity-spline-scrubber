using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SplineScrubber.Timeline
{
    [TrackClipType(typeof(SplineClip))]
    [DisplayName("SplineScrubber/Jobs Track")]
    [TrackBindingType(typeof(SplineCart))]
    [TrackColor(0f, 0.4150943f, 0.8301887f)]
    public class SplineTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<SplineMixerBehaviour>.Create (graph, inputCount);
        }
    }
}
