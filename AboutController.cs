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
        public class AboutController : Controller
        {
            int AboutIds;
            string FileNamepics;
            private readonly UserManager<ApplicationUsers> _userManager;
            private readonly SignInManager<ApplicationUsers> signInManager;
            private readonly ICategoryRepository _icatr;
            private readonly ApplicationDbContext database;
            private readonly IHostingEnvironment environment;
            private readonly IAboutRepository _iAboutr;
            public AboutController(ApplicationDbContext context,
            IAboutRepository iAboutr, UserManager<ApplicationUsers> userManager, IHostingEnvironment _environment)
            {
                database = context;
                _iAboutr = iAboutr;
                _userManager = userManager;
                environment = _environment;
            }
            public IActionResult AboutListAdmin()
            {
                return View(_iAboutr.ShowAllAbout());
            }
            [HttpGet]
            public IActionResult EditAbout(int AboutId)
            {
                var model =_iAboutr.ShowAboutDetails(AboutId);

                if (model == null)
                {
                    return NotFound();
                }
                return View(model);
            }
            [HttpPost]
            public IActionResult EditAbout(int AboutId, About model)
            {
                if (ModelState.IsValid)
                {
                    var qAbout = database.Tbl_About.Where(c => c.AboutId == AboutId).SingleOrDefault();
                    try
                    {
                        //FileNamepics = Guid.NewGuid().ToString().Replace("-", "") + defaultpicAbout.FileName.ToLower();
                        //var uploadspics = Path.Combine(environment.WebRootPath, "img\\About");
                        //using (var fileStream = new FileStream(Path.Combine(uploadspics, FileNamepics), FileMode.Create))
                        //{
                        //    defaultpicAbout.CopyToAsync(fileStream);
                        //}
                        qAbout.AboutId = model.AboutId;
                        qAbout.DateAbout = model.DateAbout;
                        qAbout.Description= model.Description;
                        qAbout.IsShowAbout = true;
                        qAbout.SummeryDescription = model.SummeryDescription;
                        database.SaveChanges();
                    }
                    catch
                    {
                        throw;
                    }
                    //RedirectToAction("AboutListAdmin");
                }
            return RedirectToAction("AboutListAdmin", "About");
            //return Json(new { status = "success", message = "تصویر با موفقیت آپلود شد", imagename = FileNamepics });
            /*return View()*/
            ;
            }
            public IActionResult DeleteAbout(int AboutId)
            {
                if (AboutId == null)
                {
                    return NotFound();
                }
            var model = _iAboutr.ShowAboutDetails(AboutId);
                //var DeleteImage = Path.Combine(environment.WebRootPath, "img\\About\\" + model.DefaultPic);
                //if (System.IO.File.Exists(DeleteImage))
                //{
                //    System.IO.File.Delete(DeleteImage);
                //}
                if (model == null)
                {
                    return NotFound();
                }
                return View(model);
            }
            // POST: Abouts/Delete/5
            [HttpPost]
            //[ValidateAntiForgeryToken]
            public IActionResult DeleteAbout(VmAbout model, int AboutId)
            {

                About About = database.Tbl_About.Where(c => c.AboutId == AboutId).SingleOrDefault();
                database.Tbl_About.Remove(About);
                database.SaveChanges();
                //return RedirectToAction("DeleteALLGalleryAbout", "Galleries", new { GalleryAboutId = GalleryAboutId });
                return RedirectToAction("AboutListAdmin", "About");
            }  
        

         public async Task<IActionResult> EditGalleryAbout()
        {
            var gallery = await database.Tbl_GalleryAbout.ToListAsync();
            if (gallery == null)
            {
                return NotFound();
            }
            List<VmGalleryAbout> lstGalleryAbout = new List<VmGalleryAbout>();
            foreach (var item in gallery)
            {
                VmGalleryAbout g = new VmGalleryAbout(); // از ویو مدل هم میتوان استفاده کرد
                g.TitlePic = item.TitlePic;
                g.GalleryAboutId = item.GalleryAboutId;
                g.PictureName = item.PictureName;
                g.Description = item.Description;

                lstGalleryAbout.Add(g);
            }
            return View(lstGalleryAbout);
        }
        [HttpPost]
        public async Task<IActionResult> EditGalleryAbout( IFormFile[] GalleryAbout, VmGalleryAbout[] qGallery, int GalleryAboutId)
        {
            if (GalleryAbout[0] == null)
            {
                return Redirect("About/AboutListAdmin");
            }
            else
            {
                //var qGallery = database.Tbl_GalleryAbout.Where(a => a.GalleryAboutId == GalleryAboutId).FirstOrDefault();
                List<GalleryAbout> lstgallery = new List<GalleryAbout>();
                foreach (var item in GalleryAbout)
                {
                    int i = GalleryAbout.Count();
                    string FileNamepic = Guid.NewGuid().ToString().Replace("-", "") + item.FileName.ToLower();
                    var uploadspic = Path.Combine(environment.WebRootPath, "img\\Aboutgallery\\");
                    using (var fileStream = new FileStream(Path.Combine(uploadspic, FileNamepic), FileMode.Create))
                    {
                        await item.CopyToAsync(fileStream);
                    }
                    GalleryAbout g = new GalleryAbout();
                    //g.DefaultPicProduct = false;
                    //g.GalleryAboutId= item[0].GalleryAboutId;
                    g.PictureName = FileNamepic;
                    //g.TitlePic = qGallery[0].TitlePic;
                    //g.Description = qGallery[0].Description;
                    lstgallery.Add(g);
                }
                database.Tbl_GalleryAbout.AddRange(lstgallery.AsEnumerable());
                await database.SaveChangesAsync();
                //return RedirectToAction(nameof(ProductListAdmin),"Product");
                //return RedirectToAction("AboutListAdmin", "About");
                return RedirectToAction(nameof(EditGalleryAbout), "About");
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteGalleryAbout(string[] GalleryAboutId, string[] ChekboxDelete, int IdGalleryAbout)
        {
            if (GalleryAboutId.Count() == 0 || ChekboxDelete.Count() == 0)
                return RedirectToAction(nameof(AboutListAdmin), "About");
            foreach (var item in GalleryAboutId.ToList())
            {
                foreach (var item1 in ChekboxDelete)

                {
                    if (item == item1)
                    {
                        var qdelete = await database.Tbl_GalleryAbout.SingleOrDefaultAsync(c => c.GalleryAboutId == Convert.ToInt32(item1));
                        var DeleteImage = Path.Combine(environment.WebRootPath, "img\\Aboutgallery\\" + qdelete.PictureName);
                        if (System.IO.File.Exists(DeleteImage))
                        {
                            System.IO.File.Delete(DeleteImage);
                        }
                        if (qdelete != null)
                            database.Tbl_GalleryAbout.Remove(qdelete);
                        await database.SaveChangesAsync();
                    }              
                }
            }
            return RedirectToAction(nameof(EditGalleryAbout), "About");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}