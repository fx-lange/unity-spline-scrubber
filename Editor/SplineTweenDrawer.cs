using UnityEngine;
using UnityEditor;
using SplineScrubber.Timeline.Clips;

[CustomPropertyDrawer(typeof(SplineTweenBehaviour))]
public class SplineTweenDrawer : PropertyDrawer
{
    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        int fieldCount = property.FindPropertyRelative ("_tweenType").enumValueIndex == (int)SplineTweenBehaviour.TweenType.Custom ? 4 : 3;
        return fieldCount * (EditorGUIUtility.singleLineHeight);
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        
        SerializedProperty fromProp = property.FindPropertyRelative ("_from");
        SerializedProperty toProp = property.FindPropertyRelative ("_to");
        SerializedProperty tweenTypeProp = property.FindPropertyRelative ("_tweenType");
        
        Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, fromProp);
        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, toProp);
        
        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, tweenTypeProp);

        if (tweenTypeProp.enumValueIndex == (int)SplineTweenBehaviour.TweenType.Custom)
        {
            var customCurveProp = property.FindPropertyRelative ("_customCurve");
            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField (singleFieldRect, customCurveProp);
        }
    }
}
