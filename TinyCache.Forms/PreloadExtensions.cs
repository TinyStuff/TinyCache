using System;
namespace TinyCache.Forms
{
    public static class PreloadExtensions
    {
        public static void LoadFromFile(this IPreloadableCache store, string filename)
        {
            var text = System.IO.File.ReadAllText(filename);
            store.LoadFromString(text);
        }
    }
}
