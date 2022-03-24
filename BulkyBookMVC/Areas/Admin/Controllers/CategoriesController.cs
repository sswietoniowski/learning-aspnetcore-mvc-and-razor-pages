using BulkyBook.DataAccess;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BulkyBook.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.IdentityRole_Admin)]
    public class CategoriesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int productPage = 1)
        {
            CategoriesViewModel categoriesViewModel = new CategoriesViewModel
            {
                Categories = _unitOfWork.Categories.GetAll()

            };

            int count = categoriesViewModel.Categories.Count();
            categoriesViewModel.Categories = categoriesViewModel.Categories.OrderBy(c => c.Name).Skip((productPage - 1) * 2).Take(2).ToList();
            categoriesViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = 2,
                TotalItem = count,
                urlParam = "/Admin/Categories/Index?productPage=:"
            };

            return View(categoriesViewModel);
        }

        public IActionResult Upsert(int? id)
        {
            var category = new Category();

            if (id is null)
            {
                // insert/create
                return View(category);
            }

            category = _unitOfWork.Categories.Get(id.GetValueOrDefault());

            if (category is null)
            {
                return NotFound();
            }

            // update/edit
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    _unitOfWork.Categories.Add(category);
                }
                else
                {
                    _unitOfWork.Categories.Update(category);
                }
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = _unitOfWork.Categories.GetAll();

            return Json(new { data = categories });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var categoryFromDb = _unitOfWork.Categories.Get(id);
            if (categoryFromDb is null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Categories.Remove(categoryFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion
    }
}
