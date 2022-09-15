using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UserService.Misc
{
    public class StandardResponseObjectResult : ObjectResult
    {
        public StandardResponseObjectResult(object value, int statusCode) : base(value)
        {
            StatusCode = statusCode;            
        }       
    }
}
