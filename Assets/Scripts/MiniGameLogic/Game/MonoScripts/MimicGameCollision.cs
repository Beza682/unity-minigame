using UnityEngine;
using Leopotam.Ecs;
using Voody.UniLeo;

public class MimicGameCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var entity = collision.gameObject.GetComponent<ConvertToEntity>().TryGetEntity();

        if (entity.HasValue)
        {
            if (entity.Value.Has<CircleNumberComponent>())
            {
                WorldHandler.GetWorld().NewEntity().Get<ChangeRotationComponent>() = new ChangeRotationComponent()
                {
                    RotationObject = entity.Value.Get<CircleNumberComponent>().CircleNumber,
                };
            }
        }
    }
}
