using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

sealed class TemplateGenerator : ScriptableObject
{
    const string Title = "GameDev template generator";

    const string ProviderTemplate = "ProviderTemplate.cs.txt";

    [MenuItem("Assets/Create/LeoECS/Providers/Create Provider template", false, -195)]
    static void CreateProviderTpl()
    {
        CreateAndRenameAsset($"{GetAssetPath()}/Provider.cs", GetIcon(),
            (name) => CreateTemplateInternal(GetTemplateContent(ProviderTemplate), name));
    }

    public static string CreateTemplate(string proto, string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return "Invalid filename";
        }
        var ns = EditorSettings.projectGenerationRootNamespace.Trim();
        if (string.IsNullOrEmpty(EditorSettings.projectGenerationRootNamespace))
        {
            ns = "Client";
        }

        string cn = SanitizeClassName(Path.GetFileNameWithoutExtension(fileName)).Contains("TagProvider") ?
            SanitizeClassName(Path.GetFileNameWithoutExtension(fileName).Replace("TagProvider", "Tag")) :
            SanitizeClassName(Path.GetFileNameWithoutExtension(fileName).Replace("Provider", "Component"));

        proto = proto.Replace("#NS#", ns);
        proto = proto.Replace("#SCRIPTNAME#", SanitizeClassName(Path.GetFileNameWithoutExtension(fileName)));
        proto = proto.Replace("#COMPONENTNAME#", cn);
        //proto = proto.Replace("#COMPONENTNAME#", SanitizeClassName(Path.GetFileNameWithoutExtension(fileName).Replace("Provider", "Component")));

        try
        {
            File.WriteAllText(AssetDatabase.GenerateUniqueAssetPath(fileName), proto);
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
            EditorUtility.DisplayDialog(Title, res, "Close");
        }
        return res;
    }

    static string GetTemplateContent(string proto)
    {
        // hack: its only one way to get current editor script path. :(
        var pathHelper = CreateInstance<TemplateGenerator>();
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