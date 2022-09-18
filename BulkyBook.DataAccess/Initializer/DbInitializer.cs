using BulkyBook.DataAccess.Data;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BulkyBook.DataAccess.Initializer;

public class DbInitializer : IDbInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DbInitializer(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        this._context = context;
        this._userManager = userManager;
        this._roleManager = roleManager;
    }

    public void Initialize()
    {
        try
        {
            if (_context.Database.GetPendingMigrations().Count() > 0)
            {
                _context.Database.Migrate();
            }


        }
        catch (Exception)
        {
        }

        if (_context.Roles.Any(r => r.Name == SD.IdentityRole_Admin))
        {
            return;
        }

        _roleManager.CreateAsync(new IdentityRole(SD.IdentityRole_Admin)).GetAwaiter().GetResult();
        _roleManager.CreateAsync(new IdentityRole(SD.IdentityRole_Employee)).GetAwaiter().GetResult();
        _roleManager.CreateAsync(new IdentityRole(SD.IdentityRole_Customer_Company)).GetAwaiter().GetResult();
        _roleManager.CreateAsync(new IdentityRole(SD.IdentityRole_Customer_Individual)).GetAwaiter().GetResult();

        _userManager.CreateAsync(new ApplicationUser
        {
            UserName = "admin@test.com",
            Email = "admin@test.com",
            EmailConfirmed = true,
            Name = "Admin"
        }, "NOT_REAL_PASSWORD!!!").GetAwaiter().GetResult();
        ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@test.com");
        _userManager.AddToRoleAsync(user, SD.IdentityRole_Admin).GetAwaiter().GetResult();

    }
}