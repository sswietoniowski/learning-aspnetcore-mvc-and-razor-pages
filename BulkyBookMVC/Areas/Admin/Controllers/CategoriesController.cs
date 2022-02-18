using BulkyBook.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = _unitOfWork.Categories.GetAll();

            return Json(new { data = categories });
        }

        #endregion
    }
}
