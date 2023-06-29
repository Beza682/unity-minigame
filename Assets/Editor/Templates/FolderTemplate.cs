using System;
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class FolderTemplate : ScriptableObject
{
    [MenuItem("Assets/Create/ECS folders", false, 18)]
    static void CreateProviderTpl()
    {
        var path = GetAssetPath();

        CreateAndRenameAsset($"{path}/NewFolder", GetIcon(),
            (name) => CreateFolders(name, path));
    }

    static string CreateFolders(string proto, string path)
    {
        try
        {
            string mainGuid = AssetDatabase.CreateFolder(path, proto.Replace(path + "/", ""));
            string mainPath = AssetDatabase.GUIDToAssetPath(mainGuid);

            AssetDatabase.CreateFolder(mainPath, "Components");
            AssetDatabase.CreateFolder(mainPath, "Providers");
            AssetDatabase.CreateFolder(mainPath, "Systems");
            string tagsGuid = AssetDatabase.CreateFolder(mainPath, "Tags");
            string tagsPath = AssetDatabase.GUIDToAssetPath(tagsGuid);
            AssetDatabase.CreateFolder(tagsPath, "Components");
            AssetDatabase.CreateFolder(tagsPath, "Providers");

            return File.ReadAllText(Path.Combine(path ?? "", proto));
        }
        catch
        {
            return null;
        }
    }

    static string GetAssetPath()
    {
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!string.IsNullOrEmpty(path) && AssetDatabase.Contains(Selection.activeObject))
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                path = Path.GetDirectoryName(path);
            }
        }
        else
        {
            path = "Assets";
        }
        return path;
    }

    static Texture2D GetIcon()
    {
        return EditorGUIUtility.IconContent("Folder Icon").image as Texture2D;
    }

    static void CreateAndRenameAsset(string path,Texture2D icon, Action<string> onSuccess)
    {
        var action = CreateInstance<CustomEndNameAction>();
        action.Callback = onSuccess;
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, action, path, icon, null);
    }

    sealed class CustomEndNameAction : EndNameEditAction
    {
        [NonSerialized] public Action<string> Callback;

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            Callback?.Invoke(pathName);
        }
    }
}
