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

namespace UserService.Tests.Controllers
{
    [TestClass]
    public class AvatarControllerTest
    {
        private Mock<IAvatarRepo> _mockAvatarRepo;
        private Mock<IPrincipal> _mockPrinciple;
        private Mock<IStandardHelper> _mockHelper;

        private AvatarController _controller;
        private ControllerContext _controllerContext;

        private readonly string _userId = "1E51141F-852B-4157-A73A-6CBA4DF76B0D";

        private List<Claim> _claims;
        private ClaimsIdentity _claimsIdentity;
        private ClaimsPrincipal _claimsPrinciple;

        [TestInitialize]
        public void TestInitialize()
        {

            _mockAvatarRepo = new Mock<IAvatarRepo>();
            _mockPrinciple = new Mock<IPrincipal>();
            _mockHelper = new Mock<IStandardHelper>();

            _claims = new List<Claim>() { new Claim(ClaimTypes.Name, _userId) };
            _claimsIdentity = new ClaimsIdentity(_claims);
            _claimsPrinciple = new ClaimsPrincipal(_claimsIdentity);

            // arrange
            _controller = new AvatarController(_mockAvatarRepo.Object, _mockHelper.Object);
            _mockPrinciple.Setup(x => x.Identity).Returns(_claimsIdentity);
            _mockPrinciple.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);

            _controllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext { User = _claimsPrinciple }
            };

            // Then set it to controller before executing test
            _controller.ControllerContext = _controllerContext;
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void Store_Success()
        {
            // arrange     
            

        }

    }
}
