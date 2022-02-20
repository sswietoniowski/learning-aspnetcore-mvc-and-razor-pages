using BulkyBook.DataAccess;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace BulkyBook.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductsController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            var categories = _unitOfWork.Categories.GetAll()
                .Select(item => new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            var coverTypes = _unitOfWork.CoverTypes.GetAll()
                .Select(item => new SelectListItem { Text = item.Name, Value = item.Id.ToString() });

            var productViewModel = new ProductViewModel
            {
                Product = new Product(),
                Categories = categories,
                CoverTypes = coverTypes
            };

            if (id is null)
            {
                // insert/create
                return View(productViewModel);
            }

            var product = _unitOfWork.Products.GetFirstOrDefault(p => p.Id == id.GetValueOrDefault());

            if (product is null)
            {
                return NotFound();
            }

            productViewModel.Product = product;

            // update/edit
            return View(productViewModel);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Upsert(Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (product.Id == 0)
        //        {
        //            _unitOfWork.Products.Add(product);
        //        }
        //        else
        //        {
        //            _unitOfWork.Products.Update(product);
        //        }
        //        _unitOfWork.Save();

        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(product);
        //}

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _unitOfWork.Products.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data = products });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productFromDb = _unitOfWork.Products.Get(id);
            if (productFromDb is null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Products.Remove(productFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion
    }
}
