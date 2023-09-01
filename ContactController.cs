using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HearingAidWebSite.Repository;
using HearingAidWebSite.ViewModels;
using HearingAidWebSite.Data;
using HearingAidWebSite.Data.Models;
using Microsoft.EntityFrameworkCore;
using HearingAidWebSite.Data.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Text;

namespace HearingAidWebSite.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class ContactController : Controller
    {
        int ContactIds;
        string FileNamepics;
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly SignInManager<ApplicationUsers> signInManager;
        private readonly ICategoryRepository _icatr;
        private readonly ApplicationDbContext database;
        private readonly IHostingEnvironment environment;
        private readonly IContactRepository _iContactr;
        private readonly IEmailSender _iEmailSender;
        public ContactController(ApplicationDbContext context,
        IContactRepository iContactr, UserManager<ApplicationUsers> userManager, IHostingEnvironment _environment, IEmailSender iEmailSender)
        {
            database = context;
            _iContactr = iContactr;
            _userManager = userManager;
            environment = _environment;
            _iEmailSender = iEmailSender;
        }
        public IActionResult ContactListAdmin()
        {
            return View(_iContactr.ShowAllContact());
        }
        [HttpGet]
        public IActionResult ViewContact(int ContactId)
        {
            var model = _iContactr.ShowContactDetails(ContactId);

            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult ViewContact(int ContactId, Contact model)
        {
            if (ModelState.IsValid)
            {
                var qContact = database.Tbl_Contact.Where(c => c.ContactId == ContactId).SingleOrDefault();
            }
            return Json(new { status = "success", message = "تصویر با موفقیت آپلود شد", imagename = FileNamepics });
        }
        public IActionResult DeleteContact(int ContactId)
        {
            if (ContactId == null)
            {
                return NotFound();
            }
            var model = _iContactr.ShowContactDetails(ContactId);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }
        // POST: Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteContact(int ContactId,VmContact model)
        {
            if (ModelState.IsValid)
            {
              var  Contact = database.Tbl_Contact.Where(c => c.ContactId == ContactId).SingleOrDefault();
                database.Tbl_Contact.Remove(Contact);
                database.SaveChangesAsync();
            }
            //return RedirectToAction("DeleteALLGalleryProduct", "Galleries", new { productId = productId });
            return RedirectToAction("ContactListAdmin", "Contact");
        }
        public IActionResult ReplyToContact()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ReplyToContact(string to,string subject,string message)
        {
            StringBuilder body = new StringBuilder();
            //building the body of our email
            body.Append("<html><head> </head><body>");
            body.Append("<div style=' font-family: Arial; font-size: 14px; color: black;'>Hi,<br><br>");
            body.Append(message);
            body.Append("</div><br>");
            //Mail signature
            //body.Append(string.Format("<span style='font-size:11px;font-family: Arial;color:#40411E;'>{0} - {1} {2}</span><br>", MessageModel.adress, MessageModel.zip, MessageModel.city));
            //body.Append(string.Format("<span style='font-size:11px;font-family: Arial;color:#40411E;'>Mail: <a href=\"mailto:{0}\">{0}</a></span><br>", ConfigurationSMTP.from));
            //body.Append(string.Format("<span style='font-size:11px;font-family: Arial;color:#40411E;'>Tel: {0}</span><br>", MessageModel.phone));
            //body.Append(string.Format("<span style='font-size:11px;font-family: Arial;'><a href=\"web site\">{0}</a></span><br><br>", MessageModel.link));
            //body.Append(string.Format("<span style='font-size:11px; font-family: Arial;color:#40411E;'>{0}</span><br>", MessageModel.details));
            body.Append("</body></html>");
            _iEmailSender.SendEmailAsync( to,  subject, body.ToString());        
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}