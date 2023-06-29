using Leopotam.Ecs;

namespace Voody.UniLeo
{
	public static class WorldHandler
	{
    	private static EcsWorld world;
    
    	public static void Init(EcsWorld ecsWorld) 
    	{
        	world = ecsWorld;
        }
    	public static EcsWorld GetWorld()
    	{
            return world;
    	}

    	public static void Destroy()
    	{
        	world = null;
    	}
	}
}
