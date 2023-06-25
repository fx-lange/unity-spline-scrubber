using UnityEditor;
using UnityEditor.Timeline;
using SplineScrubber.Timeline;

namespace SplineScrubber.Editor
{
    [CustomEditor(typeof(SplineClip))]
    public class SplineClipInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var graph = TimelineEditor.inspectedDirector.playableGraph;
            var resolver = graph.GetResolver();

            var clip = target as SplineClip;
            var splineData = clip.SplineData.Resolve(resolver);
            if (splineData != null)
            {
                clip.PathLength = splineData.Length;
            }
            
            //TODO undo?
            //what if not changed? should be serialized?
        }
    }
}