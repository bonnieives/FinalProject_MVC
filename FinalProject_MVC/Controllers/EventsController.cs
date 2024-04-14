using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinalProject_MVC.DAL;
using FinalProject_MVC.Models;

namespace FinalProject_MVC.Controllers
{
    public class EventsController : Controller
    {
        private FinalProjectContext db = new FinalProjectContext();

        // GET: Events
        public ActionResult Index()
        {
            int currentUserId = (int)Session["CurrentUserId"];
            int currentCategoryId = (int)Session["CurrentCategoryId"];

            var events = new List<Events>();

            if (currentCategoryId == 4)
            {
                events = db.Events
                .Include(a => a.Apartment)
                .Include(a => a.Apartment.Property)
                .Include(a => a.Apartment.Manager)
                .Include(a => a.Apartment.Owner)
                .OrderBy(a => a.EventId)
                .ToList();
            }
            else if (currentCategoryId == 5)
            {
                events = db.Events
                .Include(a => a.Apartment)
                .Include(a => a.Apartment.Property)
                .Include(a => a.Apartment.Manager)
                .Include(a => a.Apartment.Owner)
                .Where(a => a.Apartment.OwnerId == currentUserId)
                .OrderBy(a => a.EventId)
                .ToList();
            }
            else if (currentCategoryId == 7)
            {
                events = db.Events
                .Include(a => a.Apartment)
                .Include(a => a.Apartment.Property)
                .Include(a => a.Apartment.Manager)
                .Include(a => a.Apartment.Owner)
                .Where(a => a.Apartment.ManagerId == currentUserId)
                .OrderBy(a => a.EventId)
                .ToList();
            }

            return View(events);
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Events events = db.Events
                .Include(a => a.Apartment)
                .Include(a => a.Apartment.Property)
                .Include(a => a.Apartment.Manager)
                .Include(a => a.Apartment.Owner)
                .FirstOrDefault(a => a.EventId == id);

            if (events == null)
            {
                return HttpNotFound();
            }
            return View(events);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            int currentUserId = (int)Session["CurrentUserId"];
            int currentCategoryId = (int)Session["CurrentCategoryId"];

            var apartments = db.Apartments
                .Where(a => a.StatusId == 1 && a.ManagerId == currentUserId)
                .Include(a => a.Property)
                .Select(a => new SelectListItem
                {
                    Value = a.ApartmentId.ToString(),
                    Text = a.Property.CivicNumber + " " + a.Property.Address + ", " + a.Property.Zip + ", Apartment Number: " + a.ApartmentNumber
                })
                .ToList();

            ViewBag.Apartments = apartments;

            var events = new Events();

            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Events events)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(events);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            int currentUserId = (int)Session["CurrentUserId"];
            int currentCategoryId = (int)Session["CurrentCategoryId"];

            var apartments = db.Apartments
                .Where(a => a.StatusId == 1 && a.ManagerId == currentUserId)
                .Include(a => a.Property)
                .Select(a => new SelectListItem
                {
                    Value = a.ApartmentId.ToString(),
                    Text = a.Property.CivicNumber + " " + a.Property.Address + ", " + a.Property.Zip + ", Apartment Number: " + a.ApartmentNumber
                })
                .ToList();

            return View(events);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            int currentUserId = (int)Session["CurrentUserId"];
            int currentCategoryId = (int)Session["CurrentCategoryId"];

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Events events = db.Events.Find(id);
            if (events == null)
            {
                return HttpNotFound();
            }

            var apartments = db.Apartments
                .Where(a => a.StatusId == 1 && a.ManagerId == currentUserId)
                .Include(a => a.Property)
                .Select(a => new SelectListItem
                {
                    Value = a.ApartmentId.ToString(),
                    Text = a.Property.CivicNumber + " " + a.Property.Address + ", " + a.Property.Zip + ", Apartment Number: " + a.ApartmentNumber
                })
                .ToList();

            ViewBag.Apartments = apartments;

            EventModel model = new EventModel
            {
                EventId = events.EventId,
                ApartmentId = events.ApartmentId,
                Description = events.Description,
            };

            return View(model);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EventModel model)
        {
            if (ModelState.IsValid)
            {
                var existingEvent = db.Events.Find(model.EventId);

                if (existingEvent != null)
                {
                    existingEvent.EventId = model.EventId;
                    existingEvent.ApartmentId = model.ApartmentId;
                    existingEvent.Description = model.Description;

                    db.Entry(existingEvent).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    return HttpNotFound();
                }
            }
            else
            {
                return View(model);
            }
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Events events = db.Events.Find(id);
            if (events == null)
            {
                return HttpNotFound();
            }
            return View(events);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Events events = db.Events.Find(id);
            db.Events.Remove(events);
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
