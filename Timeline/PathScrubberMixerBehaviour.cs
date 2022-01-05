using PathScrubber.Path;
using UnityEngine.Playables;

namespace PathScrubber.Timeline
{
    public class PathScrubberMixerBehaviour : PlayableBehaviour
    {
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            PathingObjBase pathingObj = playerData as PathingObjBase;

            if (!pathingObj)
                return;

            int inputCount = playable.GetInputCount ();
            int activeInputCount = 0;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<PathScrubberBehaviour> inputPlayable = (ScriptPlayable<PathScrubberBehaviour>)playable.GetInput(i);
                PathScrubberBehaviour input = inputPlayable.GetBehaviour ();
                
                if (inputWeight <= 0f)
                {
                    continue;
                }

                activeInputCount++;
                pathingObj.Paused = true;
            
                var path = input.path;
                pathingObj.SetPath(path);

                var pos = inputPlayable.GetTime() * input.speed;
                var length = input.path.Length;
                pos %= length; //looping
                var tClip =  pos / input.path.Length;
                var tCurved = input.curve.Evaluate((float)tClip);
                if (input.backwards)
                {
                    tCurved = 1 - tCurved;
                }
                pathingObj.Set(tCurved, input.speed, input.offset, input.backwards);
            }

            if (activeInputCount == 0)
            {
                pathingObj.Paused = false;
            }
        }
    }
}
