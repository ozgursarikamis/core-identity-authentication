using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizeApi.Controllers
{
    public class OperationsController : Controller
    {
        private readonly IAuthorizationService _authorizationService;
        public OperationsController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }
        public async Task<IActionResult> Open()
        {
            var cookieJar = new CookieJar(); // get cookie jar from db
            await _authorizationService.AuthorizeAsync(User, cookieJar, CookieJarOperations.Open);
            return View();
        }
    }
}