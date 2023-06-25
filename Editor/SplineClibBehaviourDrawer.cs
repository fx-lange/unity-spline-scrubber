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
            int fieldCount = 1;
            return fieldCount * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty speedProp = property.FindPropertyRelative("_speed");
        

            Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(singleFieldRect, speedProp);
        }
    }
}
