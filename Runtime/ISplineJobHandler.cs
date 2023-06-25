using UnityEngine;

namespace SplineScrubber
{
    public interface ISplineJobHandler
    {
        public void UpdatePos(Transform target, float tPos, SplineClipData splineData);
    }
}