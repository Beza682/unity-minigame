using UnityEditor;
using UnityEngine;

public class OpenFMODProject
{
    [MenuItem("FMOD/Open Project", false, -10)]
    public static void CloneDataFileToJSON()
    {
        EditorUtility.OpenWithDefaultApp(Application.dataPath.Replace("Assets", "FMODProject/MainTheme/MainTheme.fspro"));
    }
}
