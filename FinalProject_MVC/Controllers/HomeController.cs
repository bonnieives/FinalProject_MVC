using FinalProject_MVC.DAL;
using FinalProject_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;


namespace FinalProject_MVC.Controllers
{
    public class HomeController : Controller
    {
        private FinalProjectContext db = new FinalProjectContext();
        [Authorize]
        public ActionResult Index()
        {
           
            string userName = User.Identity.Name;

            Users user = db.Users.FirstOrDefault(u => u.Email == userName);


            if (User.Identity.IsAuthenticated)
            {
                if (user != null)
                {
                    Session["CurrentCategoryId"] = user.CategoryId;
                    Session["CurrentUserId"] = user.UserId;
                    Session["CurrentFirstName"] = user.FirstName;
                    Session["CurrentLastName"] = user.LastName;

                    int userId = (int)Session["CurrentUserId"];

                    if (user.CategoryId == 6)
                    {
                        var availableApartments = db.Apartments
                        .Include(a => a.Property)
                        .Where(a => a.StatusId == 1)
                        .ToList();

                        ViewBag.AvailableApartments = availableApartments;

                        return View(user);
                    }

                    if (user.CategoryId == 7)
                    {
                        var messageExists = db.Messages
                        .Include(a => a.Apartment)
                        .Where(a => a.Apartment.ManagerId == userId)
                        .ToList();

                        ViewBag.MessageExists = messageExists;

                        var appointmentExists = db.Appointments
                        .Include(a => a.Apartment)
                        .Where(a => a.Apartment.ManagerId == userId)
                        .ToList();

                        ViewBag.AppointmentExists = appointmentExists;

                        return View(user);
                    }
                }
                else
                {
                    TempData["Message"] = "Username or Password is wrong";
                    return Redirect("Login");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            return View(user);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Login()
        {
            ViewBag.Message = "Your Login page.";

            return View();
        }

        [HttpPost]
        public ActionResult Login(string userName, string password)
        {
            bool isAuthenticated = AuthenticateUser(userName, password);

            if (isAuthenticated)
            {
                FormsAuthentication.SetAuthCookie(userName, false);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid username or password.";
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session.Remove("CurrentUser");
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
        private bool AuthenticateUser(string userName, string password)
        {
            return true;
        }

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(string firstName, string lastName, string email, string password)
        {
            var newUser = new Users
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password,
                CategoryId = 6
            };
            db.Users.Add(newUser);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}