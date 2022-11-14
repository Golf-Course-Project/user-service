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
using UserService.Repos.Identity;
using System.IO;

namespace UserService.Tests.Controllers
{
    [TestClass]
    public class ProfileControllerTest
    {
        private Mock<IAvatarRepo> _mockAvatarRepo;
        private Mock<IIdentityRepo> _mockIdentityRepo;
        private Mock<IStandardHelper> _mockHelper;
        private Mock<ITokenAuthorization> _mockTokenAuthorization;

        private ProfileController _controller;
        private ControllerContext _controllerContext;

        private readonly string _userId = "4b5926a7-c730-41d6-8813-a77df4910dc4";
        private readonly string _token = "d7bebe0b-991c-41da-bbb6-6d5c8caded0d";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockAvatarRepo = new Mock<IAvatarRepo>();
            _mockIdentityRepo = new Mock<IIdentityRepo>();
            _mockHelper = new Mock<IStandardHelper>();
            _mockTokenAuthorization = new Mock<ITokenAuthorization>();
                       
            // arrange
            _controller = new ProfileController(_mockAvatarRepo.Object, _mockIdentityRepo.Object, _mockHelper.Object, _mockTokenAuthorization.Object);
           
            DefaultHttpContext httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Authorization"] = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImY3ZmQ0YjQwLWNhZjQtNDU2Ny1iODg5LTE2MTlhYWQxNDM0MSIsImVtYWlsIjoiZGFuaGVsbGVtQG91dGxvb2suY29tIiwicm9sZSI6InNpdGUgYWRtaW4iLCJ0b2tlbiI6IjQzMDE2MDY3LTBlMDItNGExYS1iZGE5LTdlOTc1Njc0NWUzZCIsIm5iZiI6MTY2Mzc2ODE3NSwiZXhwIjoxNjY2MzYwMTc1LCJpYXQiOjE2NjM3NjgxNzV9.QqVMCXj7MOFM1hdEYWyytix2TKzAlfVx0GQlP-8KbFU";

            _controllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Then set it to controller before executing test
            _controller.ControllerContext = _controllerContext;
        }

