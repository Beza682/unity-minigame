using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class TransformCustomTool : EditorWindow
{
    private Camera _camera;
    private Vector2 _scroll;
    private Vector3Int _direction;
    private float _distance = 100;
    private int _layer;
    private List<(string name, string oldValue, string newValue)> _info = new(20);
    #region CONSTS
    private const string _FAIL = "Fail";
    #endregion

    [MenuItem("CustomTools/2.5D Tool")]
    public static void ShowWindow()
    {
        GetWindow<TransformCustomTool>("2.5D Tool");
    }

    private void OnEnable()
    {
        _camera = Camera.main;
    }

    private void OnGUI()
    {
        _camera = Camera.main;

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Distance", GUILayout.MaxWidth(100f));
        _distance = EditorGUILayout.FloatField(_distance);
        EditorGUILayout.LabelField("Layer", GUILayout.MaxWidth(100f));
        _layer = EditorGUILayout.LayerField(_layer);
        if (GUILayout.Button("Help", GUILayout.MinWidth(10), GUILayout.ExpandWidth(false)))
        {
            EditorUtility.DisplayDialog("Help about 2.5D Tool",
                "The tool is used to change the position of a point and automatically resize an object.\r\n" +
                "To change the position of an object, select the direction of the beam in the \"Direction\" field towards the desired collider the desired collider, after which the \"Set position\" button will become active.\r\n" +
                "The result of the tool will be displayed in the window below.\r\nThe red inscription \"Failure\" means that the position change did not occur.\r\n" +
                "A green inscription with new coordinates means that the position change was successful.\r\n" +
                "To change the scale of objects, select the desired object and click the \"Set scale\" button.",
                "OK");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Direction", GUILayout.MaxWidth(100f));
        _direction = EditorGUILayout.Vector3IntField(string.Empty, _direction);
        EditorGUILayout.EndHorizontal();
        if (_direction == default)
        {
            EditorGUILayout.HelpBox("Set raycast direction!", MessageType.Error);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name", GUILayout.MinWidth(120f), GUILayout.ExpandWidth(true));
        EditorGUILayout.LabelField("Old value", GUILayout.MinWidth(140f), GUILayout.ExpandWidth(true));
        EditorGUILayout.LabelField("New value", GUILayout.MinWidth(140f), GUILayout.ExpandWidth(true));
        EditorGUILayout.EndHorizontal();

        _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        EditorGUILayout.Space();
        foreach (var info in _info)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(info.name, GUILayout.MinWidth(120f), GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField(info.oldValue, GUILayout.MinWidth(140f), GUILayout.ExpandWidth(true));
            GUI.color = info.newValue == _FAIL ? Color.red : Color.green;
            EditorGUILayout.LabelField(info.newValue, GUILayout.MinWidth(140f), GUILayout.ExpandWidth(true));
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal();
        GUI.enabled = _direction != default;
        if (GUILayout.Button("Set position", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
        {
            string oldValue;
            string newValue;
            _info.Clear();

            foreach (GameObject gameObject in Selection.gameObjects)
            {
                Undo.RecordObject(gameObject.transform, $"Set Position to {gameObject.name} (CustomTool)");

                oldValue = gameObject.transform.position.ToString();

                if (Physics.Raycast(gameObject.transform.position, _direction, out var hit, _distance, LayerMask.GetMask(LayerMask.LayerToName(_layer))))
                {
                    gameObject.transform.position = hit.point;
                    newValue = gameObject.transform.position.ToString();
                }
                else
                {
                    newValue = _FAIL;
                }
                _info.Add((gameObject.name, oldValue, newValue));
            }
        }
        GUI.enabled = true;
        if (GUILayout.Button("Set scale", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
        {
            _info.Clear();

            foreach (GameObject gameObject in Selection.gameObjects)
            {
                Undo.RecordObject(gameObject.transform, $"Set Local Scale in {gameObject.name} (CustomTool)");

                float distance = Mathf.Abs(_camera.gameObject.transform.position.z - gameObject.transform.position.z);

                gameObject.transform.localScale = new Vector3(distance / (30.65f * 0.87f), distance / 30.65f, 1);
            }
        }

        EditorGUILayout.EndHorizontal();
    }
}