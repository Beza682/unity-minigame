using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad()]
#endif
public class DataService
{
    private const string _NAME_FILE_SAVE = "/data2.sav";

#if UNITY_EDITOR
    private const string _JSON_FILE_SAVE = "/data2.json";
    private const string SAVE_PATH = "/Saves/";

    static DataService()
    {
        Directory.CreateDirectory(Application.persistentDataPath + SAVE_PATH);
    }

    [MenuItem("DataControl/Remove Data File %&R")]
    public static void RemoveDataFile()
    {
        File.Delete(Application.persistentDataPath + _NAME_FILE_SAVE);
        Debug.Log($"Delete file {Application.persistentDataPath + _NAME_FILE_SAVE}");
    }

    [MenuItem("DataControl/Save Data File %&S")]
    public static void SaveDataFile()
    {
        var path = EditorUtility.SaveFilePanel("Save Data", Application.persistentDataPath + SAVE_PATH, "", "");

        var filer = File.ReadAllText(Application.persistentDataPath + _NAME_FILE_SAVE);
        File.WriteAllText(path, filer.Replace("-", ""));
    }

    [MenuItem("DataControl/Load Data File %&L")]
    public static void LoadDataFile()
    {
        var path = EditorUtility.OpenFilePanel("Load Data", Application.persistentDataPath + SAVE_PATH, "");

        var loadFiler = File.ReadAllText(path);
        File.WriteAllText(Application.persistentDataPath + _NAME_FILE_SAVE, loadFiler.Replace("-", ""));
    }

    [MenuItem("DataControl/Clone and open data in JSON %&J")]
    public static void CloneDataFileToJSON()
    {
        var filer = File.ReadAllText(Application.persistentDataPath + _NAME_FILE_SAVE);
        string uncrypt = Uncrypt(filer);

        File.WriteAllText(Application.persistentDataPath + _JSON_FILE_SAVE, uncrypt.Replace("-", ""));
        EditorUtility.OpenWithDefaultApp(Application.persistentDataPath + _JSON_FILE_SAVE);
    }
#endif

    public static void Save<T>(T data)
    {
        Debug.Log($"Save {Application.persistentDataPath + _NAME_FILE_SAVE}");

        var hex = DataToString(data);
        File.WriteAllText(Application.persistentDataPath + _NAME_FILE_SAVE, hex.Replace("-", ""));
    }

    public static T Load<T>() where T : new ()
    {
        Debug.Log($"Load {Application.persistentDataPath + _NAME_FILE_SAVE}");

        if (File.Exists(Application.persistentDataPath + _NAME_FILE_SAVE))
        {
            var filer = File.ReadAllText(Application.persistentDataPath + _NAME_FILE_SAVE);
            string uncrypt = Uncrypt(filer);

            return JsonUtility.FromJson<T>(uncrypt);
        }
        else
        {
            return new T();
        }
    }

    private static string DataToString<T>(T data)
    {
        byte[] byteData = DataToBytes(data);
        return BitConverter.ToString(byteData);
    }

    private static byte[] DataToBytes<T>(T data)
    {
        string jsonSave = JsonUtility.ToJson(data);
        byte[] byteData = Encoding.UTF8.GetBytes(jsonSave);

        for (int i = 0; i < byteData.Count(); i++)
        {
            byteData[i] = (byte)(byte.MaxValue - byteData[i]);
        }

        return byteData;
    }

    public static string Uncrypt(string data)
    {
        int charsCount = data.Length;
        byte[] bytes = new byte[charsCount / 2];

        for (int i = 0; i < charsCount; i += 2)
        {
            bytes[i / 2] =
            (byte)(byte.MaxValue - Convert.ToByte(data.Substring(i, 2), 16));
        }

        return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
    }
}