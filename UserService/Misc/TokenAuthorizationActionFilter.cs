using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Reflection;

using UserService.Repos.Identity;
using UserService.Enums;
using UserService.ViewModels.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace UserService.Misc
{
    public class TokenAuthorizationActionFilter : IActionFilter
    {
        private IIdentityRepo _identityRepo;
        private ITokenAuthorization _tokenAuthorization;

        public TokenAuthorizationActionFilter(IIdentityRepo identityRepo, ITokenAuthorization tokenAuthorization)
        {
            _identityRepo = identityRepo;
            _tokenAuthorization = tokenAuthorization;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            ApiResponse response = new ApiResponse()
            {
                Success = false,
                MessageCode = ApiMessageCodes.AuthFailed
            };

            // if the method allows anonoymous access then return success and skip token validation logic
            if (MethodIsAllowAnonymousAuthorization(context)) return;

            // get x-authorization header value
            string header_value = context.HttpContext.Request.Headers?.FirstOrDefault(x => x.Key.Equals("X-Authorization", StringComparison.OrdinalIgnoreCase)).Value;

            // go validate the x-authorization header value
            response = _tokenAuthorization.ValidateToken(header_value);

            // if not successfull, return a 401
            if (response.Success == false)
            {
                context.Result = new StandardResponseObjectResult(response, StatusCodes.Status401Unauthorized);
                return;
            }

            return;            

            //var handler = new JwtSecurityTokenHandler().ReadJwtToken(jwt);
            //var userProfileId = handler.Claims.First(x => x.Type == "unique_name").Value;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do something after the action executes.
        }

        private static bool MethodIsAllowAnonymousAuthorization(ActionExecutingContext context)
        {
            if (context == null) { return false; }

            ControllerActionDescriptor actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            bool results = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute)).Any();

            return results;
        }

    }
}
