using Newtonsoft.Json;

namespace Intervent.Framework.Clone
{
    public static class CloneUtil
    {
        public static T DeepClone<T>(object obj)
        {
            var temp = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(temp);
        }
    }
}
