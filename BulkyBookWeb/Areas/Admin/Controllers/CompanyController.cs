using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            Company company = new();
            if (id == 0 || id == null)
                return View(company);
            else
            {
                company = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
                return View(company);
            }
        }

        // POST => UPSERT Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj)
        {
            if (ModelState.IsValid)
            {
                if(obj.Id == 0)
                    _unitOfWork.Company.Add(obj);
                else
                    _unitOfWork.Company.Update(obj);

                _unitOfWork.Save();
                TempData["success"] = "Company Created Successfully!!!";
                return RedirectToAction("Index");
            }
            return View(obj);
        }




        #region API CALLS

        [HttpGet]
        public JsonResult GetAll()
        {
            var companyList = _unitOfWork.Company.GetAll();
            return Json(new { data = companyList });
        }


        // POST => DELETE Cover Type
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.Company.GetFirstOrDefault(x => x.Id == id);
            if (obj == null)
                return Json(new { success=false, message="Error While Deleting" });
            _unitOfWork.Company.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Company Deleted Successfully!!!" });
        }

        #endregion
    }
}
