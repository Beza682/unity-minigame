public abstract class ConstructSingleton<T> where T : new()
{
    private static T _instance;

    static ConstructSingleton()
    {
        _instance = new T();
    }

    public static T Instance
    {
        get => _instance;
        set => _instance = value;
    }
}