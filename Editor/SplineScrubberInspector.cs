using SplineScrubber.Timeline;
using UnityEditor;
using UnityEditor.Timeline;

namespace SplineScrubber.Editor
{
    [CustomEditor(typeof(SplineScrubberClip))]
    public class SplineScrubberInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Debug.Log("SplineScrubberInspector::OnGui");
            base.OnInspectorGUI();
            
            var graph = TimelineEditor.inspectedDirector.playableGraph;
            var resolver = graph.GetResolver();

            var clip = target as SplineScrubberClip;
            var path = clip.path.Resolve(resolver);
            if (path != null)
            {
                clip.PathLength = path.Length;
            }
            
            //TODO undo?
            //what if not changed? should be serialized?
        }
    }
}