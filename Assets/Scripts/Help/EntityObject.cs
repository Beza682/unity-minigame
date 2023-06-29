using UnityEngine;
using Voody.UniLeo;

namespace Leopotam.Ecs
{
    public class EntityObject
    {
        private static ConvertToEntity CreateEntity(in ConvertToEntity efi)
        {
            var entity = WorldHandler.GetWorld().NewEntity();

            foreach (var component in efi.gameObject.GetComponents<Component>())
            {
                if (component is IConvertToEntity entityComponent)
                {
                    entityComponent.Convert(entity);
                    Object.Destroy(component);
                }
            }

            efi.setProccessed();
            efi.Set(entity);

            return efi;
        }

        public static ConvertToEntity InstantiateEntity(in ConvertToEntity entity, in Vector3 position, in Quaternion rotation, in Transform parent)
        {
            return CreateEntity(Object.Instantiate(entity, position, rotation, parent));
        }

        public static ConvertToEntity InstantiateEntity(in ConvertToEntity entity, in Vector3 position, in Quaternion rotation)
        {
            return CreateEntity(Object.Instantiate(entity, position, rotation));
        }

        public static ConvertToEntity InstantiateEntity(in ConvertToEntity entity, in Transform parent)
        {
            return CreateEntity(Object.Instantiate(entity, parent));
        }

        public static ConvertToEntity InstantiateEntity(in ConvertToEntity entity)
        {
            return CreateEntity(Object.Instantiate(entity));
        }

        public static T GetComponent<T>(in ConvertToEntity go, in Vector3 position, in Quaternion rotation, in Transform parent) where T : struct
        {
            var ent = InstantiateEntity(go, position, rotation, parent).TryGetEntity().Value;
            var comp = ent.Get<T>();
            return comp;
        }
        
        public static T GetComponent<T>(in ConvertToEntity go, in Vector3 position, in Quaternion rotation) where T : struct
        {
            var ent = InstantiateEntity(go, position, rotation).TryGetEntity().Value;
            var comp = ent.Get<T>();
            return comp;
        } 
        
        public static T GetComponent<T>(in ConvertToEntity go, in Transform parent) where T : struct
        {
            var ent = InstantiateEntity(go, parent).TryGetEntity().Value;
            var comp = ent.Get<T>();
            return comp;
        } 
        
        public static T GetComponent<T>(in ConvertToEntity go) where T : struct
        {
            var ent = InstantiateEntity(go).TryGetEntity().Value;
            var comp = ent.Get<T>();
            return comp;
        }
    }
}