using Leopotam.Ecs;
using UnityEngine;
using Voody.UniLeo;

public sealed class SpawnPlatformsSystem : IEcsRunSystem
{
    private readonly EcsFilter<StartGameTag> _startGameFilter = null;
    private readonly EcsFilter<NewGameTag> _newGameFilter = null;

    private readonly MiniGameSceneData _mimicSceneData = null;
    private readonly MiniGameConfigurator _gameConfig = null;

    public MiniGameInfo MimicGameInfo
    {
        get { return Data.Instance.MiniGameInfo; }
        set { Data.Instance.MiniGameInfo = value; }
    }

    public void Run()
    {
        SpawnPlatforms();
    }

    private void SpawnPlatforms()
    {
        if (_newGameFilter.IsEmpty() && _startGameFilter.IsEmpty()) return;

        _mimicSceneData.ResetCircles();

        for (int i = 0; i < MimicGameInfo.CurrentStage.CirclesList.Length; i++)
        {
            int circleNumber = (int)MimicGameInfo.CurrentStage.CirclesList[i].CircleNumber;
            var circleInfo = _mimicSceneData.CirclesInfo[circleNumber];
            var platforms = MimicGameInfo.CurrentStage.CirclesList[i].PlatformsList;

            for (int j = 0; j < platforms.Length; j++)
            {
                var platformPref = _gameConfig.PlatformsDict[platforms[j].PlatformSize];
                var point = FindPoint(circleInfo.Container, platforms[j].Angle, circleInfo.Circle.rect.height / 2 * circleInfo.Circle.localScale.y - platformPref.transform.localScale.y / 2);
                var platform = EntityObject.InstantiateEntity(platformPref, point, Quaternion.Euler(0, 0, platforms[j].Angle), circleInfo.Container).TryGetEntity().Value;

                platform.Get<CircleNumberComponent>().CircleNumber = MimicGameInfo.CurrentStage.CirclesList[i].CircleNumber;
                platform.Get<PlatformVisualComponent>().SpriteColor.color = _gameConfig.PlatformsColorDict[platforms[j].RewardType];
                platform.Replace(platforms[j]);
            }
        }
    }

    private Vector3 FindPoint(Transform center, float angle, float radius)
    {
        return new Vector3(center.position.x + radius * Mathf.Cos((270 + angle) * Mathf.Deg2Rad),
                           center.position.y + radius * Mathf.Sin((270 + angle) * Mathf.Deg2Rad),
                           -0.1f);
    }
}