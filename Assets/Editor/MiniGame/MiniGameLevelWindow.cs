using UnityEngine;
using UnityEditor;
using System;

public class MiniGameLevelWindow : ExtendedEditorWindow
{
    private static readonly Color BgColor = new Color(56 / 255f, 56 / 255f, 56 / 255f);
    private MiniGameGrid _grid;
    private Vector3? _LastMousePosition;
    private MiniGameConfigurator _gameConfigurator;
    private static MiniGamePoolObject _miniGame;
    public event Action<EventType, Vector2, KeyCode> OnKeyEvent;
    private int _radius;

    #region CONSTS
    private const string _SELECT_LEBEL = "Select an item from the list";
    private const int _FIRST_CIRCLE_RAD = 155;
    private const int _SECOND_CIRCLE_RAD = 177;
    private const int _THIRD_CIRCLE_RAD = 199;
    private const int _FOURTH_CIRCLE_RAD = 221;
    private const int _FAIL_CIRCLE_RAD = 248;
    private const int _FIELD_CIRCLE_RAD = 263;
    #endregion

    public static void Open(MiniGamePoolObject mimicGame)
    {
        MiniGameLevelWindow window = GetWindow<MiniGameLevelWindow>(mimicGame.name);
        window.SerializedObject = new SerializedObject(mimicGame);

        _miniGame = mimicGame;
    }

    private void OnEnable()
    {
        _grid = new MiniGameGrid(this);
        var assets = AssetDatabase.FindAssets("t:MiniGameConfigurator");
        _gameConfigurator = AssetDatabase.LoadAssetAtPath<MiniGameConfigurator>(AssetDatabase.GUIDToAssetPath(assets[0]));

        wantsMouseMove = true;
    }

