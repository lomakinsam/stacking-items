using UnityEditor;
using UnityEngine;

namespace Stacking.EditorExtensions
{
    [CustomPropertyDrawer(typeof(Attributes.DisabledFieldAttribute))]
    public class DisabledPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    EditorGUI.IntField(position, label, property.intValue);
                    break;
                case SerializedPropertyType.Boolean:
                    EditorGUI.Toggle(position, label, property.boolValue);
                    break;
                case SerializedPropertyType.Float:
                    EditorGUI.FloatField(position, label, property.floatValue);
                    break;
                case SerializedPropertyType.String:
                    EditorGUI.TextField(position, label, property.stringValue);
                    break;
                case SerializedPropertyType.Color:
                    EditorGUI.ColorField(position, label, property.colorValue);
                    break;
                case SerializedPropertyType.Vector2:
                    EditorGUI.Vector2Field(position, label, property.vector2Value);
                    break;
                case SerializedPropertyType.Vector3:
                    EditorGUI.Vector3Field(position, label, property.vector3Value);
                    break;
                case SerializedPropertyType.Vector4:
                    EditorGUI.Vector4Field(position, label, property.vector4Value);
                    break;
                case SerializedPropertyType.Vector2Int:
                    EditorGUI.Vector2IntField(position, label, property.vector2IntValue);
                    break;
                case SerializedPropertyType.Vector3Int:
                    EditorGUI.Vector3IntField(position, label, property.vector3IntValue);
                    break;
                default:
                    Debug.LogWarning($"{property.propertyType} doesn't support disabling!");
                    break;
            }
            GUI.enabled = true;
        }
    }
}