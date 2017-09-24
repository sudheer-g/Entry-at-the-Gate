using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IdentitySample.Models;
using sampleMVC.Models;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Text;

namespace sampleMVC.Controllers
{
    [Authorize]
    public class GuestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public GuestsController()
        {

        }
        public GuestsController(ApplicationUserManager userManager)
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
            set
            {
                _userManager = value;
            }
        }


        // GET: Guests
        public ActionResult Index()
        {
            return View(db.GuestData.ToList());
        }

        // GET: Guests/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Guest guest = db.GuestData.Find(id);
            if (guest == null)
            {
                return HttpNotFound();
            }
            return View(guest);
        }

        // GET: Guests/Create
        public ActionResult Create()
        {
            //return View(new Guest
            //{
            //    EntryTime = DateTime.Now
            //} );
            return View();
        }

        // POST: Guests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Name,email,Reason,EntryTime")] Guest guest)
        {
            if (ModelState.IsValid)
            {
                db.GuestData.Add(guest);
                db.SaveChanges();
                using (StreamWriter writer = new StreamWriter(Server.MapPath("~/UserNotifications/") +  "Security.txt", true))
                {
                    await writer.WriteLineAsync("New Guest Request Created.Guest Name: " + guest.Name.ToString());
                }
                return RedirectToAction("Index");
            }

            return View(guest);
        }

        // GET: Guests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Guest guest = db.GuestData.Find(id);
            if (guest == null)
            {
                return HttpNotFound();
            }
            return View(guest);
        }

        // POST: Guests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,email,Reason,EntryTime")] Guest guest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(guest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(guest);
        }

        // GET: Guests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Guest guest = db.GuestData.Find(id);
            if (guest == null)
            {
                return HttpNotFound();
            }
            return View(guest);
        }

        // POST: Guests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var guestEscortMap = HttpContext.Application["guestEscortMap"] as Dictionary<string, int>;
            var escortQueue = HttpContext.Application["escortQueue"] as Queue<ApplicationUser>;

            Guest guest = db.GuestData.Find(id);
            string escortID = guest.EscortID;
            var releivedEscort = UserManager.FindById(escortID);

            db.GuestData.Remove(guest);
            db.SaveChanges();

            escortQueue.Enqueue(releivedEscort);
            guestEscortMap.Remove(escortID);
            HttpContext.Application.Lock();
            HttpContext.Application["guestEscortMap"] = guestEscortMap;
            HttpContext.Application["escortQueue"] = escortQueue;
            HttpContext.Application.UnLock();

            using (StreamWriter writer = new StreamWriter(Server.MapPath("~/UserNotifications/") +  "Admin.txt", true))
            {
                 writer.WriteLine("Guest Relieved! Guest Name: " + guest.Name.ToString() + "Escort ID:" + escortID );
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> DownloadGatePass(int id)
        {
            Guest guest = db.GuestData.Find(id);
            var user = UserManager.FindById(guest.EscortID);
            string fileData = "GATE PASS\r\n";
            fileData += "Guest Name: " + guest.Name + "\r\n";
            fileData += "Guest Email: " + guest.email + "\r\n";
            fileData += "Reason: " + guest.Reason + "\r\n";
            fileData += "Escort ID: " + guest.EscortID + "\r\n";
            fileData += "Escort Email: " + user.Email + "\r\n";
            using (StreamWriter writer = new StreamWriter(Server.MapPath("~/GatePasses/") + guest.email + ".txt"))
            {
                await writer.WriteLineAsync(fileData);
            }
           
            string filePath = Server.MapPath("~/GatePasses/") + guest.email + ".txt";
            return File(filePath, "text/plain", guest.email + ".txt");

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
