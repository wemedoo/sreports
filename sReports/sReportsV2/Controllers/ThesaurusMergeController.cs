using AutoMapper;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Singleton;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public partial class ThesaurusEntryController : BaseController
    {
        [SReportsAuthorize(Module = ModuleNames.Thesaurus, Permission = PermissionNames.CreateUpdate)]
        public ActionResult TakeBoth(int currentId)
        {
            thesaurusEntryBLL.TakeBoth(currentId);
            SingletonDataContainer.Instance.RefreshSingleton(currentId.ToString());
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [SReportsAuthorize(Module = ModuleNames.Thesaurus, Permission = PermissionNames.CreateUpdate)]
        public ActionResult MergeThesauruses(int currentId, int targetId, List<string> valuesForMerge)
        {
            thesaurusEntryBLL.MergeThesauruses(currentId, targetId, valuesForMerge, Mapper.Map<UserData>(userCookieData));
            SingletonDataContainer.Instance.RefreshSingleton(targetId.ToString());
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [SReportsAuthorize(Module = ModuleNames.Thesaurus, Permission = PermissionNames.CreateUpdate)]
        public ActionResult Merge()
        {
            int entriesUpdated = thesaurusEntryBLL.Merge(Mapper.Map<UserData>(userCookieData));
            if(entriesUpdated > 0)
            {
                SingletonDataContainer.Instance.RefreshSingleton();
            }
            return new ContentResult
            {
                Content = "Merge Process Finished!",
                ContentType = "application/json"
            };
        }
    }
}