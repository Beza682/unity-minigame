using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class InitReferences
{
    public static async Task<T> SetReference<T>(string path)
    {
        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(path);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            return handle.Result;
        }
        else
        {
            return default;
        }
    }
}