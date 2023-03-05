
using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        // ++++++++++++++++ READ +++++++++++++++++++++

        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _unitOfWork.Category.GetAll();
            return View(objCategoryList);
        }




        // ++++++++++++++++ CREATE +++++++++++++++++++++

        // Form for Creating Category
        public IActionResult Create()
        {
            return View();
        }

        // POST => CREATE Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
                ModelState.AddModelError("Name", "The Display Order cann't exactly match the Name.");

            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(category);
                _unitOfWork.Save();
                TempData["success"] = "Category Updated Successfully!!!";
                return RedirectToAction("Index");
            }
            return View(category);
        }





        // ++++++++++++++++ UPDATE +++++++++++++++++++++

        // Form for Updating Category
        public IActionResult Edit(int id)
        {
            if (id == 0)
                return NotFound();
            //var category = _db.Categories.Find(id);
            var category = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
            if (category == null)
                return NotFound();
            return View(category);
        }

        // POST => Update Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
                ModelState.AddModelError("name", "The Display Order cann't exactly match the Name.");

            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(category);
                _unitOfWork.Save();
                TempData["success"] = "Category Updated Successfully!!!";
                return RedirectToAction("Index");
            }
            return View(category);
        }




        // ++++++++++++++++ DELETE +++++++++++++++++++++

        // Form for Deleting Category
        public IActionResult Delete(int id)
        {
            if (id == 0)
                return NotFound();
            var category = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
            if (category == null)
                return NotFound();
            return View(category);
        }

        // POST => DELETE Category
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var category = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
            if (category == null) return NotFound();
            _unitOfWork.Category.Remove(category);
            _unitOfWork.Save();
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
