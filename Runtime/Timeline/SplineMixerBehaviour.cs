using UnityEngine;
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
            
            var handler = SplinesMoveHandler.Instance;
            if (handler == null)
            {
                return;
            }
            
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
                var pos = inputPlayable.GetTime() * input.Speed;
                var length = splineData.Length;
                pos %= length; //looping
                var tClip =  pos / length;
                // Debug.Log($"{Time.frameCount} Pos:{pos} T:{tClip} InputWeight:{inputWeight}");
                handler.UpdatePos(cart.transform,(float)tClip,splineData);
            }
        }
    }
}
