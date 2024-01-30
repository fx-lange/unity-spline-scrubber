using UnityEngine;

namespace SplineScrubber
{
    public class SplineCart : MonoBehaviour
    {
        public virtual void UpdatePosition(SplineJobController controller, float pos, float length)
        {
            controller.HandlePosUpdate(transform, pos / length);
        }
    }
}