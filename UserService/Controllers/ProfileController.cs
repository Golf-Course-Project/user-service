using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

using UserService.Misc;
using UserService.Repos.Identity;
using UserService.Enums;
using UserService.Helpers;
using UserService.Repos;
using UserService.ViewModels.Internal;
using UserService.Entities.Identity;

namespace UserService.Controllers
{
    [Route("api/profile")]
    [ApiController]
    [ServiceFilter(typeof(TokenAuthorizationActionFilter))]
    public partial class ProfileController : Controller
    {      
        private IAvatarRepo _avatarRepo;
        private IIdentityRepo _identityRepo;
        private ITokenAuthorization _tokenAuthorization;
        private IStandardHelper _helper;        

        public ProfileController(IAvatarRepo avatarRepo, IIdentityRepo identityRepo, IStandardHelper helper, ITokenAuthorization tokenAuthorization)
        {
            _avatarRepo = avatarRepo;
            _identityRepo = identityRepo;   
            _helper = helper;
            _tokenAuthorization = tokenAuthorization;
        }

        [HttpGet]
        [Route("avatar")]
        [AllowAnonymous]
        public IActionResult FetchAvatarUrl()
        {
            ApiResponse response = new ApiResponse() 
            { 
                Success = false,
                MessageCode = ApiMessageCodes.AuthFailed
            };

            string jwt = HttpContext.Request.Headers?["X-Authorization"].ToString();

            // go validate the x-authorization header value
            ApiResponse authResponse = _tokenAuthorization.ValidateToken(jwt);
           
            // if token is empty then something went wrong, return error
            if (authResponse.Success == false) return new StandardResponseObjectResult(response, StatusCodes.Status401Unauthorized);
            UserTokenValue userTokenValue = (UserTokenValue)authResponse.Value;

            string url = _avatarRepo.FetchBlobUrl(userTokenValue.UserId);

            // check to see if avatar url is empty
            if (string.IsNullOrEmpty(url))
            {
                response.Message = "Avatar url not found";
                response.MessageCode = ApiMessageCodes.NotFound;

                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }

            response.Count = 1;
            response.MessageCode = ApiMessageCodes.Success;
            response.Message = "Success";
            response.Success = true;
            response.Value = url;

            return new StandardResponseObjectResult(response, StatusCodes.Status200OK);                   
        }

        [HttpPost]
        [Route("avatar")]
        public IActionResult PostAvatar()
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

            User user = _identityRepo.Fetch(userTokenValue.UserId);

            // make sure we have a user object
            if (user == null)
            {
                response.MessageCode= ApiMessageCodes.NotFound;
                response.Message = "User not found";

                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }
            
            string avatar_url = _avatarRepo.StoreBlob(userTokenValue.UserId.ToLower());

            // did we get a response after storing blob?
            if (string.IsNullOrEmpty(avatar_url))
            {
                response.Message = "Error uploading and getting URI";
                response.MessageCode = ApiMessageCodes.BlogStorageFailure;

                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }

            try 
            { 
                user.Avatar_Url = avatar_url;
                user.DateUpdated = _helper.GetDateTime;

                _identityRepo.Update(user, "Avatar_Url, DateUpdated");
                int result = _identityRepo.SaveChanges();

                if (result == 0) {
                    response.Message = "Error updating user";
                    response.MessageCode = ApiMessageCodes.Failed;

                    return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
                }

                response.Message = "Success";
                response.MessageCode = ApiMessageCodes.Updated;
                response.Value = null;
                response.Success = true;

                return new StandardResponseObjectResult(response, StatusCodes.Status202Accepted);
            }
            catch (Exception ex)
            {
                return new StandardResponseObjectResult("Exception: " + ex.Message, StatusCodes.Status500InternalServerError);
            }
            finally
            {
                user = null;
            }
        }
    }
}
