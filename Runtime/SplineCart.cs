using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace SplineScrubber
{
    public class SplineCart : MonoBehaviour
    {
        [SerializeField] private SplineContainer spline;
        [SerializeField] private Transform offsetAnchor;

        private SplinePath path;
        
        public bool Paused 
        { 
            get => true;
            set => spline.enabled = value; //?
        }
        
        public void Set(float posNormalized, float speed, Vector3 offset, bool backwards = false)
        {
            //TODO extendable
            spline.Evaluate(posNormalized, out float3 worldPos, out float3 tangent, out float3 upVector);

            if (backwards)
            {
                tangent = tangent * -1;
            }
            var rotation = Quaternion.LookRotation(tangent, upVector);
            transform.SetPositionAndRotation(worldPos, rotation);
            offsetAnchor.localPosition = offset;
        }

        public void SetPath(SplinePath newPath)
        {
            if (!ReferenceEquals(path, newPath))
            {
                path = newPath;
                spline = path.Spline;
            }
        }
    }
}