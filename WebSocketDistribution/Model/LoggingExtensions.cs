using System;

namespace WebSocketDistribution.Model
{
    static class LoggingExtensions
    {
        public static string ToShortString(this Exception ex)
        {
            return $"{ex.GetType().Name}: {ex.Message}";
        }
    }
}
