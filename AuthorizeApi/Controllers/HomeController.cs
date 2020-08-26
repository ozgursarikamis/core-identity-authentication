using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
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

        [Authorize(Policy = "Claim.DoB")]
        public IActionResult SecretPolicy()
        {
            return View("Secret");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult SecretRole()
        {
            return View("Secret");
        }

        public IActionResult Authenticate()
        {
            var grandmaClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Bob"),
                new Claim(ClaimTypes.Email, "bob@fmail.com"),
                new Claim(ClaimTypes.DateOfBirth, "11/11/2000"),
                new Claim(ClaimTypes.Role, "Admin"),
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

        public async Task<IActionResult> DoStuff(
            [FromServices] IAuthorizationService authorizationService
        ){
            
            var builder = new AuthorizationPolicyBuilder("Schema");
            var customPolicy = builder.RequireClaim("Hello").Build();

            var authResult = await authorizationService.AuthorizeAsync(HttpContext.User, customPolicy);
            if (authResult.Succeeded)
            {
                return View("Index");
            }
            return View("Index");
        }
    }
}