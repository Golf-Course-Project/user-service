
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

namespace UserService.Repos
{
    public class AvatarRepo : IAvatarRepo, IDisposable
    {        
        private IStandardHelper _standardHelper;
        private string _storageConnectionString;
        private string _storageContainerName;

        public AvatarRepo(IStandardHelper standardHelper)
        {
            _standardHelper = standardHelper;
            _storageConnectionString = _standardHelper.AppSettings.StorageConnectionString;
            _storageContainerName = "profile-avatars";
    }

        public string Fetch(string id)
        {
            Uri accountUri = new Uri(this._storageConnectionString);
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);

            return "";
        }

        public void Store(string id)
        {
            string fileName = Path.GetFileName("c:/temp/myfile.txt");

            Uri accountUri = new Uri(this._storageConnectionString);
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            container.CreateIfNotExists();

            BlobClient blob = container.GetBlobClient(id);

            blob.Upload(fileName);

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
        //User Fetch(string id);        
        void Store(string id);     
        void Delete(string id);
    }
}
