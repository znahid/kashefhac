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
    public class GalleriesController : Controller
    {
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly SignInManager<ApplicationUsers> signInManager;
        private readonly ICategoryRepository _icatr;
        private readonly ApplicationDbContext database;
        private readonly IHostingEnvironment environment;
        private readonly IProductRepository _ipcr;
        public GalleriesController(ApplicationDbContext context, IProductByShapeRepository iproshaper, ICategoryRepository icatr,
            IProductRepository ipcr, UserManager<ApplicationUsers> userManager, IHostingEnvironment _environment)
        {
            database = context;
            _icatr = icatr;
            _ipcr = ipcr;
            _ipcr = ipcr;
            _userManager = userManager;
            environment = _environment;
        }
      public IActionResult Index()
        {
            return View();
        }
        // GET: Galleries/Edit/5
        public async Task<IActionResult> EditGalleryProduct(int? productId)
        {
            if (productId == null)
            {
                return NotFound();
            }
            var gallery = await database.Tbl_Gallery.Where(m => m.ProductId_FK == productId).ToListAsync();
            if (gallery == null)
            {
                return NotFound();
            }
            List<Gallery> lstGallery = new List<Gallery>();
            foreach (var item in gallery)
            {
                Gallery g = new Gallery(); // از ویو مدل هم میتوان استفاده کرد
                g.TitlePic = item.TitlePic;
                g.PictureName = item.PictureName;
                g.DefaultPicProduct = item.DefaultPicProduct;
                g.ProductId_FK = item.ProductId_FK;
                g.GalleryId = item.GalleryId;
                lstGallery.Add(g);
            }
            return View(lstGallery);
        }
        [HttpPost]
        public async Task<IActionResult> EditGalleryProduct( IFormFile[] GalleryProdouct, int Idproduct)
        {
            if (GalleryProdouct[0] == null)
            {
               return Redirect("Product/ProductListAdmin");
            }
            else
            {
                var qProduct = database.Tbl_Products.Where(a => a.ProductId == Idproduct).FirstOrDefault();
                List<Gallery> lstgallery = new List<Gallery>();
                foreach (var item in GalleryProdouct)
                {
                    string FileNamepic = Guid.NewGuid().ToString().Replace("-", "") + item.FileName.ToLower();
                    var uploadspic = Path.Combine(environment.WebRootPath, "img\\productgallery\\");
                    using (var fileStream = new FileStream(Path.Combine(uploadspic, FileNamepic), FileMode.Create))
                    {
                        await item.CopyToAsync(fileStream);
                    }
                   Gallery g = new Gallery();
                    g.DefaultPicProduct = false;
                    g.ProductId_FK = qProduct.ProductId;
                    g.PictureName = FileNamepic;
                    g.TitlePic = qProduct.ProductNameFA;
                    lstgallery.Add(g);
                }
                database.Tbl_Gallery.AddRange(lstgallery.AsEnumerable());
                await database.SaveChangesAsync();
                //return RedirectToAction(nameof(ProductListAdmin),"Product");
                return RedirectToAction("ProductListAdmin", "Product");
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteGalleryProduct(string[] IdGallery, string[] ChekboxDelete, int IdPro)
        {
            if (IdGallery.Count() == 0 || ChekboxDelete.Count() == 0)
                return RedirectToAction(nameof(EditGalleryProduct), "Galleries", new { id = IdPro });
            foreach (var item in IdGallery.ToList())
            {
                foreach (var item1 in ChekboxDelete)
                {
                    if (item == item1)
                    {
                        var qdelete = await database.Tbl_Gallery.SingleOrDefaultAsync(c => c.GalleryId == Convert.ToInt32(item1));
                        var DeleteImage = Path.Combine(environment.WebRootPath, "img\\productgallery\\" + qdelete.PictureName);
                        if (System.IO.File.Exists(DeleteImage))
                        {
                            System.IO.File.Delete(DeleteImage);
                        }
                        if (qdelete != null)
                            database.Tbl_Gallery.Remove(qdelete);
                        await database.SaveChangesAsync();
                    }
                }
            }
            return RedirectToAction(nameof(EditGalleryProduct), "Galleries", new { productId = IdPro });
        }   
        // GET: Galleries/Create
        public IActionResult CreateGalleryProduct(int productId)
        {
            //ViewData["ProductId_FK"] = new SelectList(database.Tbl_Products, "ProductId", "Description");
            var q = database.Tbl_Products.Where(c => c.ProductId == productId).SingleOrDefault();
            if (q == null)
            //return RedirectToAction(nameof(GalleriesController.Index), "Galleries");
            {
                return RedirectToAction("CreateProduct", "Product");
            }
            var lstCategory = (from row in database.Tbl_Category select row).ToList();
            ViewBag.CreateCat = lstCategory;
            ViewBag.ProductId = q.ProductId;
            ViewBag.ProductName = q.ProductNameFA;
            return View();
        }
        // POST: Galleries/Create       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGalleryProduct(IFormFile[] GalleryProduct, int ProductID)
        {
            if (GalleryProduct[0] == null)
            {
                return View();
            }
            else
            {
                var qProduct = database.Tbl_Products.Where(c => c.ProductId == ProductID).FirstOrDefault();
                Gallery tbl_gallery = new Gallery();
                //string fileNamePic = Guid.NewGuid().ToString().Replace("-", "") + GalleryProduct[0].FileName.ToLower();
                //var uploadsPicMain = Path.Combine(environment.WebRootPath, "img\\productgallery");
                //using (var fileStreamMain = new FileStream(Path.Combine(uploadsPicMain, fileNamePic), FileMode.Create))
                //{
                //    await GalleryProduct[0].CopyToAsync(fileStreamMain);
                //}
                //tbl_gallery.DefaultPicProduct = false;
                //tbl_gallery.ProductId_FK = qProduct.ProductId;
                //tbl_gallery.PictureName = fileNamePic;
                //tbl_gallery.TitlePic = qProduct.ProductNameFA;
                //database.Tbl_Gallery.Add(tbl_gallery);
                //await database.SaveChangesAsync();
                List<Gallery> lstGallery = new List<Gallery>();
                foreach (var item in GalleryProduct)
                {
                    Gallery g = new Gallery();  // یا میتوان از ویو مدل استفاده کرد
                    string filename = Guid.NewGuid().ToString().Replace("-", "") + GalleryProduct[0].FileName.ToLower();
                    var uploadsPic = Path.Combine(environment.WebRootPath, "img\\productgallery");
                    using (var fileStream = new FileStream(Path.Combine(uploadsPic, filename), FileMode.Create))
                    {
                        await item.CopyToAsync(fileStream);
                    }
                    g.ProductId_FK = qProduct.ProductId;
                    g.PictureName = filename;
                    g.TitlePic = qProduct.ProductNameFA;
                    g.DefaultPicProduct = false;
                    lstGallery.Add(g);
                }
                database.Tbl_Gallery.AddRange(lstGallery.AsEnumerable());
                await database.SaveChangesAsync();
                return RedirectToAction("ProductListAdmin","Product");
            }
        }
        public IActionResult DeleteALLGalleryProduct(int productId)
        {
            var model = database.Tbl_Gallery.Where(c => c.ProductId_FK == productId).ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteALLGalleryProduct( int productId, Gallery gallery, string[] IdGallery)
        {
            if (ModelState.IsValid) {
         
                    var model = database.Tbl_Gallery.Where(c => c.ProductId_FK == productId).ToList();          
                database.Tbl_Gallery.RemoveRange(model);
                database.SaveChangesAsync();
                return RedirectToAction("ProductListAdmin", "Product");
            }
            return RedirectToAction("ProductListAdmin", "Product");
        }
    }
}