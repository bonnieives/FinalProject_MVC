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
    public class UsersController : Controller
    {
        private FinalProjectContext db = new FinalProjectContext();

        // GET: Users
        public ActionResult Index()
        {
            int currentUserId = (int)Session["CurrentUserId"];
            int currentCategoryId = (int)Session["CurrentCategoryId"];

            var users = new List<Users>();

            if (currentCategoryId == 4  || currentCategoryId == 5)
            {
                users = db.Users.ToList();
            }
            else if (currentCategoryId == 6 || currentCategoryId == 7)
            {
                var user = db.Users.FirstOrDefault(x => x.UserId == currentUserId);
                users = new List<Users> { user };
            }
            return View(users);
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,FirstName,LastName,Email,Password,CategoryId")] Users users)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(users);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(users);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            UserModel userModel = new UserModel();

            userModel.FirstName = users.FirstName;
            userModel.LastName = users.LastName;
            userModel.UserId = users.UserId;
            userModel.CategoryId = users.CategoryId;
            userModel.Email = users.Email;
            userModel.Password = users.Password;

            if (users == null)
            {
                return HttpNotFound();
            }
            return View(userModel);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserModel users)
        {
            if (ModelState.IsValid)
            {
                Users thisUser = new Users
                {
                    UserId = users.UserId,
                    FirstName = users.FirstName,
                    LastName = users.LastName,
                    Email = users.Email,
                    Password = users.Password,
                    CategoryId = users.CategoryId
                };

                db.Entry(thisUser).State = EntityState.Modified;
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(users);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            
            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            int currentUserId = (int)Session["CurrentUserId"];

            Users users = db.Users.Find(id);
            db.Users.Remove(users);
            db.SaveChanges();

            if (currentUserId == id)
            {
                return RedirectToAction("Index", "Home");
            }
                      
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
