using System.IO;
using System.Web;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IBlobStorageBLL
    {
        string Create(HttpPostedFileBase file);
        Stream Download(string resourceId);
    }
}
