using Leopotam.Ecs;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Voody.UniLeo;

public class ViewECSComponents : EditorWindow
{
    private Vector2 _scroll;
    private List<ComponentLog> _components = new List<ComponentLog>();

    private class ComponentLog
    {
        public string Name;
        public List<string> Values = new List<string>();
    }

    [MenuItem("CustomTools/LeoECS/Show ECS components")]
    public static void ShowWindow()
    {
        GetWindow<ViewECSComponents>("ECS components view");
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal("box");
        _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        byte number = 0;
        _components.ForEach(c =>
        {
            number++;
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"{number}. {c.Name}");

            c.Values.ForEach(v =>
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"* {v}");
                EditorGUILayout.EndHorizontal();
            });
            EditorGUILayout.EndVertical();
        });
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Show Components"))
        {
            _components.Clear();
            foreach (GameObject gameObject in Selection.gameObjects)
            {
                if (gameObject.TryGetComponent<ConvertToEntity>(out var cto))
                {
                    if (cto.TryGetEntity().HasValue)
                    {
                        DrawInfo(cto.TryGetEntity().Value, nameof(ConvertToEntity));
                    }
                }
                if (gameObject.TryGetComponent<EntityReference>(out var er))
                {
                    DrawInfo(er.Entity, nameof(EntityReference));
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawInfo(in EcsEntity entity, in string source)
    {
        object[] componentsList = null;
        if (entity.IsAlive())
        {
            entity.GetComponentValues(ref componentsList);
        }

        ComponentLog[] temp = new ComponentLog[componentsList.Length];

        for (int i = 0; i < componentsList.Length; i++)
        {
            var component = componentsList[i];
            temp[i] = new ComponentLog() { Name = $"{component}  -  {source} (Source)"};

            foreach (var fieldInfo in component.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
            {
                string modificator = string.Empty;

                if (fieldInfo.IsPublic)
                    modificator = "public";
                else if (fieldInfo.IsAssembly)
                    modificator = "internal";

                temp[i].Values.Add($"{modificator}  {fieldInfo.FieldType.Name}  {fieldInfo.Name}  =  {fieldInfo.GetValue(component) ?? "NULL"}");
            }
        }
        _components.AddRange(temp);
    }
}