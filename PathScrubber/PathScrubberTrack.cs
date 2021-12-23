using System.ComponentModel;
using Path;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PathScrubber
{
    [TrackClipType(typeof(PathScrubberClip))]
    [DisplayName("PathScrubber Track")]
    [TrackBindingType(typeof(PathingObjBase))]
    [TrackColor(0f, 0.4150943f, 0.8301887f)]
    public class PathScrubberTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<PathScrubberMixerBehaviour>.Create (graph, inputCount);
        }
    }
}
