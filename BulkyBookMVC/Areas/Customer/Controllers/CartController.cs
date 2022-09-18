using BulkyBook.DataAccess;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace BulkyBook.MVC.Areas.Customer.Controllers;

[Area("Customer")]
public class CartController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailSender _emailSender;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly TwilioOptions _twilioOptions;

    [BindProperty]
    public ShoppingCartViewModel ShoppingCartViewModel { get; set; }

    public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender, UserManager<IdentityUser> userManager, IOptions<TwilioOptions> twilioOptions)
    {
        _unitOfWork = unitOfWork;
        _emailSender = emailSender;
        _userManager = userManager;
        _twilioOptions = twilioOptions.Value;
    }

    public IActionResult Index()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        ShoppingCartViewModel = new ShoppingCartViewModel()
        {
            OrderHeader = new OrderHeader(),
            ListCart = _unitOfWork.ShoppingCarts.GetAll(
                u => u.ApplicationUserId == claim.Value, includeProperties: "Product")
        };
        ShoppingCartViewModel.OrderHeader.OrderTotal = 0;
        ShoppingCartViewModel.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUsers
            .GetFirstOrDefault(u => u.Id == claim.Value, includeProperties: "Company");

        foreach (var list in ShoppingCartViewModel.ListCart)
        {
            list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
            ShoppingCartViewModel.OrderHeader.OrderTotal += (list.Price * list.Count);
            list.Product.Description = SD.ConvertToRawHtml(list.Product.Description ?? "");
            if (list.Product.Description.Length > 100)
            {
                list.Product.Description = list.Product.Description.Substring(0, 99) + "...";
            }
        }

        return View(ShoppingCartViewModel);
    }

    [HttpPost]
    [ActionName("Index")]
    public async Task<IActionResult> IndexPost()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        var user = _unitOfWork.ApplicationUsers.GetFirstOrDefault(u => u.Id == claim.Value);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Verification email is empty!");
        }

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: new { area = "Identity", userId = user.Id, code = code },
            protocol: Request.Scheme);

        await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        ModelState.AddModelError(string.Empty, "Verification email sent. Please check you email.");
        return RedirectToAction("Index");
    }

    public IActionResult Plus(int cartId)
    {
        var shoppingCart = _unitOfWork.ShoppingCarts
            .GetFirstOrDefault(sc => sc.Id == cartId, includeProperties: "Product");
        shoppingCart.Count++;
        shoppingCart.Price = SD.GetPriceBasedOnQuantity(shoppingCart.Count,
            shoppingCart.Product.Price, shoppingCart.Product.Price50, shoppingCart.Product.Price100);
        _unitOfWork.Save();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Minus(int cartId)
    {
        var shoppingCart = _unitOfWork.ShoppingCarts
            .GetFirstOrDefault(sc => sc.Id == cartId, includeProperties: "Product");

        if (shoppingCart.Count == 1)
        {
            var count = _unitOfWork.ShoppingCarts.GetAll(u => u.ApplicationUserId == shoppingCart.ApplicationUserId).Count();
            _unitOfWork.ShoppingCarts.Remove(shoppingCart);
            _unitOfWork.Save();
            HttpContext.Session.SetInt32(SD.Session_ShoppingCart, count - 1);
        }
        else
        {
            shoppingCart.Count--;
            shoppingCart.Price = SD.GetPriceBasedOnQuantity(shoppingCart.Count,
                shoppingCart.Product.Price, shoppingCart.Product.Price50, shoppingCart.Product.Price100);
            _unitOfWork.Save();
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Remove(int cartId)
    {
        var shoppingCart = _unitOfWork.ShoppingCarts
            .GetFirstOrDefault(sc => sc.Id == cartId, includeProperties: "Product");
        var count = _unitOfWork.ShoppingCarts.GetAll(u => u.ApplicationUserId == shoppingCart.ApplicationUserId).Count();
        _unitOfWork.ShoppingCarts.Remove(shoppingCart);
        _unitOfWork.Save();
        HttpContext.Session.SetInt32(SD.Session_ShoppingCart, count - 1);

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Summary()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        ShoppingCartViewModel = new ShoppingCartViewModel
        {
            OrderHeader = new OrderHeader(),
            ListCart = _unitOfWork.ShoppingCarts
                .GetAll(sc => sc.ApplicationUserId == claim.Value, includeProperties: "Product")
        };

        ShoppingCartViewModel.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUsers
            .GetFirstOrDefault(u => u.Id == claim.Value, includeProperties: "Company");

        foreach (var list in ShoppingCartViewModel.ListCart)
        {
            list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
            ShoppingCartViewModel.OrderHeader.OrderTotal += (list.Price * list.Count);
        }

        ShoppingCartViewModel.OrderHeader.Name = ShoppingCartViewModel.OrderHeader.ApplicationUser.Name;
        ShoppingCartViewModel.OrderHeader.PhoneNumber = ShoppingCartViewModel.OrderHeader.ApplicationUser.PhoneNumber;
        ShoppingCartViewModel.OrderHeader.StreetAddress = ShoppingCartViewModel.OrderHeader.ApplicationUser.StreetAddress;
        ShoppingCartViewModel.OrderHeader.City = ShoppingCartViewModel.OrderHeader.ApplicationUser.City;
        ShoppingCartViewModel.OrderHeader.PostalCode = ShoppingCartViewModel.OrderHeader.ApplicationUser.PostalCode;

        return View(ShoppingCartViewModel);
    }

    [HttpPost]
    [ActionName("Summary")]
    [ValidateAntiForgeryToken]
    public IActionResult SummaryPost(string stripeToken)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        ShoppingCartViewModel.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUsers
            .GetFirstOrDefault(u => u.Id == claim.Value, includeProperties: "Company");

        ShoppingCartViewModel.ListCart = _unitOfWork.ShoppingCarts
            .GetAll(sc => sc.ApplicationUserId == claim.Value, includeProperties: "Product");

        ShoppingCartViewModel.OrderHeader.PaymentStatus = SD.PaymentStatus_Pending;
        ShoppingCartViewModel.OrderHeader.OrderStatus = SD.OrderStatus_Pending;
        ShoppingCartViewModel.OrderHeader.ApplicationUserId = claim.Value;
        ShoppingCartViewModel.OrderHeader.OrderDate = DateTime.Now;

        _unitOfWork.OrderHeaders.Add(ShoppingCartViewModel.OrderHeader);
        _unitOfWork.Save();

        List<OrderDetail> orderDetails = new List<OrderDetail>();
        foreach (var item in ShoppingCartViewModel.ListCart)
        {
            item.Price = SD.GetPriceBasedOnQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
            OrderDetail orderDetail = new OrderDetail
            {
                ProductId = item.ProductId,
                OrderId = ShoppingCartViewModel.OrderHeader.Id,
                Price = item.Price,
                Count = item.Count
            };
            ShoppingCartViewModel.OrderHeader.OrderTotal += orderDetail.Count * orderDetail.Price;
            _unitOfWork.OrderDetails.Add(orderDetail);
            orderDetails.Add(orderDetail);
        }

        _unitOfWork.ShoppingCarts.RemoveRange(ShoppingCartViewModel.ListCart);

        _unitOfWork.Save();

        HttpContext.Session.SetInt32(SD.Session_ShoppingCart, 0);

        if (stripeToken is null)
        {
            ShoppingCartViewModel.OrderHeader.PaymentStatus = SD.PaymentStatus_DelayedPayment;
            ShoppingCartViewModel.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
            ShoppingCartViewModel.OrderHeader.OrderStatus = SD.OrderStatus_Approved;
        }
        else
        {
            // process the payment
            var options = new ChargeCreateOptions
            {
                Amount = Convert.ToInt32(ShoppingCartViewModel.OrderHeader.OrderTotal * 100),
                Currency = "usd",
                Description = "Order ID : " + ShoppingCartViewModel.OrderHeader.Id,
                Source = stripeToken
            };

            var service = new ChargeService();
            Charge charge = service.Create(options);

            if (charge.BalanceTransactionId is null)
            {
                ShoppingCartViewModel.OrderHeader.PaymentStatus = SD.PaymentStatus_Rejected;
            }
            else
            {
                ShoppingCartViewModel.OrderHeader.TransactionId = charge.BalanceTransactionId;
            }
            if (charge.Status.ToLower() == "succeeded")
            {
                ShoppingCartViewModel.OrderHeader.PaymentStatus = SD.PaymentStatus_Approved;
                ShoppingCartViewModel.OrderHeader.OrderStatus = SD.OrderStatus_Approved;
                ShoppingCartViewModel.OrderHeader.PaymentDate = DateTime.Now;
            }
        }

        _unitOfWork.Save();

        return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartViewModel.OrderHeader.Id });
    }

    public IActionResult OrderConfirmation(int id)
    {
        OrderHeader orderHeader = _unitOfWork.OrderHeaders.GetFirstOrDefault(oh => oh.Id == id);
        TwilioClient.Init(_twilioOptions.AccountSid, _twilioOptions.AuthToken);
        try
        {
            var message = MessageResource.Create(
                body: "Order placed on Bulky Book. Your Order ID: " + id,
                from: new Twilio.Types.PhoneNumber(_twilioOptions.PhoneNumber),
                to: new Twilio.Types.PhoneNumber(orderHeader.PhoneNumber));
        }
        catch (Exception)
        {
            throw;
        }

        return View(id);
    }
}