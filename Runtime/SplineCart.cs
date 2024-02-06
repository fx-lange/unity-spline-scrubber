using UnityEngine;

namespace SplineScrubber
{
    public class SplineCart : MonoBehaviour
    {
        public virtual void UpdatePosition(SplineJobController controller, float tPos, float length)
        {
            controller.HandlePosUpdate(transform, tPos);
        }
    }
}