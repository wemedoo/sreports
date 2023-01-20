using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;

namespace sReportsV2.Common.Extensions
{
    public static class LocalizationExtension
    {
        public static string GetStringOrDefault(this ResourceManager resourceManager, string name)
        {
            resourceManager = Ensure.IsNotNull(resourceManager, nameof(resourceManager));
            return resourceManager.GetString(name) ?? name;
        }
    }
}