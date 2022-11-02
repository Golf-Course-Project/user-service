
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
using UserService.ViewModels.Identity;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using UserService.Repos.Identity;

namespace UserService.Repos
{
    public class AvatarRepo : IAvatarRepo, IDisposable
    {        
        private IStandardHelper _standardHelper;
        private string _storageConnectionString;
        private string _storageContainerName;
        private IIdentityRepo _identityRepo;

        public AvatarRepo(IIdentityRepo identityRepo, IStandardHelper standardHelper)
        {
            _identityRepo = identityRepo;
            _standardHelper = standardHelper;
            _storageConnectionString = _standardHelper.AppSettings.StorageConnectionString;
            _storageContainerName = "profile-avatars";           
        }

        public string FetchBlobUrl(string id)
        {
           return _identityRepo.Fetch(id).Avatar_Url;
        }

        public string StoreBlob(string id)
        {
            string filePath = @"C:/temp/dan-128.png";

            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            container.CreateIfNotExists();

            BlobClient blob = container.GetBlobClient($"{id}/dan-128.png");

            blob.DeleteIfExists(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots);
            blob.Upload(filePath);

            BlobProperties props = blob.GetProperties();

            if (props.ContentLength > 0) return blob.Uri.ToString();

            return null;          
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
        string FetchBlobUrl(string id);        
        string StoreBlob(string id);    
        void Delete(string id);
    }
}
