using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinalProject_MVC.DAL;
using FinalProject_MVC.Models;

namespace FinalProject_MVC.Controllers
{
    public class ContractsController : Controller
    {
        private FinalProjectContext db = new FinalProjectContext();

        // GET: Contracts
        public ActionResult Index()
        {
            int currentUserId = (int)Session["CurrentUserId"];
            int currentCategoryId = (int)Session["CurrentCategoryId"];

            var contracts = new List<Contracts>();

            if (currentCategoryId == 4)
            {
                contracts = db.Contracts
                    .Include(c => c.Apartment)
                    .Include(c => c.Tenant)
                    .ToList();
            } 
            else if (currentCategoryId == 5)
            {
                contracts = db.Contracts
                    .Where(c => c.Apartment.OwnerId == currentUserId)
                    .Include(c => c.Apartment)
                    .Include(c => c.Tenant)
                    .ToList();
            }
            else if (currentCategoryId == 6)
            {
                contracts = db.Contracts
                    .Where(c => c.TenantId == currentUserId)
                    .Include(c => c.Apartment)
                    .Include(c => c.Tenant)
                    .ToList();
            }
            else if (currentCategoryId == 7)
            {
                contracts = db.Contracts
                    .Where(c => c.Apartment.ManagerId == currentUserId)
                    .Include(c => c.Apartment)
                    .Include(c => c.Tenant)
                    .ToList();
            }

            return View(contracts);
        }

        // GET: Contracts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contracts contracts = db.Contracts.Find(id);
            if (contracts == null)
            {
                return HttpNotFound();
            }
            return View(contracts);
        }

        // GET: Contracts/Create
        public ActionResult Create()
        {
            int currentUserId = (int)Session["CurrentUserId"];
            int currentCategoryId = (int)Session["CurrentCategoryId"];
            
            if (currentCategoryId == 4)
            {
                ViewBag.ApartmentId = new SelectList(db.Apartments.Where(u => u.Status.StatusId == 1).
                    ToList()
                    .Select(a => new
                    {
                        ApartmentId = a.ApartmentId,
                        Address = $"{a.ApartmentNumber} - {a.Property.CivicNumber} {a.Property.Address}, {a.Property.Zip}"
                    }), "ApartmentId", "Address");
                                ViewBag.TenantId = new SelectList(db.Users.Where(u => u.CategoryId == (int)Category.Tenant)
                                                              .Select(u => new { u.UserId, FullName = u.FirstName + " " + u.LastName })
                                                              .AsEnumerable(), "UserId", "FullName");
            } 
            else if (currentCategoryId == 5)
            {
                ViewBag.ApartmentId = new SelectList(db.Apartments.Where(u => u.Status.StatusId == 1 && u.OwnerId == currentUserId).
                    ToList()
                    .Select(a => new
                    {
                        ApartmentId = a.ApartmentId,
                        Address = $"{a.ApartmentNumber} - {a.Property.CivicNumber} {a.Property.Address}, {a.Property.Zip}"
                    }), "ApartmentId", "Address");
                                ViewBag.TenantId = new SelectList(db.Users.Where(u => u.CategoryId == (int)Category.Tenant)
                                                              .Select(u => new { u.UserId, FullName = u.FirstName + " " + u.LastName })
                                                              .AsEnumerable(), "UserId", "FullName");
            }

          
            return View();
        }

        // POST: Contracts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ContractModel model)
        {
            if (ModelState.IsValid)
            {
                var apartment = db.Apartments.Find(model.ApartmentId);

                Contracts newContract = new Contracts
                {
                    InitialDate = model.InitialDate,
                    FinalDate = model.FinalDate,
                    Value = model.Value,
                    Payday = model.Payday,
                    TenantId = model.TenantId,
                    ApartmentId = model.ApartmentId
                };

                if (apartment != null)
                {
                    apartment.StatusId = 2;
                    db.SaveChanges();
                }

                db.Contracts.Add(newContract);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Contracts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Contracts contract = db.Contracts.Find(id);

            if (contract == null)
            {
                return HttpNotFound();
            }

            ContractModel model = new ContractModel
            {
                ContractId = contract.ContractId,
                InitialDate = contract.InitialDate,
                FinalDate = contract.FinalDate,
                Value = contract.Value,
                Payday = contract.Payday,
                TenantId = contract.TenantId,
                ApartmentId = contract.ApartmentId
            };

            ViewBag.TenantId = new SelectList(db.Users.Where(u => u.CategoryId == (int)Category.Tenant)
                                                      .Select(u => new { u.UserId, FullName = u.FirstName + " " + u.LastName }), "UserId", "FullName", contract.TenantId);
            var apartments = db.Apartments
                .Select(a => new
                {
                    a.ApartmentId,
                    Address = a.ApartmentNumber + " - " + a.Property.CivicNumber + " " + a.Property.Address + ", " + a.Property.Zip
                })
                .ToList();

            var thisApartment = db.Apartments
                .Where(a => a.ApartmentId == model.ApartmentId)
                .Include(a => a.Property)
                .FirstOrDefault();


            ViewBag.CurrentApartment = thisApartment;
            ViewBag.ApartmentId = new SelectList(apartments, "ApartmentId", "Address", model.ApartmentId);
            return View(model);
        }

        // POST: Contracts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ContractModel model)
        {
            if (ModelState.IsValid)
            {
                Contracts thisContract = new Contracts
                {
                    ContractId = model.ContractId,
                    InitialDate = model.InitialDate,
                    FinalDate = model.FinalDate,
                    Value = model.Value,
                    Payday = model.Payday,
                    TenantId = model.TenantId,
                    ApartmentId = model.ApartmentId
                };

                db.Entry(thisContract).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.TenantId = new SelectList(db.Users.Where(u => u.CategoryId == (int)Category.Tenant)
                                                      .Select(u => new { u.UserId, FullName = u.FirstName + " " + u.LastName }), "UserId", "FullName", model.TenantId);
            var apartments = db.Apartments
                .Select(a => new
                {
                    a.ApartmentId,
                    Address = a.ApartmentNumber + " - " + a.Property.CivicNumber + " " + a.Property.Address + ", " + a.Property.Zip
                })
                .ToList();

            ViewBag.ApartmentId = new SelectList(apartments, "ApartmentId", "Address", model.ApartmentId);                                          
            return View(model);
        }

        // GET: Contracts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contracts contracts = db.Contracts.Find(id);
            if (contracts == null)
            {
                return HttpNotFound();
            }
            return View(contracts);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contracts contracts = db.Contracts.Find(id);
            db.Contracts.Remove(contracts);
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
