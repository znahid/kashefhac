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
using HearingAidWebSite.PublicClass;
using System.Globalization;

namespace HearingAidWebSite.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class AccessoriesController : Controller
    {
        int AccessoryIds;
        string FileNamepics;
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly SignInManager<ApplicationUsers> signInManager;
        private readonly ICategoryRepository _icatr;
        private readonly ApplicationDbContext database;
        private readonly IHostingEnvironment environment;
        private readonly IAccessoriesRepository _iaccessr;
        private readonly IUploadfile _upload;
        public AccessoriesController(ApplicationDbContext context, IProductByShapeRepository iproshaper, ICategoryRepository icatr,
      IAccessoriesRepository iaccessr, UserManager<ApplicationUsers> userManager, IHostingEnvironment _environment, IUploadfile upload)
        {
            database = context;
            _icatr = icatr;
            _iaccessr = iaccessr;     
            _userManager = userManager;
            environment = _environment;
            _upload = upload;
        }
        public IActionResult AccessoriesListAdmin()
        {
            return View(_iaccessr.ShowAllAccessory());
        }
        public  IActionResult UploadFile(IEnumerable<IFormFile> files)
        {
            string filename = _upload.UploadFiles(files, "img\\Accessories\\", "");
            return Json(new { status = "success", message = "تصویر با موفقیت آپلود شد.", imagename = filename });
        }
        [HttpGet]
        public IActionResult EditAccessory(int AccessoryId)
        {
            var model = _iaccessr.ShowAccessoryDetails(AccessoryId);

          ViewBag.date = model.DateAccessory.ToPeString(); //DataTimeEx.MiladiToShamsi(model.DateAccessory);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult EditAccessory(int AccessoryId, VmAccessory model, string AccesoryDates, string newIndexImage, string currentImageName)
        {
            if (ModelState.IsValid)
            {
                var qAccessory = database.Tbl_Accessories.Where(c => c.AccessoryId == AccessoryId).SingleOrDefault();
                if (newIndexImage != null)
                {
                    qAccessory.DefaultPic = newIndexImage;
                }
                else
                {
                    qAccessory.DefaultPic = currentImageName;
                }             
                try
                {               
                    //FileNamepics = Guid.NewGuid().ToString().Replace("-", "") + defaultpicAccessory.FileName.ToLower();
                    //var uploadspics = Path.Combine(environment.WebRootPath, "img\\Accessories");
                    //using (var fileStream = new FileStream(Path.Combine(uploadspics, FileNamepics), FileMode.Create))
                    //{
                    //    defaultpicAccessory.CopyToAsync(fileStream);
                    //}
                    //qAccessory.DefaultPic = FileNamepics;
                    qAccessory.AccessoryNameFA = model.AccessoryNameFA;
                    qAccessory.AccessoryNameEN = model.AccessoryNameEN;
                  DateTime EditTime  = DataTimeEx.PersinToMiladiDate(DataTimeEx.ConvertNumberToEnglish(AccesoryDates));
                    //DateTime startstime = Models.ExMethod.DataTimeEx.PersinToMiladiDate(DateTime.Parse(Models.ExMethod.DataTimeEx.ConvertNumberToEnglish(sttime)));
                    qAccessory.DateAccessory = EditTime;
                    qAccessory.Description = model.Description;
                    qAccessory.SummeryDescription = model.SummeryDescription;
                    database.Update(qAccessory);
                    database.SaveChanges();
                }
                catch
                {
                    throw;
                }
       
            }
    
           return RedirectToAction("AccessoriesListAdmin", "Accessories");
        }
  
        public IActionResult DeletePicAccessory(int AccessoryId)
        {
            if (AccessoryId == null)
            {
                return NotFound();
            }
            var model = _iaccessr.ShowAccessoryDetails(AccessoryId);
            var DeleteImage = Path.Combine(environment.WebRootPath, "img\\Accessories\\" + model.DefaultPic);
            if (System.IO.File.Exists(DeleteImage))
            {
                System.IO.File.Delete(DeleteImage);
            }
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }
        // POST: Products/Delete/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult DeletePicAccessory(VmAccessory model, int AccessoryId, IFormFile defaultpicAccessory)
        {
            Accessories Accessory = database.Tbl_Accessories.Where(c => c.AccessoryId == AccessoryId).SingleOrDefault();
            AccessoryId = Accessory.AccessoryId;
            database.Tbl_Accessories.Remove(Accessory);
            database.SaveChanges();
            //return RedirectToAction("DeleteALLGalleryProduct", "Galleries", new { productId = productId });
            return RedirectToAction("AccessoriesListAdmin", "Accessories");
        }
        [HttpGet]
        public IActionResult CreateAccessory()
        {
        
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAccessory(VmAccessory model,string AccesoryDates, int AccessoryId, string DefaultPic, string currentImageName)
        {
            if (ModelState.IsValid)
            {

                model.DefaultPic = DefaultPic;
                //if (DefaultPic != null)
                //    model.DefaultPic = DefaultPic;
                //else
                //{
                //    model.DefaultPic = currentImageName;
                //}
                try
                {
                    //string FileNamepics = Guid.NewGuid().ToString().Replace("-", "") + defaultpicAccessory.FileName.ToLower();
                    //var uploadspics = Path.Combine(environment.WebRootPath, "img\\Accessories");
                    //using (var fileStream = new FileStream(Path.Combine(uploadspics, FileNamepics), FileMode.Create))
                    //{
                    //    await defaultpicAccessory.CopyToAsync(fileStream);
                    //}
                    Accessories Accessory = new Accessories
                    {
                        AccessoryNameFA = model.AccessoryNameFA,
                        AccessoryNameEN = model.AccessoryNameEN,
                        DefaultPic = model.DefaultPic,
                        Description = model.Description,
                        DateAccessory = DataTimeEx.PersinToMiladiDate(AccesoryDates),
                        SummeryDescription = model.SummeryDescription,

                    };
                    database.Tbl_Accessories.Add(Accessory);
                    database.SaveChanges();
                    AccessoryIds = Accessory.AccessoryId;
                }
                catch
                {
                    throw;
                }
                return RedirectToAction("AccessoriesListAdmin", "Accessories");
            }
            return View(model);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}