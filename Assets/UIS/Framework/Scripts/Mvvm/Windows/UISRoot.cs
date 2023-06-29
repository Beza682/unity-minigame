using UnityEngine;

namespace UIS
{
    public class UISRoot : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}