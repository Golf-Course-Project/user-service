using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

using UserService.Misc;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace UserService.Helpers
{  
    public class StandardHelper : IStandardHelper
    {
        private IOptions<AppSettings> _appSettings;
        private IHttpContextAccessor _httpContextAccessor;   
        private readonly int _daysToExpire = 7;

        public int DaysToExpire
        {
            get { return _daysToExpire; }
        }

        public StandardHelper(IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor)
        {
            _appSettings = appSettings;
            _httpContextAccessor = httpContextAccessor;       
        }

        public string GetNewId
        {
            get
            {
                return System.Guid.NewGuid().ToString().ToLower();
            }
        }

        public DateTime GetDateTime
        {
            get { return DateTime.Now; }
        }

        public AppSettings AppSettings 
        { 
            get 
            { 
                return _appSettings.Value; 
            } 
        }        
    }

    public interface IStandardHelper
    {
        int DaysToExpire { get; }
        string GetNewId { get; }
        DateTime GetDateTime { get; }
        AppSettings AppSettings { get;  }   
    }
    
}
