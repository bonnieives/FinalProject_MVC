using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinalProject_MVC.DAL;

namespace FinalProject_MVC.Models
{
    public class ApartmentsController : Controller
    {
        private FinalProjectContext db = new FinalProjectContext();

        // GET: Apartments
        public ActionResult Index()
        {
            var apartments = db.Apartments.Include(a => a.Manager).Include(a => a.Owner).Include(a => a.Property).Include(a => a.Status);
            return View(apartments.ToList());
        }

        // GET: Apartments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Apartments apartments = db.Apartments.Find(id);
            if (apartments == null)
            {
                return HttpNotFound();
            }
            return View(apartments);
        }

        // GET: Apartments/Create
        public ActionResult Create()
        {
            ViewBag.ManagerId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.OwnerId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.PropertyId = new SelectList(db.Properties, "PropertyId", "Address");
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "StatusId");
            return View();
        }

        // POST: Apartments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ApartmentId,ApartmentNumber,OwnerId,ManagerId,PropertyId,StatusId")] Apartments apartments)
        {
            if (ModelState.IsValid)
            {
                db.Apartments.Add(apartments);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ManagerId = new SelectList(db.Users, "UserId", "FirstName", apartments.ManagerId);
            ViewBag.OwnerId = new SelectList(db.Users, "UserId", "FirstName", apartments.OwnerId);
            ViewBag.PropertyId = new SelectList(db.Properties, "PropertyId", "Address", apartments.PropertyId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "StatusId", apartments.StatusId);
            return View(apartments);
        }

        // GET: Apartments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Apartments apartments = db.Apartments.Find(id);
            if (apartments == null)
            {
                return HttpNotFound();
            }
            ViewBag.ManagerId = new SelectList(db.Users, "UserId", "FirstName", apartments.ManagerId);
            ViewBag.OwnerId = new SelectList(db.Users, "UserId", "FirstName", apartments.OwnerId);
            ViewBag.PropertyId = new SelectList(db.Properties, "PropertyId", "Address", apartments.PropertyId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "StatusId", apartments.StatusId);
            return View(apartments);
        }

        // POST: Apartments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ApartmentId,ApartmentNumber,OwnerId,ManagerId,PropertyId,StatusId")] Apartments apartments)
        {
            if (ModelState.IsValid)
            {
                db.Entry(apartments).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ManagerId = new SelectList(db.Users, "UserId", "FirstName", apartments.ManagerId);
            ViewBag.OwnerId = new SelectList(db.Users, "UserId", "FirstName", apartments.OwnerId);
            ViewBag.PropertyId = new SelectList(db.Properties, "PropertyId", "Address", apartments.PropertyId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "StatusId", apartments.StatusId);
            return View(apartments);
        }

        // GET: Apartments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Apartments apartments = db.Apartments.Find(id);
            if (apartments == null)
            {
                return HttpNotFound();
            }
            return View(apartments);
        }

        // POST: Apartments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Apartments apartments = db.Apartments.Find(id);
            db.Apartments.Remove(apartments);
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
