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

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<SplineScrubberBehaviour> inputPlayable = (ScriptPlayable<SplineScrubberBehaviour>)playable.GetInput(i);
                SplineScrubberBehaviour input = inputPlayable.GetBehaviour ();
                
                if (inputWeight <= 0f)
                {
                    continue;
                }

                var path = input.path;
                cart.SetContainer(path.Spline); //TODO cache but how to blend?

                var pos = inputPlayable.GetTime() * input.speed;
                var length = input.path.Length;
                pos %= length; //looping
                var tClip =  pos / input.path.Length;
                if (input.backwards)
                {
                    tClip = 1 - tClip;
                }
                cart.Set((float)tClip, input.speed, input.offset, input.backwards);
            }
        }
    }
}
