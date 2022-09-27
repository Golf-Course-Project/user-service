using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Security.Principal;
using System.Security.Claims;

using Moq;

using UserService.Controllers;
using UserService.Helpers;
using System.Security.Cryptography.X509Certificates;
using UserService.Repos;
using UserService.Misc;
using UserService.ViewModels.Internal;
using Azure;
using Newtonsoft.Json.Linq;
using UserService.Entities.Identity;
using UserService.Enums;

namespace UserService.Tests.Controllers
{
    [TestClass]
    public class AvatarControllerTest
    {
        private Mock<IAvatarRepo> _mockAvatarRepo;        
        private Mock<IStandardHelper> _mockHelper;
        private Mock<ITokenAuthorization> _mockTokenAuthorization;

        private AvatarController _controller;
        private ControllerContext _controllerContext;

        private readonly string _userId = "4b5926a7-c730-41d6-8813-a77df4910dc4";
        private readonly string _token = "d7bebe0b-991c-41da-bbb6-6d5c8caded0d";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockAvatarRepo = new Mock<IAvatarRepo>();
            _mockHelper = new Mock<IStandardHelper>();
            _mockTokenAuthorization = new Mock<ITokenAuthorization>();
                       
            // arrange
            _controller = new AvatarController(_mockAvatarRepo.Object, _mockHelper.Object, _mockTokenAuthorization.Object);
           
            DefaultHttpContext httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Authorization"] = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImY3ZmQ0YjQwLWNhZjQtNDU2Ny1iODg5LTE2MTlhYWQxNDM0MSIsImVtYWlsIjoiZGFuaGVsbGVtQG91dGxvb2suY29tIiwicm9sZSI6InNpdGUgYWRtaW4iLCJ0b2tlbiI6IjQzMDE2MDY3LTBlMDItNGExYS1iZGE5LTdlOTc1Njc0NWUzZCIsIm5iZiI6MTY2Mzc2ODE3NSwiZXhwIjoxNjY2MzYwMTc1LCJpYXQiOjE2NjM3NjgxNzV9.QqVMCXj7MOFM1hdEYWyytix2TKzAlfVx0GQlP-8KbFU";

            _controllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Then set it to controller before executing test
            _controller.ControllerContext = _controllerContext;
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void Fetch_Success()
        {
            // arrange
            ApiResponse validateTokenResponse = new ApiResponse() { 
                Success = true, 
                Message = "Success", 
                MessageCode = ApiMessageCodes.Success, 
                Value = new UserTokenValue { Token = _token, UserId = _userId } 
            };
          
            _mockTokenAuthorization.Setup(x => x.ValidateToken(It.IsAny<string>())).Returns(validateTokenResponse);

            // act
            IActionResult result = _controller.Fetch();
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
        }

    }
}
