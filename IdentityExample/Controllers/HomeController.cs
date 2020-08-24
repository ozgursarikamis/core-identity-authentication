using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;

namespace IdentityExample.Controllers
{
	public class HomeController : Controller
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signinManager;
		private readonly IEmailService _emailService;

		public HomeController(
			UserManager<IdentityUser> userManager,
			SignInManager<IdentityUser> signInManager,
			IEmailService emailService
		)
		{
			_userManager = userManager;
			_signinManager = signInManager;
			_emailService = emailService;
		}
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

			var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity, licenseIdentity });

			HttpContext.SignInAsync(userPrincipal);
			// return View();
			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<IActionResult> Login(string username, string password)
		{
			var user = await _userManager.FindByNameAsync(username);
			if (user != null)
			{
				var result = await _signinManager.PasswordSignInAsync(user, password, false, false);
				if (result.Succeeded)
				{
					return RedirectToAction("Index");
				}
			}
			return RedirectToAction("Index");
		}

		public IActionResult Login() => View();
		public IActionResult Register() => View();

		[HttpPost]
		public async Task<IActionResult> Register(string username, string password)
		{
			IdentityUser user = new IdentityUser
			{
				UserName = username,
				Email = ""
			};
			var result = await _userManager.CreateAsync(user, password);
			if (result.Succeeded)
			{
				// generation of the email token
				var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
				var link = Url.Action(
					nameof(VerifyEmail),
					"Home",
					new { userId = user.Id, code },
					Request.Scheme,
					Request.Host.ToString()
				);

				var linkHtml = $"<a href='{link}'>Click To Verify</a>";
				await _emailService.SendAsync("test@test.com", "verification", linkHtml, true);

				return RedirectToAction("EmailVerification");
			}
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> VerifyEmail(string userId, string code)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
				return BadRequest();

			var confirmResult = await _userManager.ConfirmEmailAsync(user, code);

			if (confirmResult.Succeeded)
			{
				return View();
			}

			return BadRequest();
		}

		public IActionResult EmailVerification() => View();

		public async Task<IActionResult> Logout()
		{
			await _signinManager.SignOutAsync();
			return RedirectToAction("Index");
		}
	}
}