using FinalProject_MVC.DAL;
using FinalProject_MVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FinalProject_MVC.Services;

namespace FinalProject_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthService _authService;
        private readonly FinalProjectContext _context;

        public HomeController(IAuthService authService, FinalProjectContext context)
        {
            _authService = authService;
            _context = context;
        }

        [Authorize]
        public ActionResult Index()
        {
            string userName = User.Identity.Name;
            Users user = _context.Users.FirstOrDefault(u => u.Email == userName);

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
                        var availableApartments = _context.Apartments
                            .Include(a => a.Property)
                            .Where(a => a.StatusId == 1)
                            .ToList();

                        ViewBag.AvailableApartments = availableApartments;

                        return View(user);
                    }

                    if (user.CategoryId == 7)
                    {
                        var messageExists = _context.Messages
                            .Include(a => a.Apartment)
                            .Where(a => a.Apartment.ManagerId == userId)
                            .ToList();

                        ViewBag.MessageExists = messageExists;

                        var appointmentExists = _context.Appointments
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
                    return RedirectToAction("Login");
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

        public ActionResult Login(string returnUrl)
        {
            ViewBag.Message = "Your Login page.";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Login(string userName, string password, string returnUrl)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == userName);

            if (user == null)
            {
                ViewBag.ErrorMessage = "Invalid username or password.";
                return View();
            }

            TempData["Category"] = user.Category;

            bool isAuthenticated = _authService.AuthenticateUser(userName, password, user.CategoryId);

            if (isAuthenticated)
            {
                DateTime expirationTime = DateTime.Now.AddSeconds(30);

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1,                              // Version
                    userName,                       // User's name (in this case, the username)
                    DateTime.Now,                   // Issue time
                    expirationTime,                 // Expiration time
                    false,                          // Persistent cookie
                    String.Empty                    // User data (optional)
                );

                string encryptedTicket = FormsAuthentication.Encrypt(ticket);

                string cookieName = "MyCustomAuthCookie";

                HttpCookie authCookie = new HttpCookie(cookieName, encryptedTicket);

                authCookie.Expires = expirationTime;

                Response.Cookies.Add(authCookie);

                FormsAuthentication.SetAuthCookie(userName, false);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
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

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(string firstName, string lastName, string email, string password, HttpPostedFileBase image)
        {
            FormsAuthentication.SignOut();

            // Check if the email is already registered
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == email);
            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "Email is already registered.";
                return View();
            }

            byte[] imageData = null;
            if (image != null)
            {
                using (var binaryReader = new BinaryReader(image.InputStream))
                {
                    imageData = binaryReader.ReadBytes(image.ContentLength);
                }
            }

            var newUser = new Users
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password,
                CategoryId = 6,
                Image = imageData
            };
            _context.Users.Add(newUser);
            _context.SaveChanges();

            bool isAuthenticated = _authService.AuthenticateUser(email, password, newUser.CategoryId);

            if (isAuthenticated)
            {
                DateTime expirationTime = DateTime.Now.AddSeconds(30);

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1,                              // Version
                    email,                          // User's name (in this case, the username)
                    DateTime.Now,                   // Issue time
                    expirationTime,                 // Expiration time
                    false,                          // Persistent cookie
                    String.Empty                    // User data (optional)
                );

                string encryptedTicket = FormsAuthentication.Encrypt(ticket);

                string cookieName = "MyCustomAuthCookie";

                HttpCookie authCookie = new HttpCookie(cookieName, encryptedTicket);

                authCookie.Expires = expirationTime;

                Response.Cookies.Add(authCookie);

                FormsAuthentication.SetAuthCookie(email, false);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid username or password.";
                return View();
            }
        }
    }
}
