using UnityEngine.Playables;
using SplineScrubber.Timeline.Clips;

namespace SplineScrubber.Timeline
{
    public class SplineMixerBehaviour : PlayableBehaviour
    {
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (playerData is not SplineCart cart)
                return;

            var inputCount = playable.GetInputCount();

            for (var i = 0; i < inputCount; i++)
            {
                var inputWeight = playable.GetInputWeight(i);
                var inputPlayable = (ScriptPlayable<BaseSplineBehaviour>)playable.GetInput(i);
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

                var tPos = input.EvaluateNormPos(inputPlayable.GetTime());
                cart.UpdatePosition(splineController, (float)tPos, splineController.Length);
            }
        }
    }
}