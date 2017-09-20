using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Collections;
using System;
using sampleMVC.Models;
using System.IO;
using System.Data.Entity;

namespace IdentitySample.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesAdminController : Controller
    {
        public RolesAdminController()
        {
        }

        private ApplicationDbContext db = new ApplicationDbContext();

        public RolesAdminController(ApplicationUserManager userManager,
            ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
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

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Roles/
        public ActionResult Index()
        {
            return View(RoleManager.Roles);
        }

        //
        // GET: /Roles/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            // Get the list of Users in this Role
            var users = new List<ApplicationUser>();

            // Get the list of Users in this Role
            foreach (var user in UserManager.Users.ToList())
            {
                if (await UserManager.IsInRoleAsync(user.Id, role.Name))
                {
                    users.Add(user);
                }
            }

            ViewBag.Users = users;
            ViewBag.UserCount = users.Count();
            return View(role);
        }

        //
        // GET: /Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        public async Task<ActionResult> Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole(roleViewModel.Name);
                var roleresult = await RoleManager.CreateAsync(role);
                if (!roleresult.Succeeded)
                {
                    ModelState.AddModelError("", roleresult.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        //
        // GET: /Roles/Edit/Admin
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            RoleViewModel roleModel = new RoleViewModel { Id = role.Id, Name = role.Name };
            return View(roleModel);
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Name,Id")] RoleViewModel roleModel)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleManager.FindByIdAsync(roleModel.Id);
                role.Name = roleModel.Name;
                await RoleManager.UpdateAsync(role);
                return RedirectToAction("Index");
            }
            return View();
        }

        //
        // GET: /Roles/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        //
        // POST: /Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id, string deleteUser)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var role = await RoleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return HttpNotFound();
                }
                IdentityResult result;
                if (deleteUser != null)
                {
                    result = await RoleManager.DeleteAsync(role);
                }
                else
                {
                    result = await RoleManager.DeleteAsync(role);
                }
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }


        public async Task AssignEscort(int guestID)
        {
            string roleName = "Employee";
            var role = await RoleManager.FindByNameAsync(roleName);

            var guestEscortMap = HttpContext.Application["guestEscortMap"] as Dictionary<string, int>;
            var escortQueue = HttpContext.Application["escortQueue"] as Queue<ApplicationUser>;
            int firstAssignmentCall = (int)HttpContext.Application["firstAssignment"];

            // Get the list of Users in this Role
            if(firstAssignmentCall == 0)
            {
                foreach (var user in UserManager.Users.ToList())
                {
                    if (await UserManager.IsInRoleAsync(user.Id, role.Name))
                    {
                        //users.Add(user);
                        escortQueue.Enqueue(user);
                    }
                }
                firstAssignmentCall = 1;
            }

            Guest guest = db.GuestData.Find(guestID);
            var upcomingEscort = escortQueue.Dequeue();
            //Assigned Escort
            guestEscortMap.Add(upcomingEscort.Id, guestID);

            guest.EscortID = upcomingEscort.Id;
            db.SaveChanges();


            using (StreamWriter writer = new StreamWriter(Server.MapPath("~/UserNotifications/") + upcomingEscort.Id.ToString() + ".txt", true))
            {
                await writer.WriteLineAsync("You have an Escort Job! Guest Name: " + guest.Name.ToString());
            }

            HttpContext.Application.Lock();
            HttpContext.Application["guestEscortMap"] = guestEscortMap;
            HttpContext.Application["escortQueue"] = escortQueue;
            HttpContext.Application["firstAssignment"] = firstAssignmentCall;
            HttpContext.Application.UnLock();
        }

        [AllowAnonymous]
        public async Task FreeEscort(string escortID)
        {

            var releivedEscort = UserManager.FindById(escortID);
            var guestEscortMap = HttpContext.Application["guestEscortMap"] as Dictionary<string, int>;
            var escortQueue = HttpContext.Application["escortQueue"] as Queue<ApplicationUser>;

            Guest guest = db.GuestData.Find(guestEscortMap[escortID]);
            guest.EscortID = "Unassigned";
            db.SaveChanges();

            escortQueue.Enqueue(releivedEscort);
            guestEscortMap.Remove(escortID);

            using (StreamWriter writer = new StreamWriter(Server.MapPath("~/UserNotifications/") +  "Admin.txt", true))
            {
                await writer.WriteLineAsync("Escort has requested to transfer his job! Guest Name: " + guest.Name.ToString() + ", Escort ID: " + escortID);
            }

            HttpContext.Application.Lock();
            HttpContext.Application["guestEscortMap"] = guestEscortMap;
            HttpContext.Application["escortQueue"] = escortQueue;
            HttpContext.Application.UnLock();
        }
    }
}
