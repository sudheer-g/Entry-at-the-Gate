using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.IO;

namespace IdentitySample.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {

        }

        public HomeController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "App description.TODO";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Placeholder.TODO.";

            return View();
        }

        public async Task<ActionResult> Profiles()
        {
            string id = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            return View(user);
        }

        public ActionResult Notifications()
        {
            string userId = User.Identity.GetUserId();
            ViewBag.userId = userId;
            string notificationsString = "<html><body>";
            if(User.IsInRole("Admin"))
            {
                using (StreamReader reader = new StreamReader(Server.MapPath("~/UserNotifications/") + "Admin.txt"))
                {
                    while (reader.Peek() >= 0)
                        notificationsString += "<p>" + reader.ReadLine() + "</p>";
                }
            }
            else
            {
                using (StreamReader reader = new StreamReader(Server.MapPath("~/UserNotifications/") + userId + ".txt"))
                {
                    while (reader.Peek() >= 0)
                        notificationsString += "<p>" + reader.ReadLine() + "</p>";
                }
            }
            
            notificationsString += "</body></html>";
            ViewBag.notifications = notificationsString;
            return View();
        }
    }
}
