using UnityEngine;
using UnityEngine.SceneManagement;

public class MonoResetGame : MonoBehaviour
{
    public void ResetGame()
    {
        Destroy(GameObject.Find(nameof(EcsGameStartup)));
        DataService.Save(new Data());
        SceneManager.LoadScene("LoaderScene", LoadSceneMode.Single);
    }
}