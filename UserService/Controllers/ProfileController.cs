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
using System.IO;
using UserService.ViewModels.Identity;

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
        [Route("me")]       
        public IActionResult Fetch()
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

            // fetch user
            User user = _identityRepo.Fetch(userTokenValue.UserId);

            // check to see if user object is empty
            if (user == null)
            {
                response.Message = "User profile not found";
                response.MessageCode = ApiMessageCodes.NotFound;

                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }

            // create viewmodel that can be shared and returned back
            ProfileGet vm = new ProfileGet()
            {
                Name = user.Name,
                Email = user.Email,
                Avatar_Url = user.Avatar_Url,
                IsLocked = user.IsLocked
            };

            response.Count = 1;
            response.MessageCode = ApiMessageCodes.Success;
            response.Message = "Success";
            response.Success = true;
            response.Value = vm;

            return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
        }

        [HttpPut]
        [Route("me")]
        public IActionResult Update([FromBody] ProfilePatch body)
        {
            ApiResponse response = new ApiResponse()
            {
                Success = false,
                MessageCode = ApiMessageCodes.AuthFailed
            };

            // check for valid model 
            if (!ModelState.IsValid)
            {
                response.MessageCode = ApiMessageCodes.InvalidModelState;
                response.Message = "Invalid model state";

                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }

            string jwt = HttpContext.Request.Headers?["X-Authorization"].ToString();

            // go validate the x-authorization header value
            ApiResponse authResponse = _tokenAuthorization.ValidateToken(jwt);

            // if token is empty then something went wrong, return error
            if (authResponse.Success == false) return new StandardResponseObjectResult(response, StatusCodes.Status401Unauthorized);
            UserTokenValue userTokenValue = (UserTokenValue)authResponse.Value;

            // fetch user
            User user = _identityRepo.Fetch(userTokenValue.UserId);

            // check to see if user object is empty
            if (user == null)
            {
                response.Message = "User profile not found";
                response.MessageCode = ApiMessageCodes.NotFound;
                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }

            // check to see if email is already in use
            User userByEmail = _identityRepo.FetchByEmail(body.Email);

            // if email is already in use, return error
            if (userByEmail != null && userByEmail.Email != user.Email) 
            {
                response.MessageCode = ApiMessageCodes.AlreadyExists;
                response.Message = "Email provided is in use by another user";
                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }

            // set name and email
            user.Name = body.Name.Trim();
            user.Email = body.Email.Trim();

            try
            {
                _identityRepo.Update(user, "Name,Email");
                int result = _identityRepo.SaveChanges();

                if (result != 1)
                {
                    response.MessageCode = ApiMessageCodes.Failed;
                    response.Message = "Error saving changes";
                    return new StandardResponseObjectResult(response, StatusCodes.Status500InternalServerError);
                }

                response.Count = 1;
                response.MessageCode = ApiMessageCodes.Success;
                response.Message = "Success";
                response.Success = true;
                response.Value = "";

                return new StandardResponseObjectResult(response, StatusCodes.Status202Accepted);
            } 
            catch (Exception ex)
            {
                response.Message = "Exception thrown: " + ex.Message;
                response.MessageCode = ApiMessageCodes.ExceptionThrown;

                return new StandardResponseObjectResult(response, StatusCodes.Status500InternalServerError);
            }            
        }

        [HttpPost]
        [Route("avatar")]
        public IActionResult PostAvatar([FromForm] IFormFile file)
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
                response.MessageCode = ApiMessageCodes.NotFound;
                response.Message = "User not found";

                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }
            
            string avatar_url = "";

            try
            {
                // https://www.c-sharpcorner.com/article/uploading-files-with-react-js-and-net/
                // https://stackoverflow.com/questions/54795740/access-to-fetch-at-from-origin-has-been-blocked-by-cors-policy-no-acce
                using (MemoryStream memoryStream = new MemoryStream())
                {           
                    file.CopyTo(memoryStream);                   
                    avatar_url = _avatarRepo.StoreBlob(userTokenValue.UserId.ToLower(), file.FileName, memoryStream);
                }
            }
            catch (Exception ex)
            {
                return new StandardResponseObjectResult("MemoryStream Exception: " + ex.Message, StatusCodes.Status500InternalServerError);
            }


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

                if (result == 0)
                {
                    response.Message = "Error updating user";
                    response.MessageCode = ApiMessageCodes.Failed;

                    return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
                }

                response.Message = "Success";
                response.MessageCode = ApiMessageCodes.Updated;
                response.Value = avatar_url;
                response.Success = true;

                return new StandardResponseObjectResult(response, StatusCodes.Status202Accepted);
            }
            catch (Exception ex)
            {
                return new StandardResponseObjectResult("Update Profile Exception: " + ex.Message, StatusCodes.Status500InternalServerError);
            }
            finally
            {
                user = null;
            }
        }
    }
}
