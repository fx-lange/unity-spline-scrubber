using Path;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Spline
{
    public class SplineCart : PathingObjBase//MonoBehaviour, IPathMove
    {
        [SerializeField] private SplineContainer spline;
        [SerializeField] private Transform offsetAnchor;

        private SplinePath path;
        
        public override bool Paused 
        { 
            get => true;
            set => spline.enabled = value;
        }
        
        public override void SetPosition(float pos, Vector3 offset, bool backwards = false)
        {
            spline.Evaluate(pos, out float3 worldPos, out float3 tangent, out float3 upVector);

            if (backwards)
            {
                tangent = tangent * -1;
            }
            var rotation = Quaternion.LookRotation(tangent, upVector);
            transform.SetPositionAndRotation(worldPos, rotation);
            offsetAnchor.localPosition = offset;
        }

        public override void SetPath(IPath newPath)
        {
            if (!ReferenceEquals(path, newPath))
            {
                path = newPath as SplinePath;
                spline = path.Spline;
            }
        }
    }
}