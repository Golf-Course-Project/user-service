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
using System.Reflection.PortableExecutable;

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

            _avatarRepo.Fetch(userTokenValue.UserId.ToLower());

            return new StandardResponseObjectResult(response, StatusCodes.Status200OK);                   
        }

        [HttpPost]
        [Route("avatar")]
        [AllowAnonymous]
        public IActionResult Post()
        {
            ApiResponse response = new ApiResponse()
            {
                Success = false,
                MessageCode = ApiMessageCodes.AuthFailed
            };

            string jwt = HttpContext.Request.Headers?["X-Authorization"].ToString();

            // go validate the x-authorization header value
            ApiResponse authResponse = _tokenAuthorization.ValidateToken(jwt);

            //if token is empty then something went wrong, return error
            if (authResponse.Success == false) return new StandardResponseObjectResult(response, StatusCodes.Status401Unauthorized);
            UserTokenValue userTokenValue = (UserTokenValue)authResponse.Value;

            string avatar_uri = _avatarRepo.Store(userTokenValue.UserId.ToLower());

            if (string.IsNullOrEmpty(avatar_uri)) {
                response.Message = "Error uploading and getting URI";
                response.MessageCode = ApiMessageCodes.BlogStorageFailure;

                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }
           
            response = _avatarRepo.Update(avatar_uri, jwt);

            return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
        }
    }
}
