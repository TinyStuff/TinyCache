namespace TinyCacheLib
{
    public class CacheUpdatedEvt
    {
        public string Key
        {
            get;
            set;
        }

        public object Value { get; set; }
    }
}
