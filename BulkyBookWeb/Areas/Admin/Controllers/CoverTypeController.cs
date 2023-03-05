using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        // ++++++++++++++++ READ +++++++++++++++++++++

        public IActionResult Index()
        {
            IEnumerable<CoverType> objCoverTypeList = _unitOfWork.CoverType.GetAll();
            return View(objCoverTypeList);
        }


        // ++++++++++++++++ CREATE +++++++++++++++++++++

        // Form for Creating Cover Type
        public IActionResult Create()
        {
            return View();
        }

        // POST => CREATE Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CoverType coverType)
        {
            if(ModelState.IsValid)
            {
                _unitOfWork.CoverType.Add(coverType);
                _unitOfWork.Save();
                TempData["success"] = "Cover Type Added Successfully!!!";
                return RedirectToAction("Index");
            }
            return View(coverType);
        }


        // ++++++++++++++++ UPDATE +++++++++++++++++++++

        // Form for Creating Cover Type
        public IActionResult Edit(int id)
        {
            if(id == 0)
                return NotFound();
            var coverType = _unitOfWork.CoverType.GetFirstOrDefault(x => x.Id == id);
            if(coverType == null)
                return NotFound();
            return View(coverType);
        }

        // POST => UPDATE Cover Type
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverType.Update(coverType);
                _unitOfWork.Save();
                TempData["success"] = "Cover Type Updated Successfully!!!";
                return RedirectToAction("Index");
            }
            return View(coverType);
        }



        // ++++++++++++++++ DELETE +++++++++++++++++++++

        // Form for Deleting Cover Type
        public IActionResult Delete(int id)
        {
            if (id == 0)
                return NotFound();
            var coverType = _unitOfWork.CoverType.GetFirstOrDefault(x => x.Id == id);
            if (coverType == null)
                return NotFound();
            return View(coverType);
        }

        // POST => DELETE Cover Type
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCoverType(int id)
        {
            var coverType = _unitOfWork.CoverType.GetFirstOrDefault(x => x.Id == id);
            if (coverType == null) return NotFound();
            _unitOfWork.CoverType.Remove(coverType);
            _unitOfWork.Save();
            TempData["success"] = "Cover Type Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
