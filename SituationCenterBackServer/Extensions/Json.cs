using Newtonsoft.Json;

namespace SituationCenterBackServer.Extensions
{
    public static class Json
    {
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
    }
}