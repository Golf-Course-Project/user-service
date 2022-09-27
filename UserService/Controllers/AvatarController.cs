using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

using UserService.Misc;
using UserService.Repos.Identity;
using UserService.Enums;
using UserService.Helpers;
using UserService.Repos;
using Microsoft.Extensions.Caching.Memory;
using UserService.ViewModels.Internal;

namespace UserService.Controllers
{
    [Route("api/profile")]
    [ApiController]
    [ServiceFilter(typeof(TokenAuthorizationActionFilter))]
    public partial class AvatarController : Controller
    {      
        private IAvatarRepo _avatarRepo;
        private ITokenAuthorization _tokenAuthorization;
        private IStandardHelper _helper;        

        public AvatarController(IAvatarRepo avatarRepo, IStandardHelper helper, ITokenAuthorization tokenAuthorization)
        {
            _avatarRepo = avatarRepo;
            _helper = helper;
            _tokenAuthorization = tokenAuthorization;
        }

        [HttpGet]
        [Route("avatar")]
        [AllowAnonymous]
        public IActionResult Fetch()
        {
            ApiResponse response = new ApiResponse() 
            { 
                Success = false,
                MessageCode = ApiMessageCodes.AuthFailed
            };

            string header_value = HttpContext.Request.Headers?["X-Authorization"].ToString();

            // go validate the x-authorization header value
            ApiResponse authResponse = _tokenAuthorization.ValidateToken(header_value);

            //if token is empty then something went wrong, return error
            if (authResponse.Success == false) return new StandardResponseObjectResult(response, StatusCodes.Status401Unauthorized);
            UserTokenValue userTokenValue = (UserTokenValue)authResponse.Value;
                          
            return new StandardResponseObjectResult(response, StatusCodes.Status200OK);                   
        }

        //[HttpPost]
        //[Route("avatar")]
        //public IActionResult Post()
        //{
        //    ApiResponse response = new ApiResponse();

        //    string token = User != null ? _helper.GetTokenFromIdentity() : string.Empty;

        //    //if token is empty then something went wrong, return error
        //    if (String.IsNullOrEmpty(token))
        //    {
        //        response.MessageCode = ApiMessageCodes.NotFound;
        //        response.Message = "Error getting token from identity";

        //        return new StandardResponseObjectResult(response, StatusCodes.Status400BadRequest);
        //    }           

        //    return new StandardResponseObjectResult(response, StatusCodes.Status200OK);

        //}
    }
}
