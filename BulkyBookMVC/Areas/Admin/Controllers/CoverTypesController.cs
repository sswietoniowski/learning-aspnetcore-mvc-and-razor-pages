using BulkyBook.DataAccess;
using BulkyBook.Models;
using BulkyBook.Utility;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            var coverType = new CoverType();

            if (id is null)
            {
                return View(coverType);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@Id", id.GetValueOrDefault());
            coverType = _unitOfWork.StoredProcedureCalls.OneRecord<CoverType>(SD.SP_CoverTypes_Select, parameters);

            if (coverType is null)
            {
                return NotFound();
            }

            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Name", coverType.Name);
                if (coverType.Id == 0)
                {
                    _unitOfWork.StoredProcedureCalls.Execute(SD.SP_CoverTypes_Insert, parameters);
                }
                else
                {
                    parameters.Add("@Id", coverType.Id);
                    _unitOfWork.StoredProcedureCalls.Execute(SD.SP_CoverTypes_Update, parameters);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(coverType);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var coverTypes = _unitOfWork.StoredProcedureCalls.List<CoverType>(SD.SP_CoverTypes_SelectAll, null);
            return Json(new { data = coverTypes });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            var coverTypeFromDb = _unitOfWork.StoredProcedureCalls.OneRecord<CoverType>(SD.SP_CoverTypes_Select, parameters);
            if (coverTypeFromDb is null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.StoredProcedureCalls.Execute(SD.SP_CoverTypes_Delete, parameters);

            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion
    }
}
