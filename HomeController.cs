using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HearingAidWebSite.Models;
using HearingAidWebSite.Repository;
using HearingAidWebSite.ViewModels;
using HearingAidWebSite.Data;
using HearingAidWebSite.Data.Models;
using Microsoft.EntityFrameworkCore;
using HearingAidWebSite.Data.Services;
using System.Text;

namespace HearingAidWebSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext database;
        private readonly IBrandRepository _ibrandr;
        private readonly INewsRepository _iNewsr;
        private readonly IProductRepository _ipcr;
        private readonly ISliderRepository _isr;
        private readonly ICategoryRepository _icatr;
        private readonly IBrandSubCategoryRepository _ibrandSubCatr;
        private readonly IAccessoriesRepository _iaccessr;
        private readonly IConsultantRepository _iConsultr;
        private readonly IProductByShapeRepository _iproshaper;
        private readonly IEmailSender _iEmailSender;
        public HomeController(ApplicationDbContext context, IBrandRepository ibrandr, IProductByShapeRepository iproshaper,
            IProductRepository ipcr, INewsRepository iNewsr, IConsultantRepository iConsultr, IAccessoriesRepository iaccessr, ISliderRepository isr, ICategoryRepository icatr, IBrandSubCategoryRepository ibrandSubCatr, IEmailSender iEmailSender)
        {
            database = context;
            _ibrandr= ibrandr;
            _ipcr = ipcr;
            _isr = isr;
            _icatr = icatr;
            _ibrandSubCatr = ibrandSubCatr;
            _iConsultr = iConsultr;
            _ipcr = ipcr;
            _iproshaper = iproshaper;
            _iNewsr = iNewsr;
            _iEmailSender = iEmailSender;
        }
        //ApplicationDbContext database =new ApplicationDbContext();
        public IActionResult Main()
        {
            return View();
        }
        [Route("ProductList")]
        public IActionResult ProductList()
        {
            return View(_ipcr.ShowAllProducts());
        }
        [Route("ProductByShapeList")]
        public IActionResult ProductByShapeList()
        {
            return View(_iproshaper.ShowAllVmProductByShape());
        }
        [Route("ProductDetailByCategory/{catid:int}")]
        public IActionResult ProductDetailByCategory(int catid)
        {
            if (catid == 0)
            {
                return null;
            }
            return View(_ipcr.ShowNewProductsDetails(catid));
        }
        [Route("ProductDetails/{ProductId:int}")]
        public IActionResult ProductDetails(int ProductId)
        {
            if (ProductId == 0)
            {
                return null;
            }
            return View(_ipcr.ShowProductsDetails(ProductId));
        }
        [Route("BrandList")]
        public IActionResult BrandList()
        {
            return View(_ibrandr.ShowBrand());
        }
        [Route("BrandDetails/{BrandSubcatid:int}")]
        public IActionResult BrandDetails(int BrandSubcatid)
        {
            if (BrandSubcatid == 0)
            {
                return null;
            }
            ViewBag.BrandSubcatList = _ibrandSubCatr.ShowBrandSubCategoryList().Where(C=>C.BrandSubCategoryId== BrandSubcatid);
            return View(_ibrandSubCatr.ShowBrandSubCategoryDetails(BrandSubcatid));
        }
        [Route("Accessories")]
        public IActionResult Accessories()
        {
            return View();
        }
        [Route("BlogNews")]
        public IActionResult BlogNews()
        {
            return View(_iNewsr.ShowAllNews());
        }
        [Route("Consultant")]
        public IActionResult Consultant()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [Route("ContactUs")]
        public IActionResult ContactUs()
        {
            return View();
        }
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactUs(Contact contact)
        {
            if (ModelState.IsValid)
            {
                database.Tbl_Contact.Add(contact);
                database.SaveChanges();




                StringBuilder body = new StringBuilder();
                //building the body of our email
                body.Append("<html><head> </head><body>");
                body.Append("<div style=' font-family: Arial; font-size: 14px; color: black;'>Hi,<br><br>");
                body.Append(contact.Description);
                body.Append("</div><br>");
                //Mail signature
                //body.Append(string.Format("<span style='font-size:11px;font-family: Arial;color:#40411E;'>{0} - {1} {2}</span><br>", MessageModel.adress, MessageModel.zip, MessageModel.city));
                //body.Append(string.Format("<span style='font-size:11px;font-family: Arial;color:#40411E;'>Mail: <a href=\"mailto:{0}\">{0}</a></span><br>", ConfigurationSMTP.from));
                //body.Append(string.Format("<span style='font-size:11px;font-family: Arial;color:#40411E;'>Tel: {0}</span><br>", MessageModel.phone));
                //body.Append(string.Format("<span style='font-size:11px;font-family: Arial;'><a href=\"web site\">{0}</a></span><br><br>", MessageModel.link));
                //body.Append(string.Format("<span style='font-size:11px; font-family: Arial;color:#40411E;'>{0}</span><br>", MessageModel.details));
                body.Append("</body></html>");
                _iEmailSender.SendEmailAsync(contact.Email, "کلینیک سمعک کاشف", body.ToString());





                return RedirectToAction("ContactUs");
            }

            //ViewBag.CompanyID = new SelectList(db.Companies, "ID", "Title", contact.CompanyID);
            //ViewBag.CustomerID = new SelectList(db.Customers, "ID", "Title", contact.CustomerID);
            return View(contact);
        }












        [Route("AboutUs")]
        public IActionResult AboutUs()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
