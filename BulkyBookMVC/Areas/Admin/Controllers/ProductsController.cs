using BulkyBook.DataAccess;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Any())
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images\products");
                    var extension = Path.GetExtension(files[0].FileName);

                    if (!String.IsNullOrWhiteSpace(productViewModel.Product.ImageUrl))
                    {
                        // this is an edit and we need to remove old image
                        var imagePath = Path.Combine(uploads, productViewModel.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    using var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create);
                    files[0].CopyTo(fileStreams);
                    productViewModel.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }
                else
                {
                    // update when they do not change the image
                    if (productViewModel.Product.Id != 0)
                    {
                        var productFromDb = _unitOfWork.Products.Get(productViewModel.Product.Id);
                        productViewModel.Product.ImageUrl = productFromDb.ImageUrl;
                    }
                }

                if (productViewModel.Product.Id == 0)
                {
                    _unitOfWork.Products.Add(productViewModel.Product);
                }
                else
                {
                    _unitOfWork.Products.Update(productViewModel.Product);
                }
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                // handling edge case (error in case when client's validation would be disabled and not all
                // inputs would be properly selected), otherwise we woudl see a 500 error

                var categories = _unitOfWork.Categories.GetAll()
                    .Select(item => new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                productViewModel.Categories = categories;

                var coverTypes = _unitOfWork.CoverTypes.GetAll()
                    .Select(item => new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                productViewModel.CoverTypes = coverTypes;

                if (productViewModel.Product.Id != 0)
                {
                    productViewModel.Product = _unitOfWork.Products.Get(productViewModel.Product.Id);
                }
            }

            return View(productViewModel);
        }

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

            if (!String.IsNullOrWhiteSpace(productFromDb.ImageUrl))
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                var imagePath = Path.Combine(webRootPath, productFromDb.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _unitOfWork.Products.Remove(productFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion
    }
}
