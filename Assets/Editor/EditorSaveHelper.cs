using UnityEditor;
using UnityEngine;

public class EditorSaveHelper
{
    public static void Save<T>(T editor) where T : Object
    {
        if (GUI.changed)
        {
            Undo.RecordObject(editor, nameof(T));
            EditorUtility.SetDirty(editor);
        }
    }
}