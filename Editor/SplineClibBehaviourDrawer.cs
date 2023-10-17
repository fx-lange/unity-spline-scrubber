using UnityEditor;
using UnityEngine;
using SplineScrubber.Timeline;

namespace SplineScrubber.Editor
{
    [CustomPropertyDrawer(typeof(SplineClipBehaviour))]
    public class SplineClibBehaviourDrawer : PropertyDrawer
    {
        private bool _showAccFoldOut = true;
        private bool _showDecFoldOut = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var speedProp = property.FindPropertyRelative("_speed");
            var noRotProp = property.FindPropertyRelative("_ignoreRotation");

            EditorGUILayout.PropertyField(speedProp); 
            EditorGUILayout.PropertyField(noRotProp);
            FoldOut("_accTime", "_accDistance", "_accCurve", ref _showAccFoldOut, "Acceleration");
            FoldOut("_decTime", "_decDistance", "_decCurve", ref _showDecFoldOut, "Deceleration");

            void FoldOut(string time, string distance,
                string curve, ref bool active, string label)
            {
                active = EditorGUILayout.Foldout(active, label);
                if (!active) return;

                var timeProp = property.FindPropertyRelative(time);
                var curveProp = property.FindPropertyRelative(curve);
                var distanceProp = property.FindPropertyRelative(distance);
                
                EditorGUI.indentLevel += 1;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(timeProp);
                EditorGUILayout.LabelField($"~{distanceProp.floatValue:F2}m");
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(curveProp);
                EditorGUI.indentLevel -= 1;
            }
        }
    }
}