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

namespace HearingAidWebSite.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class ConsultantController : Controller
    {
        int ConsultantIds;
        string FileNamepics;
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly SignInManager<ApplicationUsers> signInManager;
        private readonly ICategoryRepository _icatr;
        private readonly ApplicationDbContext database;
        private readonly IHostingEnvironment environment;
        private readonly IConsultantRepository _iConsultr;
        private readonly IUploadfile _upload;
        public ConsultantController(ApplicationDbContext context, IProductByShapeRepository iproshaper,
        IConsultantRepository iConsultr, UserManager<ApplicationUsers> userManager, IHostingEnvironment _environment, IUploadfile upload)
        {
            database = context;  
            _iConsultr = iConsultr;
            _userManager = userManager;
            environment = _environment;
            _upload = upload;
        }
        public IActionResult ConsultantListAdmin()
        {
            return View(_iConsultr.ShowAllConsultant());
        }
        public IActionResult UploadFile(IEnumerable<IFormFile> files)
        {

            string filename = _upload.UploadFiles(files, "img\\Consultant\\", "");
            return Json(new { status = "success", message = "تصویر با موفقیت آپلود شد.", imagename = filename });

        }
        [HttpGet]
        public IActionResult EditConsultant(int ConsultantId)
        {
            var model = _iConsultr.ShowConsultantDetails(ConsultantId);

            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult EditConsultant(int ConsultantId, Consultant model, string newIndexImage, string currentImageName)
        {
            if (ModelState.IsValid)
            {
                var qConsultant = database.Tbl_Consultant.Where(c => c.ConsultantId == ConsultantId).SingleOrDefault();

                if (newIndexImage != null)
                {
                    qConsultant.DefaultPic = newIndexImage;
                }
                else
                {
                    qConsultant.DefaultPic = currentImageName;
                }
                try
                {

                    //FileNamepics = Guid.NewGuid().ToString().Replace("-", "") + defaultpicConsultant.FileName.ToLower();
                    //var uploadspics = Path.Combine(environment.WebRootPath, "img\\Consultant");
                    //using (var fileStream = new FileStream(Path.Combine(uploadspics, FileNamepics), FileMode.Create))
                    //{
                    //    defaultpicConsultant.CopyToAsync(fileStream);
                    //}
                    //qConsultant.DefaultPic = FileNamepics;
                    qConsultant.ConsultantNameFA = model.ConsultantNameFA;
                    qConsultant.ConsultantNameEN = model.ConsultantNameEN;
                    qConsultant.Description = model.Description;
                    qConsultant.SummeryDescription = model.SummeryDescription;
                    database.Update(qConsultant);
                    database.SaveChanges();
                }
                catch
                {
                    throw;
                }
                //RedirectToAction("ProductListAdmin");
            }
            return RedirectToAction("ConsultantListAdmin", "Consultant");
            //return Json(new { status = "success", message = "تصویر با موفقیت آپلود شد", imagename = FileNamepics });
            ///*return View()*/
            ;
        }
               public IActionResult DeleteConsultant(int ConsultantId)
        {
            if (ConsultantId == null)
            {
                return NotFound();
            }
            var model = _iConsultr.ShowConsultantDetails(ConsultantId);
            var DeleteImage = Path.Combine(environment.WebRootPath, "img\\Consultant\\" + model.DefaultPic);
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
        public IActionResult DeleteConsultant(VmConsultant model, int ConsultantId, IFormFile defaultpicConsultant)
        {
            Consultant Consultant = database.Tbl_Consultant.Where(c => c.ConsultantId == ConsultantId).SingleOrDefault();
            database.Tbl_Consultant.Remove(Consultant);
            database.SaveChanges();
            //return RedirectToAction("DeleteALLGalleryProduct", "Galleries", new { productId = productId });
            return RedirectToAction("ConsultantListAdmin", "Consultant");
        }
        [HttpGet]
        public IActionResult CreateConsultant()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateConsultant(VmConsultant model, int ConsultantId, string DefaultPic)
        {
            if (ModelState.IsValid)
            {            
                    model.DefaultPic = DefaultPic;
  
                try
                {
                    //string FileNamepics = Guid.NewGuid().ToString().Replace("-", "") + defaultpicConsultant.FileName.ToLower();
                    //var uploadspics = Path.Combine(environment.WebRootPath, "img\\Consultant");
                    //using (var fileStream = new FileStream(Path.Combine(uploadspics, FileNamepics), FileMode.Create))
                    //{
                    //    await defaultpicConsultant.CopyToAsync(fileStream);
                    //}
                    Consultant Consultant = new Consultant
                    {
                        ConsultantNameFA = model.ConsultantNameFA,
                        ConsultantNameEN = model.ConsultantNameEN,
                        DefaultPic = model.DefaultPic,
                        Description = model.Description,
                        SummeryDescription = model.SummeryDescription,
                    };
                    database.Tbl_Consultant.Add(Consultant);
                    database.SaveChanges();
                    ConsultantIds = Consultant.ConsultantId;
                }
                catch
                {
                    throw;
                }
                //return RedirectToAction("CreateGallerProduct","Galleries");
                return RedirectToAction("ConsultantListAdmin", "Consultant");
            }
            return View(model);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}