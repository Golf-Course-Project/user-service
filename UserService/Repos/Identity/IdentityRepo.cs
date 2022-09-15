using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

using UserService.Entities.Identity;
using UserService.Data;
using UserService.Helpers;
using UserService.ViewModels.Internal;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace UserService.Repos.Identity
{
    public class IdentityRepo : IDisposable, IIdentityRepo
    {
        private IStandardHelper _standardHelper;
        private string _identityServiceUrl;       

        public IdentityRepo(IStandardHelper standardHelper)
        {
            _standardHelper = standardHelper;
            _identityServiceUrl = _standardHelper.AppSettings.IdentityService;           
        }

        public ValidateTokenResponse ValidateJwt(string jwt)
        {
            ValidateTokenResponse result = new ValidateTokenResponse();

            if (string.IsNullOrEmpty(jwt))
            {
                result.Message = "JWT cannot be null or empty";

                return result;
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(@"https://" + _identityServiceUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

                HttpResponseMessage response = client.GetAsync("/api/auth/validate").Result;

                var json = response.Content.ReadAsStringAsync().Result;
                result = JsonConvert.DeserializeObject<ValidateTokenResponse>(json);

                client.Dispose();

                return result;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~IdentityRepo()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                
            }
        }
    }

    public interface IIdentityRepo
    {
        ValidateTokenResponse ValidateJwt(string jwt);   
    }
}
