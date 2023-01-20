using Serilog;

namespace sReportsV2.Common.Helpers
{
    public static class LogHelper
    {
        public static void Error(string message)
        {
            Log.Error(message);
        }

        public static void Info(string message)
        {
            Log.Information(message);
        }
    }
}
