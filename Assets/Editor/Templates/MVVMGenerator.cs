using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public sealed class MVVMGenerator : ScriptableObject
{
    #region CONSTS
    private const string _TITLE = "GameDev template generator";
    private const string _MVVM_TEMPLATE = "MVVMTemplate.cs.txt";
    private const string _FILE_NAME = "NewView.cs";
    #endregion

    [MenuItem("Assets/Create/MVVM Script", false, 62)]
    static void CreateMVVMTpl()
    {
        CreateAndRenameAsset($"{GetAssetPath()}/{_FILE_NAME}", GetIcon(),
            (name) => CreateTemplateInternal(GetTemplateContent(_MVVM_TEMPLATE), name));
    }

    public static string CreateTemplate(string proto, string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return "Invalid filename";
        }

        string cn = SanitizeClassName(Path.GetFileNameWithoutExtension(fileName)).Replace("View", string.Empty).Replace("Model", string.Empty);

        proto = proto.Replace("#SCRIPTNAME#", cn);

        try
        {
            var file = fileName.Replace("Model", string.Empty);
            var fileRename = file.Contains("View") ? file : file.Replace(".cs", "View.cs");

            File.WriteAllText(AssetDatabase.GenerateUniqueAssetPath(fileRename), proto);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        AssetDatabase.Refresh();
        return null;
    }

    static string SanitizeClassName(string className)
    {
        var sb = new StringBuilder();
        var needUp = true;
        foreach (var c in className)
        {
            if (char.IsLetterOrDigit(c))
            {
                sb.Append(needUp ? char.ToUpperInvariant(c) : c);
                needUp = false;
            }
            else
            {
                needUp = true;
            }
        }
        return sb.ToString();
    }

    static string CreateTemplateInternal(string proto, string fileName)
    {
        var res = CreateTemplate(proto, fileName);
        if (res != null)
        {
            EditorUtility.DisplayDialog(_TITLE, res, "Close");
        }
        return res;
    }

    static string GetTemplateContent(string proto)
    {
        // hack: its only one way to get current editor script path. :(
        var pathHelper = CreateInstance<MVVMGenerator>();
        var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(pathHelper)));
        DestroyImmediate(pathHelper);
        try
        {
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
        return EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
    }

    static void CreateAndRenameAsset(string fileName, Texture2D icon, Action<string> onSuccess)
    {
        var action = CreateInstance<CustomEndNameAction>();
        action.Callback = onSuccess;
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, action, fileName, icon, null);
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