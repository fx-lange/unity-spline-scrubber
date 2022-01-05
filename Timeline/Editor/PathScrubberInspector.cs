using UnityEditor;
using UnityEditor.Timeline;

namespace PathScrubber.Timeline.Editor
{
    [CustomEditor(typeof(PathScrubberClip))]
    public class PathScrubberInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var graph = TimelineEditor.inspectedDirector.playableGraph;
            var resolver = graph.GetResolver();

            var clip = target as PathScrubberClip;
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