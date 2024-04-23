using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using FinalProject_MVC.Authorization;
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
            int currentUserId = (int)Session["CurrentUserId"];
            int currentCategoryId = (int)Session["CurrentCategoryId"];

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            if (currentCategoryId == 4 || currentCategoryId == 5)
            {
                return View(users);
            } 
            else
            {
                if (currentUserId == id)
                {
                    return View(users);
                }
                else
                {
                    return View("~/Views/Shared/Error.cshtml");
                }
            }
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            int currentCategoryId = (int)Session["CurrentCategoryId"];

            if (currentCategoryId == 4 || currentCategoryId == 5)
            {
                return View();
            } 
            else
            {
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,FirstName,LastName,Email,Password,CategoryId,Image")] Users users, string base64)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(users.FirstName) || string.IsNullOrWhiteSpace(users.LastName) ||
        string.IsNullOrWhiteSpace(users.Email) || string.IsNullOrWhiteSpace(users.Password))
                {
                    ViewBag.Error = "Please fill in all fields.";
                    return View(users);
                }

                // Validate First Name
                if (!System.Text.RegularExpressions.Regex.IsMatch(users.FirstName, "^[a-zA-Z]+$"))
                {
                    ViewBag.Error = "First name should contain only letters.";
                    return View(users);
                }

                // Validate Last Name
                if (!System.Text.RegularExpressions.Regex.IsMatch(users.LastName, "^[a-zA-Z]+$"))
                {
                    ViewBag.Error = "Last name should contain only letters.";
                    return View(users);
                }

                // Validate Email
                if (!IsValidEmail(users.Email))
                {
                    ViewBag.Error = "Invalid email format.";
                    return View(users);
                }

                // Validate Password
                if (users.Password.Length < 8 || users.Password.Length > 12)
                {
                    ViewBag.Error = "Password must be between 8 and 12 characters.";
                    return View(users);
                }

                if (!string.IsNullOrEmpty(base64))
                {
                    byte[] imageData = Convert.FromBase64String(base64);
                    users.Image = imageData;
                }

                db.Users.Add(users);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(users);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            int currentUserId = (int)Session["CurrentUserId"];
            int currentCategoryId = (int)Session["CurrentCategoryId"];

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Users user = db.Users.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            UserModel userModel = new UserModel
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                CategoryId = user.CategoryId,
                Image = user.Image
            };

            if (currentCategoryId == 4 || currentCategoryId == 5)
            {
                return View(userModel);
            }
            else
            {
                if (currentUserId == id)
                {
                    return View(userModel);
                }
                else
                {
                    return View("~/Views/Shared/Error.cshtml");
                }
            }
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserModel userModel, string base64)
        {

            if (string.IsNullOrWhiteSpace(userModel.FirstName) && string.IsNullOrWhiteSpace(userModel.LastName) &&
                string.IsNullOrWhiteSpace(userModel.Email) && string.IsNullOrWhiteSpace(userModel.Password))
            {
                ViewBag.Error = "Please fill in at least one field.";
                return View(userModel);
            }

            if (!string.IsNullOrWhiteSpace(userModel.FirstName) && !System.Text.RegularExpressions.Regex.IsMatch(userModel.FirstName, "^[a-zA-Z]+$"))
            {
                ViewBag.Error = "First name should contain only letters.";
                return View(userModel);
            }

            if (!string.IsNullOrWhiteSpace(userModel.LastName) && !System.Text.RegularExpressions.Regex.IsMatch(userModel.LastName, "^[a-zA-Z]+$"))
            {
                ViewBag.Error = "Last name should contain only letters.";
                return View(userModel);
            }

            if (!string.IsNullOrWhiteSpace(userModel.Email) && !IsValidEmail(userModel.Email))
            {
                ViewBag.Error = "Invalid email format.";
                return View(userModel);
            }

            if (!string.IsNullOrWhiteSpace(userModel.Password) && (userModel.Password.Length < 8 || userModel.Password.Length > 12))
            {
                ViewBag.Error = "Password must be between 8 and 12 characters.";
                return View(userModel);
            }

            if (ModelState.IsValid)
            {
                Users thisUser = db.Users.Find(userModel.UserId);

                if (thisUser == null)
                {
                    return HttpNotFound();
                }

                thisUser.FirstName = userModel.FirstName;
                thisUser.LastName = userModel.LastName;
                thisUser.Email = userModel.Email;
                thisUser.Password = userModel.Password;
                thisUser.CategoryId = userModel.CategoryId;

                if (!string.IsNullOrEmpty(base64))
                {
                    byte[] imageData = Convert.FromBase64String(base64);
                    thisUser.Image = imageData;
                }

                db.Entry(thisUser).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(userModel);
        }

        private bool IsValidEmail(string email)
        {
            // You can implement a more comprehensive email validation logic here if needed
            return !string.IsNullOrWhiteSpace(email) && System.Text.RegularExpressions.Regex.IsMatch(email, @"^\S+@\S+\.\S+$");
        }
        private bool IsValidName(string name)
        {
            // Regular expression to match names containing letters, hyphens, or spaces
            return System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z\s\-]+$");
        }
        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            int currentUserId = (int)Session["CurrentUserId"];
            int currentCategoryId = (int)Session["CurrentCategoryId"];

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            
            if (currentCategoryId == 4 || currentCategoryId == 5)
            {
                return View(users);
            }
            else
            {
                if (currentUserId == id)
                {
                    return View(users);
                }
                else
                {
                    return View("~/Views/Shared/Error.cshtml");
                }
            }

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

        public FileContentResult GetImage(int id)
        {
            id = (int)Session["CurrentUserId"];
            var user = db.Users.Find(id);
            if (user != null && user.Image != null)
            {
                return new FileContentResult(user.Image, "image/jpeg");
            }
            else
            {
                string placeholderImagePath = Server.MapPath("~/Pictures/placeholder.jpg");
                byte[] placeholderImageBytes = System.IO.File.ReadAllBytes(placeholderImagePath);
                return new FileContentResult(placeholderImageBytes, "image/jpeg");
            }
        }
    }
}
