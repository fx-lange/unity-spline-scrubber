using UnityEngine.Playables;

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
                ScriptPlayable<SplineClipBehaviour> inputPlayable = (ScriptPlayable<SplineClipBehaviour>)playable.GetInput(i);
                var input = inputPlayable.GetBehaviour ();
                
                if (inputWeight <= 0f)
                {
                    continue;
                }
            
                var splineData = input.SplineData;
                if (splineData == null)
                {
                    return;
                }

                var pos = input.EvaluateDistance(inputPlayable.GetTime());
                var length = splineData.Length;
                pos %= length; //looping
                var tClip =  pos / length;
                // Debug.Log($"{Time.frameCount} Pos:{pos} T:{tClip} InputWeight:{inputWeight}"); 
                splineData.JobHandler.HandlePosUpdate(cart.transform,(float)tClip);
            }
        }
    }
}
