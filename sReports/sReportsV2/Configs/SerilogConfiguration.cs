using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.Configs
{
    public static class SerilogConfiguration
    {
        public static void ConfigureWritingToFile()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(HttpContext.Current.Server.MapPath("~/App_Data/logs/log.txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}