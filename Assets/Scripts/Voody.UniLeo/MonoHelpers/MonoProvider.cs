using Leopotam.Ecs;

namespace Voody.UniLeo
{
    public abstract class MonoProvider <T> : BaseMonoProvider, IConvertToEntity where T : struct
    {
        public T value;

        void IConvertToEntity.Convert(EcsEntity entity)
        {
            entity.Replace(value);
        }
    }
}
