using Leopotam.Ecs;

namespace GameDev.Ilink
{
    public static class EntitySorter
    {
        public static int AddAndShortEntity<TInc1, TInc2>(in EcsFilter<TInc1> mainFilter, in EcsFilter<TInc1, TInc2> additiveFilter) where TInc1 : struct
                                                                                                                                      where TInc2 : struct
        {
            if (additiveFilter.IsEmpty()) return 0;

            int newNumber = 0;

            for (int i = 0; i < additiveFilter.GetEntitiesCount(); i++)
            {
                ref var newEntityNumber = ref additiveFilter.Get1(i);
                var shortNumber = EcsComponentType<TInc1>.Type.GetField("ShortNumber");
                newNumber = (int)shortNumber.GetValue(newEntityNumber);

                for (int j = 0; j < mainFilter.GetEntitiesCount(); j++)
                {
                    ref var entityNumber = ref mainFilter.Get1(j);
                    var mainShortNumber = EcsComponentType<TInc1>.Type.GetField("ShortNumber");
                    int mainNumber = (int)mainShortNumber.GetValue(entityNumber);

                    if (newNumber <= mainNumber)
                    {
                        newNumber = mainNumber + 1;
                    }
                }
            }

            return newNumber;
        }

        public static EcsEntity GetFirstEntity<TInc1>(in EcsFilter<TInc1> filter) where TInc1 : struct
        {
            int min = int.MaxValue;
            int id = 0;

            for (int i = 0; i < filter.GetEntitiesCount(); i++)
            {
                ref var entityNumber = ref filter.Get1(i);

                int number = (int)EcsComponentType<TInc1>.Type.GetField("ShortNumber").GetValue(entityNumber);

                if (min > number)
                {
                    id = i;
                    min = number;
                }
            }

            return filter.GetEntity(id);
        }

        public static EcsEntity GetLastEntity<TInc1>(in EcsFilter<TInc1> filter) where TInc1 : struct
        {
            int max = 0;
            int id = 0;

            for (int i = 0; i < filter.GetEntitiesCount(); i++)
            {
                ref var entityNumber = ref filter.Get1(i);

                int number = (int)EcsComponentType<TInc1>.Type.GetField("ShortNumber").GetValue(entityNumber);

                if (max < number)
                {
                    id = i;
                    max = number;
                }
            }

            return filter.GetEntity(id);
        }
    }
}

