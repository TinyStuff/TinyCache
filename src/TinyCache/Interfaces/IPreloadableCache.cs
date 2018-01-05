namespace TinyCache
{
    public interface IPreloadableCache : ICacheStorage
    {
        string GetAllAsLoadableString();
        void LoadFromString(string fillData);
    }
}
