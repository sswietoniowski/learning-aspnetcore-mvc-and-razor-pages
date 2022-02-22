using BulkyBook.DataAccess.Data;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BulkyBook.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
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

        #endregion
    }
}
