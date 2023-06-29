using UnityEngine;

public class MiniGameSceneData : MonoBehaviour
{
    public CircleInfo[] CirclesInfo;

    public void ResetCircles()
    {
        for (int i = 0; i < CirclesInfo.Length; i++)
        {
            CirclesInfo[i].Container.DeleteChilds();
            CirclesInfo[i].Circle.rotation = Quaternion.identity;
        }
    }
}

[System.Serializable]
public struct CircleInfo
{
    public RectTransform Circle;
    public Transform Container;
}