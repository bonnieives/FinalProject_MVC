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
using Newtonsoft.Json.Linq;

namespace FinalProject_MVC.Controllers
{
    public class AppointmentsController : Controller
    {
        private FinalProjectContext db = new FinalProjectContext();

        // GET: Appointments
        public ActionResult Index()
        {
            int currentUserId = (int)Session["CurrentUserId"];
            int currentCategoryId = (int)Session["CurrentCategoryId"];

            var appointments = new List<Appointments>();

            if (currentUserId != null && currentCategoryId == 4)
            {
                appointments = db.Appointments
                                    .Include(a => a.Apartment)
                                    .Include(a => a.Apartment.Property)
                                    .Include(a => a.User)
                                    .ToList();

            } else if (currentUserId != null && currentCategoryId == 5)
            {
                appointments = db.Appointments
                                    .Include(a => a.Apartment)
                                    .Include(a => a.Apartment.Property)
                                    .Include(a => a.User)
                                    .Where(a => a.Apartment.OwnerId == currentUserId)
                                    .ToList();

            } else if (currentUserId != null && currentCategoryId == 6)
            {
                appointments = db.Appointments
                                    .Include(a => a.Apartment)
                                    .Include(a => a.Apartment.Property)
                                    .Include(a => a.User)
                                    .Where(a => a.SenderId == currentUserId)
                                    .ToList();

            } else
            {
                appointments = db.Appointments
                                    .Include(a => a.Apartment)
                                    .Include(a => a.Apartment.Property)
                                    .Include(a => a.User)
                                    .Where(a => a.Apartment.ManagerId == currentUserId)
                                    .ToList();

            }

            if (appointments == null)
            {
                appointments = new List<Appointments> { new Appointments { AppointmentId = 0, DateTime = DateTime.Now, Apartment = new Apartments { ApartmentNumber = 0 } } };
            }

            return View(appointments);
        }

        // GET: Appointments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Appointments appointments = db.Appointments.Include(a => a.User).Include(a => a.Apartment).FirstOrDefault(a => a.AppointmentId == id);

            if (appointments == null)
            {
                return HttpNotFound();
            }

            ViewBag.UserName = appointments.User.FirstName;

            return View(appointments);
        }

        // GET: Appointments/Create
        public ActionResult Create()
        {
            int currentUserId = (int)Session["CurrentUserId"];
            int currentCategoryId = (int)Session["CurrentCategoryId"];

            if (currentCategoryId == 7)
            {
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
            }
            else
            {
                var apartments = db.Apartments
                    .Where(a => a.StatusId == 1)
                    .Include(a => a.Property)
                    .Select(a => new SelectListItem
                    {
                        Value = a.ApartmentId.ToString(),
                        Text = a.Property.CivicNumber + " " + a.Property.Address + ", " + a.Property.Zip + ", Apartment Number: " + a.ApartmentNumber
                    })
                    .ToList();
                ViewBag.Apartments = apartments;
            }

            var user = db.Users
                .Where(u => u.CategoryId == 6)
                .Select(u => new SelectListItem
                {
                    Value = u.UserId.ToString(),
                    Text = u.FirstName + " " + u.LastName
                })
                .ToList();

            ViewBag.Tenants = user;

            var appointments = new Appointments();

            return View(appointments);
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Appointments appointments)
        {
            if (ModelState.IsValid)
            {
                int userId = (int)Session["CurrentUserId"];
                int currentCategoryId = (int)Session["CurrentCategoryId"];


                if (currentCategoryId == 7)
                {
                    int senderId = appointments.User.UserId;

                    appointments.SenderId = senderId;

                    var thisUser = db.Users
                        .FirstOrDefault(u => u.UserId == userId);

                    
                        
                }
                else
                {
                    var thisUser = db.Users
                        .FirstOrDefault(u => u.UserId == userId);

                    appointments.User = thisUser;
                }

                db.Appointments.Add(appointments);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var apartments = db.Apartments
                .Where(a => a.StatusId == 1)
                .Include(a => a.Property)
                .Select(a => new SelectListItem
                {
                    Value = a.ApartmentId.ToString(),
                    Text = a.Property.CivicNumber.ToString() + " " + a.Property.Address + ", " + a.Property.Zip + ", Apartment Number: " + a.ApartmentNumber
                })
                .ToList();

            ViewBag.Apartments = apartments;

            var user = db.Users
                .Where(u => u.CategoryId == 6)
                .Select(u => new SelectListItem
                {
                    Value = u.UserId.ToString(),
                    Text = u.FirstName + " " + u.LastName
                })
                .ToList();

            ViewBag.Tenants = user;

            return View(appointments);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        // GET: Appointments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Appointments appointments = db.Appointments.Find(id);
            if (appointments == null)
            {
                return HttpNotFound();
            }

            var apartments = db.Apartments
                .Where(a => a.StatusId == 1)
                .Include(a => a.Property)
                .Select(a => new SelectListItem
                {
                    Value = a.ApartmentId.ToString(),
                    Text = a.Property.CivicNumber.ToString() + " " + a.Property.Address + ", " + a.Property.Zip + ", Apartment Number: " + a.ApartmentNumber
                })
                .ToList();

            ViewBag.Apartments = apartments;

            AppointmentModel model = new AppointmentModel
            {
                AppointmentId = appointments.AppointmentId,
                DateTime = appointments.DateTime,
                ApartmentId = appointments.ApartmentId,
                SenderId = appointments.SenderId,
                Confirmation = appointments.Confirmation,
            };

            return View(model);
        }


        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AppointmentModel model)
        {
            if (ModelState.IsValid)
            {
                var existingAppointment = db.Appointments.Find(model.AppointmentId);

                if (existingAppointment != null)
                {
                    existingAppointment.DateTime = model.DateTime;
                    existingAppointment.ApartmentId = model.ApartmentId;
                    existingAppointment.SenderId = model.SenderId;
                    existingAppointment.Confirmation = model.Confirmation;

                    db.Entry(existingAppointment).State = EntityState.Modified;
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

        // GET: Appointments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointments appointments = db.Appointments.Find(id);
            if (appointments == null)
            {
                return HttpNotFound();
            }
            return View(appointments);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Appointments appointments = db.Appointments.Find(id);
            db.Appointments.Remove(appointments);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
