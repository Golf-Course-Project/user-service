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
        private IMemoryCache _memoryCache;
        private readonly int _daysToExpire = 7;

        public int DaysToExpire
        {
            get { return _daysToExpire; }
        }

        public StandardHelper(IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
        {
            _appSettings = appSettings;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
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

        public string GetTokenFromIdentity()
        {
            string jwt = _httpContextAccessor.HttpContext.Request.Headers?["X-Authorization"].ToString();
            string value = string.Empty;

            value = _memoryCache.Get<String>(jwt);
            return value;
        }
    }

    public interface IStandardHelper
    {
        int DaysToExpire { get; }
        string GetNewId { get; }
        DateTime GetDateTime { get; }
        AppSettings AppSettings { get;  }
        string GetTokenFromIdentity();
    
    }
    
}
