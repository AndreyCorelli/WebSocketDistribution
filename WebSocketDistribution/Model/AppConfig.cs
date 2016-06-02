using System.Configuration;

namespace WebSocketDistribution.Model
{
    static class AppConfig
    {
        public static string GetStringParam(string key, string defaultValue)
        {
            var str = ConfigurationManager.AppSettings.Get(key);
            return string.IsNullOrEmpty(str) ? defaultValue : str;
        }

        public static int GetIntParam(string key, int defaultValue)
        {
            var str = ConfigurationManager.AppSettings.Get(key);
            return string.IsNullOrEmpty(str) ? defaultValue :
                int.Parse(str);
        }

        public static ulong GetULongParam(string key, ulong defaultValue)
        {
            var str = ConfigurationManager.AppSettings.Get(key);
            return string.IsNullOrEmpty(str) ? defaultValue :
                ulong.Parse(str);
        }

        public static bool GetBooleanParam(string key, bool defaultValue)
        {
            var str = ConfigurationManager.AppSettings.Get(key);
            if (string.IsNullOrEmpty(str)) return defaultValue;
            bool rst;
            if (!bool.TryParse(str, out rst))
                return defaultValue;
            return rst;
        }
    }
}
