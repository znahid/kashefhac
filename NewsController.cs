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
    public class NewsController : Controller
    {
        int NewsIds;
        string FileNamepics;
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly SignInManager<ApplicationUsers> signInManager;
        private readonly ICategoryRepository _icatr;
        private readonly ApplicationDbContext database;
        private readonly IHostingEnvironment environment;
        private readonly INewsRepository _iNewsr;

        private readonly IUploadfile _upload;
        public NewsController(ApplicationDbContext context, IProductByShapeRepository iproshaper, ICategoryRepository icatr,
     INewsRepository iNewsr, UserManager<ApplicationUsers> userManager, IHostingEnvironment _environment, IUploadfile upload)
        {
            database = context;
            _icatr = icatr;
            _iNewsr = iNewsr;
            _userManager = userManager;
            environment = _environment;
            _upload = upload;
        }
        public IActionResult NewsListAdmin()
        {
            return View(_iNewsr.ShowAllNews());
        }
        public IActionResult UploadFile(IEnumerable<IFormFile> files)
        {

            string filename = _upload.UploadFiles(files, "img\\BlogNews\\", "");
            return Json(new { status = "success", message = "تصویر با موفقیت آپلود شد.", imagename = filename });

        }
        [HttpGet]
        public IActionResult EditNews(int NewsId)
        {
            var model = _iNewsr.ShowNewsDetails(NewsId);
            ViewBag.date = (model.DateNews).ToPeString();

            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult EditNews(int NewsId, VmNews model, string NewsDates, string newIndexImage, string currentImageName)
        {
            if (ModelState.IsValid)
            {
                var qNews = database.Tbl_BlogNews.Where(c => c.NewsId == NewsId).SingleOrDefault();

                if (newIndexImage != null)
                {
                    qNews.DefaultPic = newIndexImage;
                }
                else
                {
                    qNews.DefaultPic = currentImageName;
                }
                try
                {
                    //FileNamepics = Guid.NewGuid().ToString().Replace("-", "") + defaultpicNews.FileName.ToLower();
                    //var uploadspics = Path.Combine(environment.WebRootPath, "img\\Accessories");
                    //using (var fileStream = new FileStream(Path.Combine(uploadspics, FileNamepics), FileMode.Create))
                    //{
                    //    defaultpicNews.CopyToAsync(fileStream);
                    //}
                    //qNews.DefaultPic = FileNamepics;
                    qNews.NewsNameFA = model.NewsNameFA;
                    qNews.NewsNameEN = model.NewsNameEN;
                    qNews.Description = model.Description;

                    DateTime EditTime = DataTimeEx.PersinToMiladiDate(DataTimeEx.ConvertNumberToEnglish(NewsDates));

                    //DateTime startstime = Models.ExMethod.DataTimeEx.PersinToMiladiDate(DateTime.Parse(Models.ExMethod.DataTimeEx.ConvertNumberToEnglish(sttime)));
                    qNews.DateNews = EditTime;
                    qNews.SummeryDescription = model.SummeryDescription;
                    database.Update(qNews);
                    database.SaveChanges();
                }
                catch
                {
                    throw;
                }

            }

            return RedirectToAction("NewsListAdmin", "News");
        }
        public IActionResult DeleteNews(int NewsId)
        {
            if (NewsId == null)
            {
                return NotFound();
            }
            var model = _iNewsr.ShowNewsDetails(NewsId);
            var DeleteImage = Path.Combine(environment.WebRootPath, "img\\BlogNews\\" + model.DefaultPic);
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
        public IActionResult DeleteNews(VmNews model, int NewsId, IFormFile defaultpicNews)
        {
            var newss = database.Tbl_BlogNews.Where(c => c.NewsId == NewsId).SingleOrDefault();
            NewsId = newss.NewsId;
            database.Tbl_BlogNews.Remove(newss);
            database.SaveChanges();
            //return RedirectToAction("DeleteALLGalleryProduct", "Galleries", new { productId = productId });
            return RedirectToAction("NewsListAdmin", "News");
        }
        [HttpGet]
        public IActionResult CreateNews()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateNews(VmNews model, int NewsId, string DefaultPic, string NewsDates)
        {
            if (ModelState.IsValid)
            {
                model.DefaultPic = DefaultPic;
                try
                {
                    //string FileNamepics = Guid.NewGuid().ToString().Replace("-", "") + defaultpicNews.FileName.ToLower();
                    //var uploadspics = Path.Combine(environment.WebRootPath, "img\\Accessories");
                    //using (var fileStream = new FileStream(Path.Combine(uploadspics, FileNamepics), FileMode.Create))
                    //{
                    //    await defaultpicNews.CopyToAsync(fileStream);
                    //}
                    BlogNews News = new BlogNews
                    {
                        NewsNameFA = model.NewsNameFA,
                        NewsNameEN = model.NewsNameEN,
                        DefaultPic = model.DefaultPic,
                        Description = model.Description,
                        DateNews = DataTimeEx.PersinToMiladiDate(DataTimeEx.ConvertNumberToEnglish(NewsDates)),
                        SummeryDescription = model.SummeryDescription,
                    };
                    database.Tbl_BlogNews.Add(News);
                    database.SaveChanges();
                    NewsIds = News.NewsId;
                }
                catch
                {
                    throw;
                }
                //return RedirectToAction("CreateGallerProduct","Galleries");
                return RedirectToAction("NewsListAdmin", "News");
            }
            return View(model);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}