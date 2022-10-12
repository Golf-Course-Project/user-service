
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Collections.Generic;

using UserService.Data;
using UserService.Entities.Identity;
using Microsoft.Data.SqlClient;
using UserService.Helpers;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Server.IIS.Core;
using System.IO;
using Azure.Storage.Blobs.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using UserService.ViewModels.Internal;
using System.Text;
using UserService.Misc;

namespace UserService.Repos
{
    public class AvatarRepo : IAvatarRepo, IDisposable
    {        
        private IStandardHelper _standardHelper;
        private string _storageConnectionString;
        private string _storageContainerName;
        private string _identityServiceUrl;

        public AvatarRepo(IStandardHelper standardHelper)
        {
            _standardHelper = standardHelper;
            _storageConnectionString = _standardHelper.AppSettings.StorageConnectionString;
            _storageContainerName = "profile-avatars";
            _identityServiceUrl = _standardHelper.AppSettings.IdentityService;
    }

        public string Fetch(string id)
        {           
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);

            var results = container.GetBlobsAsync(BlobTraits.None, BlobStates.None, id);

            return "";
        }

        public string Store(string id)
        {
            string filePath = @"C:/temp/myfile.txt";

            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            container.CreateIfNotExists();

            BlobClient blob = container.GetBlobClient($"{id}/myfile.txt");

            blob.DeleteIfExists(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots);
            blob.Upload(filePath);

            BlobProperties props = blob.GetProperties();

            if (props.ContentLength > 0) return blob.Uri.ToString();

            return null;          
        }

        public ApiResponse Update(string uri, string jwtHeader)
        {
            ApiResponse result = new ApiResponse() { Success = false };

            if (string.IsNullOrEmpty(jwtHeader))
            {
                result.Message = "JWT cannot be null or empty";
                return result;
            }

            if (string.IsNullOrEmpty(uri))            {
                result.Message = "Uri cannot be null or empty";

                return result;
            }

            // split the header token to get Bearer and token value
            string[] splitToken = jwtHeader.Split(' ');
          
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(@"https://" + _identityServiceUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(splitToken[0], splitToken[1]);

                var data = new StringContent(uri, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PatchAsync("/api/users/update/avatarurl", data).Result;

                string json = response.Content.ReadAsStringAsync().Result;
                result = JsonConvert.DeserializeObject<ApiResponse>(json);

                client.Dispose();

                return result;
            }
        }

        public void Delete(string id)
        {           

           
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AvatarRepo()
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

    public interface IAvatarRepo
    {
        string Fetch(string id);        
        string Store(string id);
        ApiResponse Update(string uri, string jwt);
        void Delete(string id);
    }
}
