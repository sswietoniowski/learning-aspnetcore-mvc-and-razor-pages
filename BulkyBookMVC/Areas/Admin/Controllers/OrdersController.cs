using BulkyBook.DataAccess;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace BulkyBook.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderDetailsViewModel OrderDetailsViewModel { get; set; }

        public OrdersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            OrderDetailsViewModel = new OrderDetailsViewModel
            {
                OrderHeader = _unitOfWork.OrderHeaders
                    .GetFirstOrDefault(oh => oh.Id == id, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetails
                    .GetAll(od => od.OrderId == id, includeProperties: "Product")
            };

            return View(OrderDetailsViewModel);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<OrderHeader> orderHeaders;

            if (User.IsInRole(SD.IdentityRole_Admin) || User.IsInRole(SD.IdentityRole_Employee))
            {
                orderHeaders = _unitOfWork.OrderHeaders.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                orderHeaders = _unitOfWork.OrderHeaders
                    .GetAll(o => o.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
            }

            switch (status)
            {
                case "Processing":
                    orderHeaders = orderHeaders.Where(oh => oh.OrderStatus == SD.OrderStatus_InProcess ||
                        oh.OrderStatus == SD.OrderStatus_Approved || oh.OrderStatus == SD.OrderStatus_Pending);
                    break;
                case "Pending":
                    orderHeaders = orderHeaders.Where(oh => oh.PaymentStatus == SD.PaymentStatus_DelayedPayment);
                    break;
                case "Completed":
                    orderHeaders = orderHeaders.Where(oh => oh.OrderStatus == SD.OrderStatus_Shipped);
                    break;
                case "Rejected":
                    orderHeaders = orderHeaders.Where(oh => oh.OrderStatus == SD.OrderStatus_Cancelled ||
                        oh.OrderStatus == SD.OrderStatus_Refunded || oh.PaymentStatus == SD.PaymentStatus_Rejected);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeaders });
        }

        #endregion
    }
}
