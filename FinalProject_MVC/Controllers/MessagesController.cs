using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using FinalProject_MVC.Authorization;
using FinalProject_MVC.DAL;
using FinalProject_MVC.Models;

namespace FinalProject_MVC.Controllers
{
    public class MessagesController : Controller
    {
        private FinalProjectContext db = new FinalProjectContext();

        // GET: Messages
        public ActionResult Index()
        {
            int currentUserId = (int)Session["CurrentUserId"];
            int currentCategoryId = (int)Session["CurrentCategoryId"];

            var messages = new List<Messages>();

            if (currentCategoryId == 4)
            {
                messages = db.Messages
                    .Include(a => a.Apartment)
                    .Include(a => a.Apartment.Property)
                    .Include(a => a.Sender)
                    .ToList();
            } 
            else if (currentCategoryId == 5)
            {
                messages = db.Messages
                    .Include(a => a.Apartment)
                    .Include(a => a.Apartment.Property)
                    .Include(a => a.Sender)
                    .Where(a => a.Apartment.OwnerId == currentUserId)
                    .ToList();
            }
            else if (currentCategoryId == 6)
            {
                messages = db.Messages
                    .Include(a => a.Apartment)
                    .Include(a => a.Apartment.Property)
                    .Include(a => a.Sender)
                    .Where(a => a.SenderId == currentUserId || a.ReceiverId == currentUserId)
                    .ToList();
            }
            else if (currentCategoryId == 7)
            {
                messages = db.Messages
                    .Include(a => a.Apartment)
                    .Include(a => a.Apartment.Property)
                    .Include(a => a.Sender)
                    .Where(a => a.SenderId == currentUserId || a.ReceiverId == currentUserId)
                    .ToList();
            }

            if (messages == null)
            {
                messages = new List<Messages> { new Messages { MessageId = 0, Description = "", Apartment = new Apartments { ApartmentNumber = 0 } } };
            }

            return View(messages);
        }
        // GET: Messages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Messages messages = db.Messages
                .Include(a => a.Sender)
                .Include(a => a.Apartment)
                .Include(a => a.Apartment.Manager)
                .FirstOrDefault(a => a.MessageId == id);

            int currentUserId = (int)Session["CurrentUserId"];
            int currentCategoryId = (int)Session["CurrentCategoryId"];

            var ownerMessageId = db.Messages.
                Include(c => c.Apartment).
                Where(c => c.Apartment.OwnerId == currentUserId).
                Select(c => c.MessageId).
                ToList();

            var tenantManagerMessageId = db.Messages.
                Where(c => c.SenderId == currentUserId || c.ReceiverId == currentUserId).
                Select(c => c.MessageId).
                ToList();

            if (messages == null)
            {
                return HttpNotFound();
            }

            ViewBag.UserName = messages.Sender.FirstName;

            if (currentCategoryId != 4)
            {
                if (id.HasValue && ownerMessageId.Contains(id.Value))
                {
                    return View(messages);
                }
                else if (id.HasValue && tenantManagerMessageId.Contains(id.Value))
                {
                    return View(messages);
                }
                else
                {
                    return View("~/Views/Shared/Error.cshtml");
                }
            }

            return View(messages);
        }

        // GET: Messages/Create
        [CategoryAuthorize(6, 7)]
        public ActionResult Create(int id)
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

            var senderNames = db.Messages
                .Where(a => a.MessageId == id)
                .Select(a => new {
                    FirstName = a.Sender.FirstName,
                    LastName = a.Sender.LastName
                })
                .FirstOrDefault();

            if (senderNames != null)
            {
                ViewBag.SenderFirstName = senderNames.FirstName;
                ViewBag.SenderLastName = senderNames.LastName;
            }

            

            ViewBag.Apartments = apartments;

            if (id != -1)
            {
                var existingMessage = db.Messages.FirstOrDefault(m => m.MessageId == id);

                if (existingMessage != null)
                {
                    var newMessage = new Messages
                    {
                        SenderId = existingMessage.ReceiverId,
                        ReceiverId = existingMessage.SenderId,
                        ApartmentId = existingMessage.ApartmentId,
                        MessageId = id
                    };

                    return View(newMessage);
                }
            }

            var messages = new Messages();
            return View(messages);
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CategoryAuthorize(6, 7)]
        public ActionResult Create([Bind(Include = "MessageId,Description,ApartmentId")] Messages message)
        {
            if (ModelState.IsValid)
            {
                int userId = (int)Session["CurrentUserId"];
                int currentCategoryId = (int)Session["CurrentCategoryId"];

                if (currentCategoryId == 6)
                {
                    var selectedApartment = db.Apartments.FirstOrDefault(a => a.ApartmentId == message.ApartmentId);
                    
                    if (selectedApartment != null)
                    {
                        var managerId = selectedApartment.ManagerId;
                        message.ReceiverId = managerId;

                        var selectedManager = db.Users.FirstOrDefault(u => u.UserId == managerId);
                        message.SenderId = userId;
                        db.Messages.Add(message);
                    }
                }

                if (currentCategoryId == 7)
                {
                    var selectedMessage = db.Messages.FirstOrDefault(a => a.MessageId == message.MessageId);

                    if (selectedMessage != null)
                    {
                        var SenderId = selectedMessage.SenderId;
                        var ReceiverId = selectedMessage.ReceiverId;
                        var ApartmentId = selectedMessage.ApartmentId;

                        message.ReceiverId = SenderId;
                        message.SenderId = ReceiverId;
                        message.ApartmentId = ApartmentId;
                        db.Messages.Add(message);
                        db.SaveChanges();
                    }
                }

                
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

            return View(message);
        }

        // GET: Messages/Edit/5
        [CategoryAuthorize(6, 7)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Messages message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }

            int currentUserId = (int)Session["CurrentUserId"];
            int currentCategoryId = (int)Session["CurrentCategoryId"];

            if (message.SenderId != currentUserId)
            {
                return View("~/Views/Shared/Error.cshtml");
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

            MessageModel model = new MessageModel
            {
                MessageId = message.MessageId,
                Description = message.Description,
                ApartmentId = message.ApartmentId,
                SenderId = message.SenderId,
            };

            return View(model);
        }




        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CategoryAuthorize(6, 7)]
        public ActionResult Edit(MessageModel model)
        {
            if (ModelState.IsValid)
            {
                var existingMessage = db.Messages.Find(model.MessageId);

                if (existingMessage != null)
                {
                    existingMessage.Description = model.Description;
                    existingMessage.ApartmentId = model.ApartmentId;
                    existingMessage.SenderId = model.SenderId;

                    db.Entry(existingMessage).State = EntityState.Modified;
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

        // GET: Messages/Delete/5
        [CategoryAuthorize(6, 7)]
        public ActionResult Delete(int? id)
        {
            int currentUserId = (int)Session["CurrentUserId"];

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Messages message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }

            if (message.SenderId == currentUserId)
            {
                return View(message);
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CategoryAuthorize(6, 7)]
        public ActionResult DeleteConfirmed(int id)
        {
            Messages messages = db.Messages.Find(id);
            db.Messages.Remove(messages);
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