        private Mock<IFormFile> GetMockFile()
        {
            var mockFile = new Mock<IFormFile>();
            var conent = "Hello world from fake file";
            var fileName = "test.png";
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            sw.Write(conent);
            sw.Flush();
            ms.Position = 0;
            mockFile.Setup(x => x.OpenReadStream()).Returns(ms);
            mockFile.Setup(x => x.FileName).Returns(fileName);
            mockFile.Setup(x => x.Length).Returns(ms.Length);

            return mockFile;
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void FetchAvatar_Success()
        {
            // arrange
            ApiResponse validateTokenResponse = new ApiResponse() { 
                Success = true, 
                Message = "Success", 
                MessageCode = ApiMessageCodes.Success, 
                Value = new UserTokenValue { Token = _token, UserId = _userId } 
            };

            string url = "https://alystorage.blob.core.windows.net/profile-avatars/f7fd4b40-caf4-4567-b889-1619aad14341/dan-128x.png";

            _mockTokenAuthorization.Setup(x => x.ValidateToken(It.IsAny<string>())).Returns(validateTokenResponse);
            _mockAvatarRepo.Setup(x => x.FetchBlobUrl(It.IsAny<string>())).Returns(url);

            // act
            IActionResult result = _controller.FetchAvatar();
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status200OK, standardResponse.StatusCode);
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual("Success", apiResponse.Message);
            Assert.AreEqual(ApiMessageCodes.Success, apiResponse.MessageCode);
            Assert.AreEqual(url, apiResponse.Value);

            _mockTokenAuthorization.Verify(x => x.ValidateToken(It.IsAny<string>()), Times.Once());
            _mockAvatarRepo.Verify(x => x.FetchBlobUrl(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void FetchAvatar_FailedAuth()
        {
            // arrange
            ApiResponse validateTokenResponse = new ApiResponse()
            {
                Success = false,                
                MessageCode = ApiMessageCodes.AuthFailed,
                Value = null
            };

            string url = String.Empty;

            _mockTokenAuthorization.Setup(x => x.ValidateToken(It.IsAny<string>())).Returns(validateTokenResponse);
           
            // act
            IActionResult result = _controller.FetchAvatar();
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status401Unauthorized, standardResponse.StatusCode);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual(String.Empty, apiResponse.Message);
            Assert.AreEqual(ApiMessageCodes.AuthFailed, apiResponse.MessageCode);
            Assert.AreEqual(null, apiResponse.Value);

            _mockTokenAuthorization.Verify(x => x.ValidateToken(It.IsAny<string>()), Times.Once());
            _mockAvatarRepo.Verify(x => x.FetchBlobUrl(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void FetchAvatar_EmptyAvatarUrl()
        {
            // arrange
            ApiResponse validateTokenResponse = new ApiResponse()
            {
                Success = true,
                Message = "Success",
                MessageCode = ApiMessageCodes.Success,
                Value = new UserTokenValue { Token = _token, UserId = _userId }
            };

            string url = String.Empty;

            _mockTokenAuthorization.Setup(x => x.ValidateToken(It.IsAny<string>())).Returns(validateTokenResponse);
            _mockAvatarRepo.Setup(x => x.FetchBlobUrl(It.IsAny<string>())).Returns(url);

            // act
            IActionResult result = _controller.FetchAvatar();
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status200OK, standardResponse.StatusCode);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual("Avatar url not found", apiResponse.Message);
            Assert.AreEqual(ApiMessageCodes.NotFound, apiResponse.MessageCode);
            Assert.AreEqual(null, apiResponse.Value);

            _mockTokenAuthorization.Verify(x => x.ValidateToken(It.IsAny<string>()), Times.Once());
            _mockAvatarRepo.Verify(x => x.FetchBlobUrl(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void PostAvatar_Success()
        {
            // arrange
            ApiResponse validateTokenResponse = new ApiResponse()
            {
                Success = true,
                Message = "Success",
                MessageCode = ApiMessageCodes.Success,
                Value = new UserTokenValue { Token = _token, UserId = _userId }
            };

            User user = new User() { Id = _userId };

            Mock<IFormFile> mockFile = this.GetMockFile();

            string url = "https://alystorage.blob.core.windows.net/profile-avatars/f7fd4b40-caf4-4567-b889-1619aad14341/dan-128x.png";

            _mockTokenAuthorization.Setup(x => x.ValidateToken(It.IsAny<string>())).Returns(validateTokenResponse);
            _mockIdentityRepo.Setup(x => x.Fetch(It.IsAny<String>())).Returns(user);
            _mockAvatarRepo.Setup(x => x.StoreBlob(_userId, It.IsAny<String>(), It.IsAny<MemoryStream>())).Returns(url);
            _mockHelper.Setup(x => x.GetDateTime).Returns(DateTime.Now);
            _mockIdentityRepo.Setup(x => x.SaveChanges()).Returns(1);

            // act
            IActionResult result = _controller.PostAvatar(mockFile.Object);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status202Accepted, standardResponse.StatusCode);
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual("Success", apiResponse.Message);
            Assert.AreEqual(ApiMessageCodes.Updated, apiResponse.MessageCode);            

            _mockTokenAuthorization.Verify(x => x.ValidateToken(It.IsAny<string>()), Times.Once());
            _mockIdentityRepo.Verify(x => x.Fetch(It.IsAny<string>()), Times.Once());
            _mockAvatarRepo.Verify(x => x.StoreBlob(It.IsAny<string>(), It.IsAny<String>(), It.IsAny<MemoryStream>()), Times.Once());
            _mockHelper.Verify(x => x.GetDateTime, Times.Once());
            _mockIdentityRepo.Verify(x => x.Update(user, "Avatar_Url, DateUpdated"), Times.Once());
            _mockIdentityRepo.Verify(x => x.SaveChanges(), Times.Once());
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void PostAvatar_FailedAuth()
        {
            // arrange
            ApiResponse validateTokenResponse = new ApiResponse()
            {
                Success = false,
                MessageCode = ApiMessageCodes.AuthFailed,
                Value = null
            };

            Mock<IFormFile> mockFile = this.GetMockFile();
            _mockTokenAuthorization.Setup(x => x.ValidateToken(It.IsAny<string>())).Returns(validateTokenResponse);
           
            // act
            IActionResult result = _controller.PostAvatar(mockFile.Object);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status401Unauthorized, standardResponse.StatusCode);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual("", apiResponse.Message);
            Assert.AreEqual(ApiMessageCodes.AuthFailed, apiResponse.MessageCode);

            _mockTokenAuthorization.Verify(x => x.ValidateToken(It.IsAny<string>()), Times.Once());
            _mockIdentityRepo.Verify(x => x.Fetch(It.IsAny<string>()), Times.Never());
            _mockAvatarRepo.Verify(x => x.StoreBlob(It.IsAny<string>(), It.IsAny<String>(), It.IsAny<MemoryStream>()), Times.Never());
            _mockHelper.Verify(x => x.GetDateTime, Times.Never());
            _mockIdentityRepo.Verify(x => x.Update(It.IsAny<User>(), "Avatar_Url, DateUpdated"), Times.Never());
            _mockIdentityRepo.Verify(x => x.SaveChanges(), Times.Never());
        }
    }
}
