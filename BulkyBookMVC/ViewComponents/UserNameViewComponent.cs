using BulkyBook.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BulkyBook.MVC.ViewComponents
{
    public class UserNameViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserNameViewComponent(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);   
            var userFromDb = _unitOfWork.ApplicationUsers.GetFirstOrDefault(u => u.Id == claims.Value);

            return View(userFromDb);
        }
    }
}
