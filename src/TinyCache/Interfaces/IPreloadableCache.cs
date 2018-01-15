namespace TinyCacheLib
{
    public interface IPreloadableCache : ICacheStorage
    {
        string GetAllAsLoadableString();
        void LoadFromString(string fillData);
    }
}
