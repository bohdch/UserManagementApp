using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using UserManagement.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace UserManagement.Controllers
{
    [Authorize(AuthenticationSchemes = "UserAccount")]
    public class PanelController : Controller
    {
        [Route("{controller}/{action=Home}")]
        public IActionResult Home()
        {
            string userName = HttpContext.User.Identity.Name;
            string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = new User { Name = userName, Id = userId};
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("UserAccount");
            return RedirectToAction("Index", "Account");
        }
    }
}

