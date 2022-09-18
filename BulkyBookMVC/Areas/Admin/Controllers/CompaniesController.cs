using BulkyBook.DataAccess;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.MVC.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{SD.IdentityRole_Admin},{SD.IdentityRole_Employee}")]
public class CompaniesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CompaniesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Upsert(int? id)
    {
        var company = new Company();

        if (id is null)
        {
            // insert/create
            return View(company);
        }

        company = _unitOfWork.Companies.Get(id.GetValueOrDefault());

        if (company is null)
        {
            return NotFound();
        }

        // update/edit
        return View(company);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(Company company)
    {
        if (ModelState.IsValid)
        {
            if (company.Id == 0)
            {
                _unitOfWork.Companies.Add(company);
            }
            else
            {
                _unitOfWork.Companies.Update(company);
            }
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        return View(company);
    }

    #region API CALLS

    [HttpGet]
    public IActionResult GetAll()
    {
        var categories = _unitOfWork.Companies.GetAll();

        return Json(new { data = categories });
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        var companyFromDb = _unitOfWork.Companies.Get(id);
        if (companyFromDb is null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        _unitOfWork.Companies.Remove(companyFromDb);
        _unitOfWork.Save();

        return Json(new { success = true, message = "Delete successful" });
    }

    #endregion
}