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
    public class ProductController : Controller
    {
        int productId;
        string FileNamepics;
        //private readonly IUploadfile _upload;
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly SignInManager<ApplicationUsers> signInManager;
        private readonly ICategoryRepository _icatr;
        private readonly ApplicationDbContext database;
        private readonly IHostingEnvironment environment;
        private readonly IProductRepository _ipcr;
        private readonly IUploadfile _upload;
        public ProductController(ApplicationDbContext context, IProductByShapeRepository iproshaper, ICategoryRepository icatr,
            IProductRepository ipcr, UserManager<ApplicationUsers> userManager, IHostingEnvironment _environment, IUploadfile upload)
        {
            database = context;
            _icatr = icatr;
            _ipcr = ipcr;
            _ipcr = ipcr;
            _userManager = userManager;
            environment = _environment;
            _upload = upload;
        }
        public IActionResult ProductListAdmin()
        {
            return View(_ipcr.ShowAllProducts());
        }
       public IActionResult UploadFile(IEnumerable<IFormFile> files)
        {
            string filename = _upload.UploadFiles(files, "img\\productdefaultpic\\", "");
            return Json(new { status = "success", message = "تصویر با موفقیت آپلود شد.", imagename = filename });
        }
        [HttpGet]
        public IActionResult EditProduct(int productid)
        {
            var lstCategory = (from row in database.Tbl_Category select row).ToList();
            var model = _ipcr.ShowProductsDetails(productid);
          
            if (model == null)
            {
                return NotFound();                     
            }
            else
            {
                ViewBag.EditCat = lstCategory;
            }
            return View(model);
        }
         [HttpPost]
        public IActionResult EditProduct(int productid, string newIndexImage, string currentImageName, Products model)
        {
            var lstCategory = (from row in database.Tbl_Category select row).ToList();
            ViewBag.EditCat = lstCategory;
            if (ModelState.IsValid)
            {
                var qProduct = database.Tbl_Products.Where(c => c.ProductId == productid).SingleOrDefault();

                if (newIndexImage != null)
                {
                    qProduct.DefaultPic = newIndexImage;
                }
                else
                {
                    qProduct.DefaultPic = currentImageName;
                }
                try
                {
                    //if (newIndexImage != null)
                    //{
                        // FileNamepics = Guid.NewGuid().ToString().Replace("-", "") + defaultpicProdouct.FileName.ToLower();
                        //var uploadspics = Path.Combine(environment.WebRootPath, "img\\productdefaultpic");
                        //using (var fileStream = new FileStream(Path.Combine(uploadspics, FileNamepics), FileMode.Create))
                        //{
                        //    defaultpicProdouct.CopyToAsync(fileStream);
                        //}
                        //qProduct.DefaultPic = FileNamepics;
                    //}
                    //else { qProduct.DefaultPic=currentImageName; }         
                    qProduct.ProductNameFA = model.ProductNameFA;
                    qProduct.ProductNameEN = model.ProductNameEN;
                    qProduct.Description = model.Description;
                    qProduct.CategoryId_FK = model.CategoryId_FK;
                    qProduct.SummeryDescription = model.SummeryDescription;
                    database.SaveChanges();                 
                }
                catch
                {
                    throw;
                }
            }      
            return RedirectToAction("ProductListAdmin", "Product");
        }
        [HttpGet]
        public  IActionResult CreateProduct()
        {
            var lstCategory = (from row in database.Tbl_Category select row).ToList();
            ViewBag.CreateCat = lstCategory;
            return View();
        }
       [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProduct(VmDetailsProduct model, int Productid, string DefaultPic)
        {
            if (ModelState.IsValid)
            {
                var lstCategory = (from row in database.Tbl_Category select row).ToList();
                ViewBag.CreateCat = lstCategory;
                model.DefaultPic = DefaultPic;
                try
                {
                    //string FileNamepics = Guid.NewGuid().ToString().Replace("-", "") + defaultpicProdouct.FileName.ToLower();
                    //var uploadspics = Path.Combine(environment.WebRootPath, "img\\productdefaultpic");
                    //using (var fileStream = new FileStream(Path.Combine(uploadspics, FileNamepics), FileMode.Create))
                    //{
                    //    await defaultpicProdouct.CopyToAsync(fileStream);
                    //}
                    Products product = new Products
                    {
                        ProductNameFA = model.ProductNameFA,
                        ProductNameEN = model.ProductNameEN,
                        DefaultPic = model.DefaultPic,
                        Description = model.Description,
                        CategoryId_FK = model.CategoryId_FK,
                        SummeryDescription = model.SummeryDescription,

                    };
                    database.Tbl_Products.Add(product);
                    database.SaveChanges();
                productId =product.ProductId;
                }

                catch
                {
                    throw;
                }
             
             return RedirectToAction("CreateGalleryProduct", "Galleries", new { productId = productId });
            }
          
            return View(model);
        }
        //public async Task<IActionResult> DeleteProduct(string IdGallery, string ChekboxDelete, int IdPro)
        //{
        //    if (IdGallery.Count() == 0 || ChekboxDelete.Count() == 0)
        //        return RedirectToAction(nameof(EditProduct), "Product", new { id = IdPro });
        //    foreach (var item in IdGallery.ToList())
        //    {
        //        foreach (var item1 in ChekboxDelete)

        //        {
        //            if (item == item1)
        //            {
        //                var qdelete = await database.Tbl_Products.SingleOrDefaultAsync(c => c.ProductId == Convert.ToInt32(item1));
        //                var DeleteImage = Path.Combine(environment.WebRootPath, "img\\productdefaultpic\\" + qdelete.DefaultPic);
        //                if (System.IO.File.Exists(DeleteImage))
        //                {
        //                    System.IO.File.Delete(DeleteImage);
        //                }
                
        //                await database.SaveChangesAsync();
        //            }           
        //        }
        //    }
        //    return RedirectToAction(nameof(EditProduct), "Product", new { productId = IdPro });
        //}


        // GET: Products/Delete/5
        public IActionResult DeleteProduct(int Productid)
        {
            if (Productid == null)
            {
                return NotFound();
            }
            var lstCategory = database.Tbl_Products.Where(c=>c.ProductId== Productid).Select(c=>c.ProductNameFA).SingleOrDefault();
            ViewBag.Cat = lstCategory;
            var products = _ipcr.ShowProductsDetails(Productid);
            var DeleteImage = Path.Combine(environment.WebRootPath, "img\\productdefaultpic\\" + products.DefaultPic);
            if (System.IO.File.Exists(DeleteImage))
            {
                System.IO.File.Delete(DeleteImage);
            }
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }
        // POST: Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProduct(VmDetailsProduct model, int Productid, IFormFile defaultpicProdouct)
        {
            var lstCategory = database.Tbl_Products.Where(c => c.ProductId == Productid).Select(c => c.ProductNameFA).SingleOrDefault();
            ViewBag.Cat = lstCategory;
            Products products = database.Tbl_Products.Where(c=>c.ProductId== Productid).SingleOrDefault();
            productId = products.ProductId;
            database.Tbl_Products.Remove(products);
           database.SaveChangesAsync();
            //return RedirectToAction("DeleteALLGalleryProduct", "Galleries", new { productId = productId });
            return RedirectToAction("ProductListAdmin", "Product");

        }
    }
    }
// GET: Products/Delete/5
//[HttpGet]
//public IActionResult DeleteProduct(int Productid)
//        {
//            if (Productid == null)
//            {
//                return NotFound();
//            }

//            var products = _ipcr.ShowProductsDetails(Productid);
//            if (products == null)
//            {
//                return NotFound();
//            }



//    return PartialView("_DeleteProduct", products);
//}


//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(VmDetailsProduct model, int Productid)
//        {
//            var products = await database.Tbl_Products.SingleOrDefaultAsync(m => m.ProductId == Productid);
//            var galleries = await database.Tbl_Gallery.Where(m=>m.ProductId_FK== Productid).ToListAsync();
//            database.Tbl_Gallery.RemoveRange(galleries);
//            database.Tbl_Products.Remove(products);
//             await database.SaveChangesAsync();
//            return RedirectToAction(nameof(ProductListAdmin));
//        }
//        [HttpGet]
//        public IActionResult CreateProduct()
//        {
//            ViewBag.CreateCat = _icatr.ShowCategories();
//            return View();
//        }