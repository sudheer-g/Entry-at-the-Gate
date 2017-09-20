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

namespace sampleMVC.Controllers
{
    [Authorize]
    public class IdentityProofsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: IdentityProofs
        public ActionResult Index()
        {
            return View(db.IdProofData.ToList());
        }

        // GET: IdentityProofs/Details/5
        public ActionResult Details(string email)
        {
            if (email == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IdentityProof identityProof = db.IdProofData.Find(email);
            if (identityProof == null)
            {
                return HttpNotFound();
            }
            return View(identityProof);
        }

        // GET: IdentityProofs/Create
        public ActionResult Create(string email)
        {
            IdentityProof identityProof = db.IdProofData.Find(email);
            ViewBag.email = email;
            if (identityProof == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Details", new { email = email });
            }
        }

        // POST: IdentityProofs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "email,Name,ContactNumber,IdProofType,IdProofNumber,ReportTime")] IdentityProof identityProof, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if(upload != null && upload.ContentLength > 0)
                {
                    identityProof.Photo = System.IO.Path.GetFileName(upload.FileName).ToString();
                }
                db.IdProofData.Add(identityProof);
                db.SaveChanges();
                return RedirectToRoute(new { controller = "Guests", action = "Index" });
            }

            return View(identityProof);
        }

        // GET: IdentityProofs/Edit/5
        public ActionResult Edit(string email)
        {
            if (email == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IdentityProof identityProof = db.IdProofData.Find(email);
            if (identityProof == null)
            {
                return HttpNotFound();
            }
            return View(identityProof);
        }

        // POST: IdentityProofs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "email,Name,ContactNumber,IdProofType,IdProofNumber,ReportTime,Photo")] IdentityProof identityProof)
        {
            if (ModelState.IsValid)
            {
                db.Entry(identityProof).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(identityProof);
        }

        // GET: IdentityProofs/Delete/5
        public ActionResult Delete(string email)
        {
            if (email == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IdentityProof identityProof = db.IdProofData.Find(email);
            if (identityProof == null)
            {
                return HttpNotFound();
            }
            return View(identityProof);
        }

        // POST: IdentityProofs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string email)
        {
            IdentityProof identityProof = db.IdProofData.Find(email);
            db.IdProofData.Remove(identityProof);
            db.SaveChanges();
            return RedirectToAction("Index");
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
