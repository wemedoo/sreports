using sReportsV2.Common.Helpers.EmailSender.Interface;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IEmailSender emailSender;
        public HomeController(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult TestEMail()
        {
            Task.Run(() => emailSender.SendAsync("Testing email", string.Empty, "<div>Testing email</div>", "danilo.acimovic@wemedoo.com"));

            return Json("Test email endpoint is finished", JsonRequestBehavior.AllowGet);
        }

    }
}