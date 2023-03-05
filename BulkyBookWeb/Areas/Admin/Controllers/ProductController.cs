﻿using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnviroent;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnviroent)
        {
            _unitOfWork = unitOfWork;
            _hostEnviroent = hostEnviroent;
        }


        // ++++++++++++++++ READ +++++++++++++++++++++
        public IActionResult Index()
        {
            return View();
        }
        

        // ++++++++++++++++ UPSERT  (Update + Insert) +++++++++++++++++++++
        // Form for Upsert Product
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select( u => new SelectListItem {
                        Text = u.Name,
                        Value = u.Id.ToString(),
                    }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select( u => new SelectListItem {
                        Text = u.Name,
                        Value = u.Id.ToString(),
                    })
            };

            if (id == 0 || id == null)
            {
                // ++++++++++ Create (Insert) Product ++++++++++
                //ViewBag.CategoryList = CategoryList;
                return View(productVM);
            }
            else
            {
                // ++++++++++ Update Product ++++++++++
                productVM.Product = _unitOfWork.Product.GetFirstOrDefault( u => u.Id == id);
                return View(productVM);
            }
        }

        // POST => UPSERT Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnviroent.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);

                    if(obj.Product.ImageUrl!= null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }

                if(obj.Product.Id == 0)
                    _unitOfWork.Product.Add(obj.Product);
                else
                    _unitOfWork.Product.Update(obj.Product);

                _unitOfWork.Save();
                TempData["success"] = "Product Created Successfully!!!";
                return RedirectToAction("Index");
            }
            return View(obj);
        }




        #region API CALLS

        [HttpGet]
        public JsonResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data = productList });
        }


        // POST => DELETE Cover Type
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
            if (obj == null)
                return Json(new { success=false, message="Error While Deleting" });

            var oldImagePath = Path.Combine(_hostEnviroent.WebRootPath, obj.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
                System.IO.File.Delete(oldImagePath);

            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Product Deleted Successfully!!!" });
        }

        #endregion
    }
}
