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
using UserService.Misc;
using UserService.Enums;

namespace UserService.Tests.Controllers
{
    [TestClass]
    public class TestControllerTest
    {         
        private Mock<IStandardHelper> _mockHelper;    

        private TestController _controller;  

        [TestInitialize]
        public void TestInitialize()        {
                              
            _mockHelper = new Mock<IStandardHelper>();               
            
            // arrange
            _controller = new TestController(_mockHelper.Object);          
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(2)]
        public void SimpleFetch()
        {
            // arrange           

            // act
            IActionResult result = _controller.SimpleFetch();
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status200OK, standardResponse.StatusCode);
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual("Success", apiResponse.Message);
            Assert.AreEqual(ApiMessageCodes.Success, apiResponse.MessageCode);
            Assert.AreEqual("Simple", apiResponse.Value);            
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(2)]
        public void SimpleAuthFetch()
        {
            // arrange           

            // act
            IActionResult result = _controller.SimpleAuthFetch();
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status200OK, standardResponse.StatusCode);
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual("Success", apiResponse.Message);
            Assert.AreEqual(ApiMessageCodes.Success, apiResponse.MessageCode);
            Assert.AreEqual("Simple Auth", apiResponse.Value);
        }

    }
}
