﻿using BulkyBook.DataAccess;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace BulkyBook.MVC.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var products = _unitOfWork.Products.GetAll(includeProperties: "Category,CoverType");

        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        if (claim is not null)
        {
            var count = _unitOfWork.ShoppingCarts
                .GetAll(c => c.ApplicationUserId == claim.Value).Count();
            HttpContext.Session.SetInt32(SD.Session_ShoppingCart, count);
        }

        return View(products);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Details(int id)
    {
        var productFromDb = _unitOfWork.Products
            .GetFirstOrDefault(p => p.Id == id, includeProperties: "Category,CoverType");
        ShoppingCart shoppingCart = new ShoppingCart()
        {
            Product = productFromDb,
            ProductId = productFromDb.Id
        };
        return View(shoppingCart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        shoppingCart.Id = 0;
        if (ModelState.IsValid)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;
            ShoppingCart shoppingCartFromDb = _unitOfWork.ShoppingCarts.GetFirstOrDefault(
                sc => sc.ApplicationUserId == shoppingCart.ApplicationUserId && sc.ProductId == shoppingCart.ProductId,
                includeProperties: "Product");
            if (shoppingCartFromDb is null)
            {
                _unitOfWork.ShoppingCarts.Add(shoppingCart);
            }
            else
            {
                shoppingCartFromDb.Count += shoppingCart.Count;
                //_unitOfWork.ShoppingCarts.Update(shoppingCartFromDb); // not really needed
            }
            _unitOfWork.Save();

            var count = _unitOfWork.ShoppingCarts
                .GetAll(c => c.ApplicationUserId == shoppingCart.ApplicationUserId).Count();

            // We could store anything, so that's why we have an extension method
            //HttpContext.Session.SetObject(SD.Session_ShoppingCart, count);
            // This method on the other hand is built in
            HttpContext.Session.SetInt32(SD.Session_ShoppingCart, count);

            return RedirectToAction(nameof(Index));
        }
        else
        {
            var productFromDb = _unitOfWork.Products
                .GetFirstOrDefault(p => p.Id == shoppingCart.Id, includeProperties: "Category,CoverType");
            shoppingCart = new ShoppingCart()
            {
                Product = productFromDb,
                ProductId = productFromDb.Id
            };
            return View(shoppingCart);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}