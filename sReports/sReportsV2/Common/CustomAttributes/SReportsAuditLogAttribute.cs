using MongoDB.Driver;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.Mongo;
using sReportsV2.Models.User;
using sReportsV2.Security;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace sReportsV2.Common.CustomAttributes
{
    public class SReportsAuditLogAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext = Ensure.IsNotNull(filterContext, nameof(filterContext));
            AuditLog al = new AuditLog();
            al.Action = filterContext.ActionDescriptor.ActionName;
            al.Controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            al.Username = System.Web.HttpContext.Current.User.Identity.Name;
            al.Time = DateTime.Now;
            al.Json = new JavaScriptSerializer().Serialize(filterContext.ActionParameters);

            Task.Run(() => SaveLog(al)); //fire and forget*/
        }

        public void SaveLog(AuditLog log)
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            IMongoCollection<AuditLog> Collection = MongoDatabase.GetCollection<AuditLog>("auditlog") as IMongoCollection<AuditLog>;
            Collection.InsertOne(log);
        }

    }
}