namespace TinyCacheLib
{
    public class CacheUpdatedEvt
    {
        public CacheUpdatedEvt() { }

        public CacheUpdatedEvt(string key, object value)
        {
            Key = key;
            Value = value; 
        }

        public string Key { get; set; }

        public object Value { get; set; }
    }
}
