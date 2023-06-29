using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class MiniGameHandler
{
    [OnOpenAsset()]
    public static bool OpenWindow(int id)
    {
        if (EditorUtility.InstanceIDToObject(id) is MiniGamePoolObject obj)
        {
            MiniGameLevelWindow.Open(obj);
            return true;
        }

        return false;
    }
}

[CustomEditor(typeof(MiniGamePoolObject))]
public class MiniGamePoolEditorGUI : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        MiniGamePoolObject editor = (MiniGamePoolObject)target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_pendulumAmplitude"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_stageCount"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_requestBundle"));

        EditorGUILayout.Space(20);

        if (GUILayout.Button("Open Editor", GUILayout.Height(40)))
        {
            MiniGameLevelWindow.Open(editor);
        }

        EditorGUILayout.Space(20);

        SerializedProperty stages = serializedObject.FindProperty("_stages");

        ShowInfo(stages);

        serializedObject.ApplyModifiedProperties();
    }

    private void ShowInfo(SerializedProperty property)
    {
        foreach (SerializedProperty prop in property)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            prop.isExpanded = EditorGUILayout.Foldout(prop.isExpanded, prop.displayName);

            var enable = prop.FindPropertyRelative("Enable");
            enable.boolValue = EditorGUILayout.Toggle(enable.boolValue, GUILayout.MaxWidth(16));
            EditorGUILayout.EndHorizontal();

            if (prop.isExpanded)
            {
                GUI.enabled = false;

                EditorGUILayout.PropertyField(prop.FindPropertyRelative("PendulumSpeed"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("LifeCount"));

                foreach (SerializedProperty circlesList in prop.FindPropertyRelative("CirclesList"))
                {
                    EditorGUILayout.Space(10);
                    EditorGUILayout.BeginHorizontal("box");

                    var circle = circlesList.FindPropertyRelative("CircleNumber");
                    EditorGUILayout.LabelField($"Circle Number - {circle.enumNames[circle.enumValueIndex]}", GUILayout.MaxWidth(140));

                    EditorGUILayout.BeginVertical();

                    EditorGUILayout.PropertyField(circlesList.FindPropertyRelative("CircleSpeed"));
                    foreach (SerializedProperty platform in circlesList.FindPropertyRelative("PlatformsList"))
                    {
                        var rewardType = platform.FindPropertyRelative("RewardType");
                        var rewardCount = platform.FindPropertyRelative("StageReward");
                        rewardCount.arraySize = serializedObject.FindProperty("_stageCount").intValue;

                        EditorGUILayout.PropertyField(platform.FindPropertyRelative("PlatformSize"));
                        EditorGUILayout.PropertyField(rewardType);
                        if ((GamePlatformType)rewardType.enumValueIndex != GamePlatformType.Fail)
                        {
                            EditorGUILayout.IntField("Reward Variants", rewardCount.arraySize);

                            byte fail = 0;
                            foreach (SerializedProperty reward in rewardCount)
                            {
                                fail++;

                                if (reward.objectReferenceValue == null)
                                {
                                    EditorGUILayout.HelpBox($"Empty link to RewardBundle number {fail}", MessageType.Error);
                                }
                            }
                        }
                        EditorGUILayout.Space(5);
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                GUI.enabled = true;
            }
            EditorGUILayout.EndVertical();
        }
    }
}