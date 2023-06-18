using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace SplineScrubber
{
    public class SplineCart : MonoBehaviour
    {
        [SerializeField] private Transform offsetAnchor;

        private SplineContainer _splineContainer;
        private SplinePath<Spline> _splinePath;
        private NativeSpline _nativeSpline;

        //TODO extendable
        public void Set(float posNormalized, float speed, Vector3 offset, bool backwards = false)
        {
            // _splinePath.Evaluate(posNormalized, out float3 worldPos, out float3 tangent, out float3 upVector); //3.66
            // var worldPos = _splinePath.EvaluatePosition(posNormalized); //1.1ms
            // var tangent = _splinePath.EvaluateTangent(posNormalized);
            var (worldPos, tangent) = EvaluatePosAndRotation(posNormalized); //0.6ms
            // var worldPos = _nativeSpline.EvaluatePosition(posNormalized); //? ms
            // var tangent = _nativeSpline.EvaluateTangent(posNormalized);
            var upVector = Vector3.up;
            //TODO local to world
            
            if (backwards)
            {
                tangent = tangent * -1;
            }
            var rotation = Quaternion.LookRotation(tangent, upVector);
            transform.SetPositionAndRotation(worldPos, rotation);
            offsetAnchor.localPosition = offset;
        }

        private (float3, float3) EvaluatePosAndRotation(float t)
        {
            if (_splinePath.Count < 1)
                return (float.PositiveInfinity,float.PositiveInfinity);
            var curve = _splinePath.GetCurve(SplineUtility.SplineToCurveT(_splinePath, t, out var curveT));
            var tangent = CurveUtility.EvaluateTangent(curve, curveT);
            var pos = CurveUtility.EvaluatePosition(curve, curveT);
            return (pos, tangent);
        }

        public void SetContainer(SplineContainer splineContainer)
        {
            if (ReferenceEquals(_splineContainer, splineContainer))
            {
                return;
            } 
            
            _splineContainer = splineContainer;
            _splinePath = new SplinePath<Spline>(_splineContainer.Splines); //TODO happening at runtime on spline change
            // _nativeSpline = new NativeSpline(_splinePath, _splineContainer.transform.localToWorldMatrix, Allocator.Persistent);
        }
    }
}