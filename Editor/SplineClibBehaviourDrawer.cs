using SplineScrubber.Timeline;
using UnityEditor;
using UnityEngine;

namespace SplineScrubber.Editor
{
    [CustomPropertyDrawer(typeof(SplineClipBehaviour))]
    public class SplineClibBehaviourDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
        {
            int fieldCount = 3;
            return fieldCount * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty speedProp = property.FindPropertyRelative("_speed");
            SerializedProperty accTimeProp = property.FindPropertyRelative("_accTime");
            SerializedProperty decTimeProp = property.FindPropertyRelative("_decTime");

            Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(singleFieldRect, speedProp);
            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, accTimeProp);
            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, decTimeProp);
        }
    }
}
