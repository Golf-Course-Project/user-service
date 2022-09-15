using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using UserService.Enums;
using UserService.Repos.Identity;
using UserService.ViewModels.Internal;

namespace UserService.Misc
{
    public class TokenAuthorization : ITokenAuthorization
    {
        private IIdentityRepo _identityRepo;      

        public TokenAuthorization(IIdentityRepo identityRepo)
        {
            _identityRepo = identityRepo;          
        }

        public ApiResponse ValidateToken(string header)
        {
            ApiResponse response = new ApiResponse()
            {
                Success = false,
                MessageCode = ApiMessageCodes.AuthFailed
            };           
          
            // if the header valu is empty, somethign went wrong, return a 401
            if (string.IsNullOrEmpty(header))
            {
                response.Message = "Missing bearer token from X-Authorization header";  
                return response;
            }

            // check to make sure "bearer" is part of the string
            if (! header.ToLower().Contains("bearer"))
            {
                response.Message = "Missing bearer token from X-Authorization header";
                return response;
            }

            string[] splitToken = header.Split(' ');
            string jwt = splitToken[1];

            //make sure jwt is not empty
            if (String.IsNullOrEmpty(jwt))
            {
                response.Message = "Missing bearer token from X-Authorization header";
                return response;
            }

            // validate jwt against the identity service
            ValidateTokenResponse valTokenResponse = _identityRepo.ValidateJwt(jwt);

            // if it is not successully validated, return 401
            if (valTokenResponse == null || valTokenResponse.Success != true)
            {
                response.Message = "Error validating jwt: " + response.Message + "'";
                return response;
            }

            // if everything was successfull
            response.Success = true;
            response.Message = "Success";
            response.MessageCode = ApiMessageCodes.Success;
            response.Value = valTokenResponse.Value;

            return response;

        }
    }

    public interface ITokenAuthorization
    {
        ApiResponse ValidateToken(string header);
    }
}
