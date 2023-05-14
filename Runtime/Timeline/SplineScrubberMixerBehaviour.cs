using UnityEngine.Playables;

namespace SplineScrubber.Timeline
{
    public class SplineScrubberMixerBehaviour : PlayableBehaviour
    {
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var cart = playerData as SplineCart;

            if (!cart)
                return;

            int inputCount = playable.GetInputCount ();
            int activeInputCount = 0;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<SplineScrubberBehaviour> inputPlayable = (ScriptPlayable<SplineScrubberBehaviour>)playable.GetInput(i);
                SplineScrubberBehaviour input = inputPlayable.GetBehaviour ();
                
                if (inputWeight <= 0f)
                {
                    continue;
                }

                activeInputCount++;
                cart.Paused = true;
            
                var path = input.path;
                cart.SetPath(path);

                var pos = inputPlayable.GetTime() * input.speed;
                var length = input.path.Length;
                pos %= length; //looping
                var tClip =  pos / input.path.Length;
                var tCurved = input.curve.Evaluate((float)tClip);
                if (input.backwards)
                {
                    tCurved = 1 - tCurved;
                }
                cart.Set(tCurved, input.speed, input.offset, input.backwards);
            }

            if (activeInputCount == 0)
            {
                cart.Paused = false;
            }
        }
    }
}
