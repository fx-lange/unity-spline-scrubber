using UnityEngine;
using UnityEngine.Playables;

namespace SplineScrubber.Timeline
{
    public class SplineMixerBehaviour : PlayableBehaviour
    {
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var cart = playerData as SplineCart;

            // if (!cart)
            //     return;
            
            int inputCount = playable.GetInputCount ();

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<SplineClipBehaviour> inputPlayable = (ScriptPlayable<SplineClipBehaviour>)playable.GetInput(i);
                var input = inputPlayable.GetBehaviour ();
                
                if (inputWeight <= 0f)
                {
                    continue;
                }
            
                var splineData = input.SplineData;
                // cart.SetContainer(path.Spline); //TODO cache but how to blend?
                //
                var pos = inputPlayable.GetTime() * input.Speed;
                var length = splineData.Length;
                pos %= length; //looping
                var tClip =  pos / splineData.Length;
                Debug.Log($"Pos:{pos} T:{tClip} InputWeight:{inputWeight}");
            }
        }
    }
}
