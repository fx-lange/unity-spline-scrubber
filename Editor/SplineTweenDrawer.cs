using UnityEngine;
using UnityEditor;
using SplineScrubber.Timeline.Clips;

[CustomPropertyDrawer(typeof(SplineTweenBehaviour))]
public class SplineTweenDrawer : PropertyDrawer
{
    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        int fieldCount = property.FindPropertyRelative ("_tweenType").enumValueIndex == (int)SplineTweenBehaviour.TweenType.Custom ? 2 : 1;
        return fieldCount * (EditorGUIUtility.singleLineHeight);
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty tweenTypeProp = property.FindPropertyRelative ("_tweenType");
        
        Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, tweenTypeProp);

        if (tweenTypeProp.enumValueIndex == (int)SplineTweenBehaviour.TweenType.Custom)
        {
            SerializedProperty customCurveProp = property.FindPropertyRelative ("_customCurve");
            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField (singleFieldRect, customCurveProp);
        }
    }
}
