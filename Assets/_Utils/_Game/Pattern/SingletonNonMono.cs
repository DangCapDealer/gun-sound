public abstract class SingletonNonMono<T> where T : class, new()
{
    private static T _instance;
    public static T I => _instance ??= new T();
}