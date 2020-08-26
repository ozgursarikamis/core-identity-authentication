using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace AuthorizeApi.Controllers
{
    public static class CookieJarAuthOperations
    {        
        public static OperationAuthorizationRequirement Open = new OperationAuthorizationRequirement
        {
            Name = CookieJarOperations.Open
        };
    }
}