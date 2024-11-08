using UnityEngine;

namespace SplineScrubber
{
    public class SplineCart : MonoBehaviour
    {
        public virtual void UpdatePosition(SplineJobController controller, float t, float length)
        {
            controller.HandlePosUpdate(transform, t);
        }
    }
}