    public void OnGUI()
    {
        CurrentProperty = SerializedObject.FindProperty("_stages");

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        EditorGUILayout.LabelField("Stages");
        DrawSidebar(CurrentProperty);
        EditorGUILayout.EndVertical();

        GUI.color = BgColor;

        var workSpace = EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        GUI.DrawTexture(new Rect(workSpace.position, workSpace.size), EditorGUIUtility.whiteTexture);
        _grid.Draw();
        KeysEvents();
        DrawGameSpace();
        EditorGUILayout.EndVertical();
        GUI.color = Color.white;

        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(400), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        if (SelectedProperty != null && DrawMenuAndLevel)
        {
            EditorGUILayout.LabelField($"Stage - {SelectedProperty.displayName}");
            DrawSelectedPropertiesPanel();
            DrawLevel();
        }
        else
        {
            EditorGUILayout.LabelField(_SELECT_LEBEL);
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        Apply();
    }

    private void DrawSelectedPropertiesPanel()
    {
        CurrentProperty = SelectedProperty;
        DrawField("Name", true);
        DrawFloatSlider("PendulumSpeed", 2, 4, true);
        DrawIntSlider("LifeCount", 1, 5, true);
        DrawCirclesInfo();
    }

    private void DrawCirclesInfo()
    {
        SerializedProperty circlesList = CurrentProperty.FindPropertyRelative("CirclesList");

        EditorGUILayout.BeginVertical("box");

        DrawTitle(circlesList);

        foreach (SerializedProperty elem in circlesList)
        {
            var circle = elem.FindPropertyRelative("CircleNumber");

            elem.isExpanded = EditorGUILayout.Foldout(elem.isExpanded, circle.enumNames[circle.enumValueIndex]);

            if (elem.isExpanded)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(elem.FindPropertyRelative("CircleNumber"), true);
                var speed = elem.FindPropertyRelative("CircleSpeed");
                var platforms = elem.FindPropertyRelative("PlatformsList");

                speed.floatValue = EditorGUILayout.Slider(speed.displayName, speed.floatValue, 0f, 0.1f);
                DrawTitle(platforms);

                foreach (SerializedProperty platform in platforms)
                {
                    var size = platform.FindPropertyRelative("PlatformSize");
                    var type = platform.FindPropertyRelative("RewardType");

                    platform.isExpanded = EditorGUILayout.Foldout(platform.isExpanded, $"{(PlatformsSize)size.enumValueIndex} {(GamePlatformType)type.enumValueIndex} platform");

                    if (platform.isExpanded)
                    {
                        EditorGUI.indentLevel++;

                        EditorGUILayout.PropertyField(platform.FindPropertyRelative("Angle"), true);
                        EditorGUILayout.PropertyField(size, true);
                        EditorGUILayout.PropertyField(type, true);

                        var rewards = platform.FindPropertyRelative("StageReward");

                        if ((GamePlatformType)type.enumValueIndex != GamePlatformType.Fail)
                        {
                            rewards.arraySize = _miniGame.StageCount;

                            byte rewardNumber = 0;

                            foreach (SerializedProperty reward in rewards)
                            {
                                rewardNumber++;

                                EditorGUI.indentLevel++;

                                EditorGUILayout.PropertyField(reward, new GUIContent { text = $"Reward stage {rewardNumber}" }, true);

                                EditorGUI.indentLevel--;

                                if (reward.objectReferenceValue == null)
                                {
                                    EditorGUILayout.HelpBox($"Empty link to RewardBundle", MessageType.Error); ;
                                }
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.indentLevel--;
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawGameSpace()
    {
        Handles.BeginGUI();

        Handles.color = Color.black;

        Handles.DrawLine(
            _grid.GridToGUI(Vector2.zero * MiniGameGrid.DEFAULT_CELL_SIZE),
            _grid.GridToGUI(_grid.FindPoint(_miniGame.PendulumAmplitude, _FIELD_CIRCLE_RAD) * MiniGameGrid.DEFAULT_CELL_SIZE));

        Handles.DrawLine(
            _grid.GridToGUI(Vector2.zero * MiniGameGrid.DEFAULT_CELL_SIZE),
            _grid.GridToGUI(_grid.FindPoint(-_miniGame.PendulumAmplitude, _FIELD_CIRCLE_RAD) * MiniGameGrid.DEFAULT_CELL_SIZE));

        Handles.DrawWireDisc(_grid.GridToGUI(Vector2.zero * MiniGameGrid.DEFAULT_CELL_SIZE), Vector3.forward, _FIELD_CIRCLE_RAD * MiniGameGrid.DEFAULT_CELL_SIZE / _grid.Zoom);

        Handles.color = Color.gray;

        Handles.DrawWireDisc(_grid.GridToGUI(Vector2.zero * MiniGameGrid.DEFAULT_CELL_SIZE), Vector3.forward, _FIRST_CIRCLE_RAD * MiniGameGrid.DEFAULT_CELL_SIZE / _grid.Zoom);
        Handles.DrawWireDisc(_grid.GridToGUI(Vector2.zero * MiniGameGrid.DEFAULT_CELL_SIZE), Vector3.forward, _SECOND_CIRCLE_RAD * MiniGameGrid.DEFAULT_CELL_SIZE / _grid.Zoom);
        Handles.DrawWireDisc(_grid.GridToGUI(Vector2.zero * MiniGameGrid.DEFAULT_CELL_SIZE), Vector3.forward, _THIRD_CIRCLE_RAD * MiniGameGrid.DEFAULT_CELL_SIZE / _grid.Zoom);
        Handles.DrawWireDisc(_grid.GridToGUI(Vector2.zero * MiniGameGrid.DEFAULT_CELL_SIZE), Vector3.forward, _FOURTH_CIRCLE_RAD * MiniGameGrid.DEFAULT_CELL_SIZE / _grid.Zoom);

        Handles.color = Color.red;

        Handles.DrawWireDisc(_grid.GridToGUI(Vector2.zero * MiniGameGrid.DEFAULT_CELL_SIZE), Vector3.forward, _FAIL_CIRCLE_RAD * MiniGameGrid.DEFAULT_CELL_SIZE / _grid.Zoom);

        //Handles.DrawSolidArc(_grid.GridToGUI(Vector2.zero * MimicGrid.DEFAULT_CELL_SIZE),
        //                     Vector3.forward,
        //                     _grid.GridToGUI(_grid.FindPoint(-_mimicGame.PendulumAmplitude, 300) * MimicGrid.DEFAULT_CELL_SIZE),
        //                     _mimicGame.PendulumAmplitude * 2,
        //                     300 * MimicGrid.DEFAULT_CELL_SIZE / _grid.Zoom);

        Handles.EndGUI();
    }

    private void DrawLevel()
    {
        if (CurrentProperty != null && DrawMenuAndLevel)
        {
            var circlesList = CurrentProperty.FindPropertyRelative("CirclesList");

            foreach (SerializedProperty circle in circlesList)
            {
                var circleNumber = circle.FindPropertyRelative("CircleNumber");
                var platformsList = circle.FindPropertyRelative("PlatformsList");

                switch ((MiniGameRotationObject)circleNumber.enumValueIndex)
                {
                    case MiniGameRotationObject.First:
                        _radius = _FIRST_CIRCLE_RAD;
                        break;
                    case MiniGameRotationObject.Second:
                        _radius = _SECOND_CIRCLE_RAD;
                        break;
                    case MiniGameRotationObject.Third:
                        _radius = _THIRD_CIRCLE_RAD;
                        break;
                    case MiniGameRotationObject.Fourth:
                        _radius = _FOURTH_CIRCLE_RAD;
                        break;
                }

                foreach (SerializedProperty platform in platformsList)
                {
                    float angle = platform.FindPropertyRelative("Angle").floatValue;
                    Color color = _gameConfigurator.PlatformsColorDict[(GamePlatformType)platform.FindPropertyRelative("RewardType").enumValueIndex];
                    GameObject go = _gameConfigurator.PlatformsDict[(PlatformsSize)platform.FindPropertyRelative("PlatformSize").enumValueIndex].gameObject;
                    RectTransform rect = go.transform.GetComponent<RectTransform>();

                    Vector3 size = new Vector3(rect.rect.width * go.transform.localScale.x, rect.rect.height * go.transform.localScale.x, 0) * 30;

                    Handles.BeginGUI();
                    Handles.color = color;
                    DrawSquare(_grid.GridToGUI(_grid.FindPoint(angle, _radius) * MiniGameGrid.DEFAULT_CELL_SIZE), size * MiniGameGrid.DEFAULT_CELL_SIZE / _grid.Zoom, angle);
                    Handles.EndGUI();
                }
            }
        }
    }

    private Vector3 FindSquareVertices(Vector3 center, Vector3 vector, float angle)
    {
        var ssin = MathF.Sin(angle * Mathf.Deg2Rad);
        var scos = MathF.Cos(angle * Mathf.Deg2Rad);

        vector.x = center.x + (vector.x - center.x) * scos -
                (vector.y - center.y) * ssin;
        vector.y = center.y - (vector.x - center.x) * ssin -
                (vector.y - center.y) * scos;

        return vector;
    }

    private void DrawSquare(Vector3 center, Vector3 size, float angle)
    {
        Vector3 vector = size * 0.5f;

        Vector3[] array = new Vector3[4]
        {
                FindSquareVertices(center, center + new Vector3(- vector.x, - vector.y, vector.z), angle),
                FindSquareVertices(center, center + new Vector3(- vector.x, vector.y,  vector.z), angle),
                FindSquareVertices(center, center + new Vector3(vector.x, vector.y, vector.z), angle),
                FindSquareVertices(center, center + new Vector3(vector.x, - vector.y, vector.z), angle),
        };

        Handles.DrawPolyLine(array);
        Handles.DrawLine(array[0], array[3]); //up line

        Handles.color = Color.cyan;
        Handles.DrawWireCube(center, Vector3.one * 1f);
    }

    private void KeysEvents()
    {
        var curEvent = Event.current;
        var mousePosition = curEvent.mousePosition;

        switch (curEvent.type)
        {
            case EventType.MouseDrag:
                if (curEvent.button == 1)
                    DragGrid();
                break;
            case EventType.MouseDown:
                if (curEvent.button == 1)
                    _LastMousePosition = null;
                break;
            case EventType.ScrollWheel:
                OnScroll(curEvent.delta.y);
                break;
        }

        if (OnKeyEvent != null)
        {
            OnKeyEvent(curEvent.type, mousePosition, curEvent.keyCode);
        }
    }

    private void DragGrid()
    {
        var curMousePosition = Event.current.mousePosition;
        if (_LastMousePosition.HasValue)
        {
            var dv = GUIUtility.GUIToScreenPoint((Vector2)_LastMousePosition)
                     - GUIUtility.GUIToScreenPoint(curMousePosition);
            _grid.Move(dv);
            Repaint();
        }
        _LastMousePosition = curMousePosition;
    }

    private void OnScroll(float speed)
    {
        _grid.Zoom += speed * _grid.Zoom * 0.1f;
        Repaint();
    }
}
