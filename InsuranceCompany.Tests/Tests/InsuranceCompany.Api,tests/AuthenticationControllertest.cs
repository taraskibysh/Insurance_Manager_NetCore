using FluentAssertions;
using InsuranceCompany.Api.Controllers;
using InsuranceCompany.Application.Services.Authentication;
using InsuranceCompany.Contracts.Authentication;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace InsuranceCompany.Tests.InsuranceCompany.Api.Tests
{
    [TestFixture]
    public class AuthenticationControllerTests
    {
        private Mock<IAuthenticationService> _mockAuthenticationService;
        private AuthenticationController _controller;

        [SetUp]
        public void Setup()
        {
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            _controller = new AuthenticationController(_mockAuthenticationService.Object);
        }

        [Test]
        public async Task Register_Should_ReturnOKResponse_WhenProvidedCorrectUserInformation()
        {
            // Arrange
            var request = new RegisterRequest("Bob", "Simpson", "Bob@gmail.com", "123456");
            var authResult = new AuthenticationResult(
                Guid.NewGuid(),
                "Bob",
                "Simpson",
                "Bob@gmail.com",
                "dummy_token"
            );

            _mockAuthenticationService
                .Setup(s => s.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(authResult);

            // Act
            var result = await _controller.Register(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.Value.Should().BeOfType<AuthenticationResponce>();

            var response = okResult.Value as AuthenticationResponce;
            response.FirstName.Should().Be(authResult.FirstName);
            response.Email.Should().Be(authResult.Email);
        }

        [Test]
        public async Task Register_Should_ReturnBadRequest_WhenUserAlreadyExists()
        {
            // Arrange
            var request = new RegisterRequest("Bob", "Simpson", "Bob@gmail.com", "123456");
            _mockAuthenticationService
                .Setup(s => s.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("User already exists"));

            // Act
            var result = await _controller.Register(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();

        }

        [Test]
        public async Task Login_Should_ReturnOKResponse_WhenCredentialsAreValid()
        {
            // Arrange
            var request = new LoginRequest("Bob@gmail.com", "123456");
            var authResult = new AuthenticationResult(
                Guid.NewGuid(),
                "Bob",
                "Simpson",
                "Bob@gmail.com",
                "dummy_token"
            );

            _mockAuthenticationService
                .Setup(s => s.Login(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(authResult);

            // Act
            var result = await _controller.Login(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            

            var response = okResult.Value as AuthenticationResponce;
            response.token.Should().Be(authResult.Token);
        }

        [Test]
        public async Task Login_Should_ReturnUnauthorized_WhenPasswordIsInvalid()
        {
            // Arrange
            var request = new LoginRequest("Bob@gmail.com", "wrongpassword");
            _mockAuthenticationService
                .Setup(s => s.Login(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid password"));

            // Act
            var result = await _controller.Login(request);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
            
        }
    }
}
