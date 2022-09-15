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

namespace UserService.Controllers
{
    [Route("api/profile")]
    [ApiController]
    [ServiceFilter(typeof(TokenAuthorizationActionFilter))]
    public partial class AvatarController : Controller
    {      
        private IAvatarRepo _avatarRepo;
        private IStandardHelper _helper;        

        public AvatarController(IAvatarRepo avatarRepo, IStandardHelper helper)
        {
            _avatarRepo = avatarRepo;
            _helper = helper;
        }       

        [HttpGet]
        [Route("avatar")]
        [AllowAnonymous]
        public IActionResult Fetch ()
        {
            ApiResponse response = new ApiResponse();

            string header_value = HttpContext.Request.Headers?["X-Authorization"].ToString();

            string token = User != null ? _helper.GetTokenFromIdentity() : string.Empty;

            //if token is empty then something went wrong, return error
            if (String.IsNullOrEmpty(token))
            {
                response.MessageCode = ApiMessageCodes.NotFound;
                response.Message = "Error getting token from identity";

                return new StandardResponseObjectResult(response, StatusCodes.Status400BadRequest);
            }
                
            return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
                   
        }

        [HttpPost]
        [Route("avatar")]
        public IActionResult Post()
        {
            ApiResponse response = new ApiResponse();

            string token = User != null ? _helper.GetTokenFromIdentity() : string.Empty;

            //if token is empty then something went wrong, return error
            if (String.IsNullOrEmpty(token))
            {
                response.MessageCode = ApiMessageCodes.NotFound;
                response.Message = "Error getting token from identity";

                return new StandardResponseObjectResult(response, StatusCodes.Status400BadRequest);
            }           

            return new StandardResponseObjectResult(response, StatusCodes.Status200OK);

        }
    }
}
