using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (((ShowIfAttribute)attribute).CheckShowIf(property))
            EditorGUI.PropertyField(position, property, label);

        else
            property.DeleteCommand();

        EditorGUI.EndProperty();
    }
}