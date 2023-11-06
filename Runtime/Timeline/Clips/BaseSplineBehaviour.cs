using UnityEngine.Playables;

namespace SplineScrubber.Timeline.Clips
{
    public class BaseSplineBehaviour : PlayableBehaviour
    {
        public SplineJobController SplineController { get; set; }

        public virtual double EvaluateNormPos(double time)
        {
            return 0;
        }
    }
}