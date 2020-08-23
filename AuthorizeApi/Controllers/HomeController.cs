using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizeApi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        public IActionResult Authenticate()
        {
            var grandmaClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Bob"),
                new Claim(ClaimTypes.Email, "bob@fmail.com"),
                new Claim("Grandma.Says", "very nice boy"),
            };
            
            var licenseClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, "bob@gmail.com"),
                new Claim("DrivingLicense", "A+")
            };
            
            
            var grandmaIdentity = new ClaimsIdentity(grandmaClaims, "Grandma Identity");
            var licenseIdentity = new ClaimsIdentity(licenseClaims, "Government Identity");
            
            var userPrincipal = new ClaimsPrincipal(new []{ grandmaIdentity, licenseIdentity });

            HttpContext.SignInAsync(userPrincipal);
            // return View();
            return RedirectToAction("Index");
        }
    }
}