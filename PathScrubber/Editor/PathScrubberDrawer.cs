using UnityEditor;
using UnityEngine;

namespace PathScrubber.Editor
{
    [CustomPropertyDrawer(typeof(PathScrubberBehaviour))]
    public class PathScrubberDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
        {
            int fieldCount = 4;
            return fieldCount * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty speedProp = property.FindPropertyRelative("speed");
            SerializedProperty backwardsProp = property.FindPropertyRelative("backwards");
            SerializedProperty curveProp = property.FindPropertyRelative("curve");
            SerializedProperty offsetProp = property.FindPropertyRelative("offset");
        

            Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(singleFieldRect, speedProp);

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, offsetProp);
            
            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, backwardsProp);
        
            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, curveProp);
        }
    }
}
