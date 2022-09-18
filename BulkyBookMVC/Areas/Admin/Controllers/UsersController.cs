using BulkyBook.DataAccess.Data;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BulkyBook.MVC.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{SD.IdentityRole_Admin},{SD.IdentityRole_Employee}")]
public class UsersController : Controller
{
    private const int LOCKOUT_DURATION_IN_MINUTES = 15;
    private readonly ApplicationDbContext _db;

    public UsersController(ApplicationDbContext db)
    {
        // we could use a UoW (as in the other controllers), this time we're using DbContext, just
        // to show an alternate way of doing things; normally we wouldn't mix the two approaches,
        // now it is only for the demo purposes
        _db = db;
    }

    public IActionResult Index()
    {
        return View();
    }

    #region API CALLS

    [HttpGet]
    public IActionResult GetAll()
    {
        var users = _db.ApplicationUsers.Include(u => u.Company).ToList();
        var userRoles = _db.UserRoles.ToList();
        var roles = _db.Roles.ToList();

        users.ForEach(u =>
        {
            var roleId = userRoles.FirstOrDefault(ur => u.Id == ur.UserId).RoleId;
            u.Role = roles.FirstOrDefault(r => r.Id == roleId).Name;

            if (u.Company is null)
            {
                u.Company = new Company { Name = "" };
            }
        });

        return Json(new { data = users });
    }

    [HttpPost]
    public IActionResult LockUnlock([FromBody] string id)
    {
        var userFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);

        if (userFromDb is null)
        {
            return Json(new { success = false, message = "Error while Locking/Unlocking" });
        }

        if (userFromDb.LockoutEnd is not null && userFromDb.LockoutEnd > DateTime.Now)
        {
            // unlock user
            userFromDb.LockoutEnd = DateTime.Now;
        }
        else
        {
            // lock user
            userFromDb.LockoutEnd = DateTime.Now.AddMinutes(LOCKOUT_DURATION_IN_MINUTES);
            _db.SaveChanges();
        }
        _db.SaveChanges();

        return Json(new { success = true, message = "Operation successful" });
    }

    #endregion
}