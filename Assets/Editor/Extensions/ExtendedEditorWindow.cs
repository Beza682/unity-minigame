using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ExtendedEditorWindow : EditorWindow
{
    protected SerializedObject SerializedObject;
    protected SerializedProperty CurrentProperty;

    private string _selectedPropertyPath;
    protected SerializedProperty SelectedProperty;
    protected bool DrawMenuAndLevel;

    protected void DrawProperties(SerializedProperty property, bool drawChildren)
    {
        string lastPropPath = string.Empty;

        foreach (SerializedProperty prop in property)
        {
            if (!prop.isArray && prop.propertyType == SerializedPropertyType.Generic)
            {
                EditorGUILayout.BeginHorizontal();
                prop.isExpanded = EditorGUILayout.Foldout(prop.isExpanded, prop.displayName);
                EditorGUILayout.EndHorizontal();

                if (prop.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    DrawProperties(prop, drawChildren);
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(lastPropPath) && prop.propertyPath.Contains(lastPropPath)) { continue; }
                lastPropPath = prop.propertyPath;
                EditorGUILayout.PropertyField(prop, drawChildren);
            }
        }
    }

    protected void DrawTitle(SerializedProperty property)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(property.displayName);
        if (GUILayout.Button("+", GUILayout.MaxWidth(40)))
        {
            if (property.arraySize < 4)
                property.arraySize++;
        }
        if (GUILayout.Button("-", GUILayout.MaxWidth(40)))
        {
            if (property.arraySize > 0)
                property.arraySize--;
        }
        EditorGUILayout.EndHorizontal();
    }

    protected void DrawSidebar(SerializedProperty property)
    {
        for (int i = 0; i < property.arraySize; i++)
        {
            SerializedProperty prop = property.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginHorizontal();
            if (property.type == nameof(StageInfo))
            {
                var enable = prop.FindPropertyRelative("Enable");
                enable.boolValue = EditorGUILayout.Toggle(enable.boolValue, GUILayout.MaxWidth(16));
                GUI.enabled = enable.boolValue;

                if (_selectedPropertyPath == prop.propertyPath)
                {
                    DrawMenuAndLevel = enable.boolValue;
                }
            }

            if (GUILayout.Button(prop.displayName))
            {
                _selectedPropertyPath = prop.propertyPath;
                DrawMenuAndLevel = true;
            }
            GUI.enabled = true;

            if (GUILayout.Button("X", GUILayout.MaxWidth(40)))
            {
                property.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add"))
        {
            int idx = property.arraySize++;
            var newElement = property.GetArrayElementAtIndex(idx);

            if (property.type == nameof(StageInfo))
            {
                newElement.FindPropertyRelative("Enable").boolValue = true;
                newElement.FindPropertyRelative("Name").stringValue = $"NewElement{property.arraySize}";
                newElement.FindPropertyRelative("PendulumSpeed").floatValue = 3;
                newElement.FindPropertyRelative("LifeCount").intValue = 1;
                newElement.FindPropertyRelative("CirclesList").ClearArray();
            }

            _selectedPropertyPath = property.GetArrayElementAtIndex(idx).propertyPath;
        }

        if (!string.IsNullOrEmpty(_selectedPropertyPath))
        {
            SelectedProperty = SerializedObject.FindProperty(_selectedPropertyPath);
        }
    }

    protected void DrawField(string propName, bool relative)
    {
        if (relative && CurrentProperty != null)
        {
            EditorGUILayout.PropertyField(CurrentProperty.FindPropertyRelative(propName), true);
        }
        else if (SerializedObject != null)
        {
            EditorGUILayout.PropertyField(SerializedObject.FindProperty(propName), true);
        }
    }

    protected void DrawIntSlider(string propName, int leftValue, int rightValue, bool relative)
    {
        if (relative && CurrentProperty != null)
        {
            CurrentProperty.FindPropertyRelative(propName).intValue = EditorGUILayout.IntSlider(CurrentProperty.FindPropertyRelative(propName).displayName, CurrentProperty.FindPropertyRelative(propName).intValue, leftValue, rightValue);
        }
        else if (SerializedObject != null)
        {
            SerializedObject.FindProperty(propName).intValue = EditorGUILayout.IntSlider(SerializedObject.FindProperty(propName).displayName, SerializedObject.FindProperty(propName).intValue, leftValue, rightValue);
        }
    }

    protected void DrawFloatSlider(string propName, float leftValue, float rightValue, bool relative)
    {
        if (relative && CurrentProperty != null)
        {
            CurrentProperty.FindPropertyRelative(propName).floatValue = EditorGUILayout.Slider(CurrentProperty.FindPropertyRelative(propName).displayName, CurrentProperty.FindPropertyRelative(propName).floatValue, leftValue, rightValue);
        }
        else if (SerializedObject != null)
        {
            SerializedObject.FindProperty(propName).floatValue = EditorGUILayout.Slider(SerializedObject.FindProperty(propName).displayName, SerializedObject.FindProperty(propName).floatValue, leftValue, rightValue);
        }
    }

    protected void Apply()
    {
        SerializedObject?.ApplyModifiedProperties();
    }
}
