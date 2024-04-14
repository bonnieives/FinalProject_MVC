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
    public class PropertiesController : Controller
    {
        private FinalProjectContext db = new FinalProjectContext();

        // GET: Properties
        public ActionResult Index()
        {
            int currentUserId = (int)Session["CurrentUserId"];
            int currentCategoryId = (int)Session["CurrentCategoryId"];

            var apartments = new List<Apartments>();

            if (currentCategoryId == 4)
            {
                apartments = db.Apartments
                    .Include(a => a.Property)
                    .Include(a => a.Manager)
                    .Include(a => a.Owner)
                    .ToList();
            }
            else if (currentCategoryId == 5)
            {
                apartments = db.Apartments
                    .Include(a => a.Property)
                    .Include(a => a.Owner)
                    .Where(a => a.OwnerId == currentUserId)
                    .OrderBy(a => a.ApartmentId)
                    .ToList();
            }
            else if (currentCategoryId == 6)
            {
                apartments = db.Apartments
                    .Include(a => a.Property)
                    .Where(a => a.StatusId == 1)
                    .OrderBy(a => a.ApartmentId)
                    .ToList();
            }
            else
            {
                apartments = db.Apartments
                    .Include(a => a.Property)
                    .Where(a => a.ManagerId == currentUserId)
                    .OrderBy(a => a.ApartmentId)
                    .ToList();
            }


            return View(apartments);
        }

        // GET: Properties/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Apartments apartment = db.Apartments
                .Where(a => a.ApartmentId == id)
                .Include(a => a.Property)
                .FirstOrDefault();

            apartment = db.Apartments.FirstOrDefault(a => a.ApartmentId == id);
            ViewBag.Apartments = apartment.ApartmentNumber;

            if (apartment == null)
            {
                return HttpNotFound();
            }
            return View(apartment);
        }

        // GET: Properties/Create
        public ActionResult Create()
        {

            var apartments = db.Apartments
                .Include(a => a.Property)
                .ToList();

            Properties thisProperty = new Properties();

            var provinces = Enum.GetValues(typeof(Province)).Cast<Province>().ToList();

            ViewBag.OwnerList = new SelectList(db.Users.Where(u => u.CategoryId == (int)Category.Owner), "UserId", "FirstName");
            ViewBag.ManagerList = new SelectList(db.Users.Where(u => u.CategoryId == (int)Category.Manager), "UserId", "FirstName");
            ViewBag.StatusList = new SelectList(db.Status.ToList(), "StatusId", "StatusDescription");

            return View();
        }

        // POST: Properties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PropertyApartmentModel model)
        {
            if (ModelState.IsValid)
            {

                int apartmentNumber = model.Apartment.ApartmentNumber;
                int ownerId = model.Apartment.OwnerId;
                int managerId = model.Apartment.ManagerId;
                int propertyId = model.Apartment.PropertyId;
                int statusId = model.Apartment.StatusId;

                int civicNumber = model.Property.CivicNumber;
                string address = model.Property.Address;
                string city = model.Property.City;
                string zip = model.Property.Zip;
                Province? province = model.Property.Province;

                ViewBag.OwnerList = new SelectList(db.Users.Where(u => u.CategoryId == (int)Category.Owner), "UserId", "FirstName");
                ViewBag.ManagerList = new SelectList(db.Users.Where(u => u.CategoryId == (int)Category.Manager), "UserId", "FirstName");
                ViewBag.StatusList = new SelectList(db.Status.ToList(), "StatusId", "StatusDescription");


                var property = new Properties
                {
                    CivicNumber = civicNumber,
                    Address = address,
                    City = city,
                    Zip = zip,
                    Province = province
                };

                db.Properties.Add(property);

                db.SaveChanges();

                var apartment = new Apartments
                {
                    ApartmentNumber = apartmentNumber,
                    OwnerId = ownerId,
                    ManagerId = managerId,
                    PropertyId = property.PropertyId,
                    StatusId = statusId
                };


                
                db.Apartments.Add(apartment);

                db.SaveChanges();

            }

            int currentUserId = (int)Session["CurrentUserId"];
            int currentCategoryId = (int)Session["CurrentCategoryId"];

            var apartments = new List<Apartments>();

            apartments = db.Apartments
                .Include(a => a.Property)
                .Include(a => a.Manager)
                .Include(a => a.Owner)
                .Where(a => a.ManagerId == currentUserId || a.OwnerId == currentUserId)
                .OrderBy(a => a.ApartmentId)
                .ToList();

            return View("Index", apartments);
        }

        // GET: Properties/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Apartments apartment = db.Apartments
                .Where(a => a.ApartmentId == id)
                .Include(a => a.Property)
                .FirstOrDefault();

            apartment = db.Apartments.FirstOrDefault(a => a.ApartmentId == id);

            Properties property = new Properties();

            if (property == null)
            {
                return HttpNotFound();
            }

            ViewBag.OwnerList = new SelectList(db.Users.Where(u => u.CategoryId == (int)Category.Owner), "UserId", "FirstName");
            ViewBag.ManagerList = new SelectList(db.Users.Where(u => u.CategoryId == (int)Category.Manager), "UserId", "FirstName");
            ViewBag.StatusList = new SelectList(db.Status.ToList(), "StatusId", "StatusDescription");

            PropertyModel model = new PropertyModel
            {
                PropertyId = apartment.Property.PropertyId,
                CivicNumber = apartment.Property.CivicNumber,
                Address = apartment.Property.Address,
                City = apartment.Property.City,
                Zip = apartment.Property.Zip,
                Province = apartment.Property.Province
            };

            ApartmentModel apModel = new ApartmentModel
            {
                ApartmentId = apartment.ApartmentId,
                ApartmentNumber = apartment.ApartmentNumber,
                OwnerId = apartment.OwnerId,
                ManagerId = apartment.ManagerId,
                PropertyId = apartment.PropertyId,
                StatusId = apartment.StatusId,
            };

            PropertyApartmentModel propertyApartmentModel = new PropertyApartmentModel
            {
                Property = model,
                Apartment = apModel,
            };
            return View(apartment);
        }

        // POST: Properties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Apartments model)
        {
            if (ModelState.IsValid)
            {
                Properties propertyToUpdate = db.Properties.Find(model.Property.PropertyId);
                Apartments apartmentToUpdate = db.Apartments.FirstOrDefault(a => a.PropertyId == model.Property.PropertyId);

                propertyToUpdate.CivicNumber = model.Property.CivicNumber;
                propertyToUpdate.Address = model.Property.Address;
                propertyToUpdate.City = model.Property.City;
                propertyToUpdate.Zip = model.Property.Zip;
                propertyToUpdate.Province = model.Property.Province;

                apartmentToUpdate.ApartmentNumber = model.ApartmentNumber;
                apartmentToUpdate.OwnerId = model.OwnerId;
                apartmentToUpdate.ManagerId = model.ManagerId;
                apartmentToUpdate.StatusId = model.StatusId;

                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving changes: " + ex.Message);
                }
            }

            ViewBag.OwnerList = new SelectList(db.Users.Where(u => u.CategoryId == (int)Category.Owner), "UserId", "FirstName");
            ViewBag.ManagerList = new SelectList(db.Users.Where(u => u.CategoryId == (int)Category.Manager), "UserId", "FirstName");
            ViewBag.StatusList = new SelectList(db.Status.ToList(), "StatusId", "StatusDescription");

            return View(model);
        }

        // GET: Properties/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Properties properties = db.Properties.Find(id);
            if (properties == null)
            {
                return HttpNotFound();
            }
            return View(properties);
        }

        // POST: Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Properties properties = db.Properties.Find(id);
            var apartment = db.Apartments.FirstOrDefault(a => a.PropertyId == id);
            db.Apartments.Remove(apartment);
            db.Properties.Remove(properties);
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
