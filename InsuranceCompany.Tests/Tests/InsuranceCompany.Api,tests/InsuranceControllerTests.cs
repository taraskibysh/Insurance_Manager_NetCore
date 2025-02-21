using FluentAssertions;
using InsuranceCompany.Api.Controllers;
using InsuranceCompany.Application.Services.Business;
using InsuranceCompany.Contracts.Model;
using InsuranceCompany.Contracts.Models;
using InsuranceCompany.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InsuranceCompany.Tests.Tests.InsuranceCompany.Api_tests
{
    public class InsuranceControllerTests
    {
        private Mock<IInsuranceService> _mockInsuranceService;
        private InsuranceController _controller;

        [SetUp]
        public void Setup()
        {
            _mockInsuranceService = new Mock<IInsuranceService>();
            _controller = new InsuranceController(_mockInsuranceService.Object);
        }

        [Test]
        public async Task Delete_Should_ReturnOKResponse_WhenInsuranceDeletedSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockInsuranceService
                .Setup(s => s.DeleteInsurance(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task Delete_Should_ReturnBadRequest_WhenInsuranceDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockInsuranceService
                .Setup(s => s.DeleteInsurance(It.IsAny<Guid>()))
                .ThrowsAsync(new InvalidOperationException("Insurance not found"));

            // Act
            var result = await _controller.Delete(id);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task GetInsurances_Should_ReturnInsurance_WhenIdIsValid()
        {



            // Arrange

            var userId = Guid.NewGuid();
            var claims = new List<System.Security.Claims.Claim>
            {
            new System.Security.Claims.Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", userId.ToString())
            };
            var identity = new System.Security.Claims.ClaimsIdentity(claims);
            var claimsPrincipal = new System.Security.Claims.ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var insuranceId = Guid.NewGuid();
            var insurance = new Insurance
            {
                Id = insuranceId,
                TypeOfInsurance = TypeOfInsurance.HealthInsurance,
                Pay = 1000,
                Price = 10000,
                Status = InsuranceStatus.Active,
                MethodOfInsurance = MethodOfInsurance.FullInsurance,
                UserId = userId
            };

            var fullName = "John Doe";

            _mockInsuranceService
                .Setup(s => s.GetInsurance(It.IsAny<Guid>()))
                .ReturnsAsync(insurance);

            _mockInsuranceService
                .Setup(s => s.GetFullName(It.IsAny<Guid>()))
                .ReturnsAsync(fullName);

            // Act
            var result = await _controller.GetInsurances(insuranceId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }


        [Test]
        public async Task AddInsurance_Should_ReturnOK_WhenInsuranceAddedSuccessfully()
        {
            var userId = Guid.NewGuid();
            var claims = new List<System.Security.Claims.Claim>
    {
            new System.Security.Claims.Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", userId.ToString())
    };
            var identity = new System.Security.Claims.ClaimsIdentity(claims);
            var claimsPrincipal = new System.Security.Claims.ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
            // Arrange
            var request = new InsuranceRequestModel
            {
                Type = TypeOfInsurance.HouseInsurance,
                Method = MethodOfInsurance.FullInsurance,
                Status = InsuranceStatus.Active,
                Price = 10000
            };

            var fake = new Insurance
            {
                Id = Guid.NewGuid(),
                TypeOfInsurance = request.Type,
                MethodOfInsurance = request.Method,
                Status = request.Status,
                Price = request.Price,
                UserId = userId
            };

            _mockInsuranceService
                .Setup(s => s.AddInsurance(It.IsAny<TypeOfInsurance>(), It.IsAny<MethodOfInsurance>(), It.IsAny<InsuranceStatus>(), It.IsAny<decimal>(), It.IsAny<Guid>()))
                .ReturnsAsync(fake);

            // Act
            var result = await _controller.AddInsurance(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().Be("Insurance added successfully.");
        }

        [Test]
        public async Task GetAllInsurances_Should_ReturnInsurancesList_WhenDataExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var claims = new List<System.Security.Claims.Claim>
    {
        new System.Security.Claims.Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", userId.ToString())
    };
            var identity = new System.Security.Claims.ClaimsIdentity(claims);
            var claimsPrincipal = new System.Security.Claims.ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var insurances = new List<Insurance>
    {
        new Insurance { Id = Guid.NewGuid(), TypeOfInsurance = TypeOfInsurance.HealthInsurance, Price = 10000, UserId = userId },
        new Insurance { Id = Guid.NewGuid(), TypeOfInsurance = TypeOfInsurance.CarInsurance, Price = 5000, UserId = userId }
    };

            _mockInsuranceService
                .Setup(s => s.GetAllInsurances(It.IsAny<Guid>()))
                .ReturnsAsync(insurances);

            // Act
            var result = await _controller.GetAllInsurances();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as List<Insurance>;
            response.Should().NotBeNull();
            response!.Count.Should().Be(insurances.Count);
        }


    }
}
