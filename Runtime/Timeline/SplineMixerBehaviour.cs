using UnityEngine.Playables;
using SplineScrubber.Timeline.Clips;

namespace SplineScrubber.Timeline
{
    public class SplineMixerBehaviour : PlayableBehaviour
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
                ScriptPlayable<BaseSplineBehaviour> inputPlayable = (ScriptPlayable<BaseSplineBehaviour>)playable.GetInput(i);
                var input = inputPlayable.GetBehaviour ();
                
                if (inputWeight <= 0f)
                {
                    continue;
                }
            
                var splineController = input.SplineController;
                if (splineController == null)
                {
                    return;
                }

                var pos = input.EvaluateNormPos(inputPlayable.GetTime());
                splineController.HandlePosUpdate(cart.transform,(float)pos);
            }
        }
    }
}
