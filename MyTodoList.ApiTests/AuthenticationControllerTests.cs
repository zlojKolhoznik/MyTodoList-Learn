using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyTodoList.Api.Authentication.Models;
using MyTodoList.Api.Controllers;
using MyTodoList.Api.Services;
using MyTodoList.Data.Models;
using System.Reflection;

namespace MyTodoList.Api.Tests;

public class AuthenticationControllerTests
{
    private Mock<UserManager<User>> _mockUserManager;
    private Mock<IJwtService> _mockJwtService;
    private AuthenticationController _controller;

    [SetUp]
    public void Setup()
    {
        _mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null,
            null, null);

        _mockJwtService = new Mock<IJwtService>();

        _controller = new AuthenticationController(_mockUserManager.Object, _mockJwtService.Object);
    }

    [Test]
    public async Task Register_ReturnsBadRequest_WhenPasswordsDoNotMatch()
    {
        // Arrange
        var model = new RegisterDto
        {
            UserName = "testuser",
            Password = "password1",
            ConfirmPassword = "password2"
        };

        // Act
        var result = await _controller.Register(model);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(((SerializableError)badRequestResult.Value).ContainsKey("ConfirmPassword"), Is.True);
    }

    [Test]
    public async Task Register_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("UserName", "UserName is required");

        var model = new RegisterDto
        {
            UserName = "",
            Password = "password1",
            ConfirmPassword = "password1"
        };

        // Act
        var result = await _controller.Register(model);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Register_ReturnsOkWithToken_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var model = new RegisterDto
        {
            UserName = "testuser",
            Password = "password1",
            ConfirmPassword = "password1"
        };

        var user = new User { UserName = model.UserName };

        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _mockJwtService.Setup(x => x.Generate(It.IsAny<User>())).Returns("sampleToken");

        // Act
        var result = await _controller.Register(model);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        var actualToken = GetTokenFromOkObjectResult(okResult);
        Assert.That(actualToken, Is.EqualTo("sampleToken"));
    }

    [Test]
    public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
    {
        // Arrange
        var model = new RegisterDto
        {
            UserName = "testuser",
            Password = "password1",
            ConfirmPassword = "password1"
        };

        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));

        // Act
        var result = await _controller.Register(model);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Login_ReturnsUnauthorized_WhenUserNotFound()
    {
        // Arrange
        var model = new LoginDto
        {
            UserName = "nonexistentuser",
            Password = "password1"
        };

        _mockUserManager.Setup(x => x.FindByNameAsync(model.UserName))
            .ReturnsAsync((User)null);

        // Act
        var result = await _controller.Login(model);

        // Assert
        Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
        var unauthorizedResult = result as UnauthorizedObjectResult;
        Assert.That(unauthorizedResult.Value, Is.EqualTo("Incorrect user name"));
    }

    [Test]
    public async Task Login_ReturnsUnauthorized_WhenPasswordIsIncorrect()
    {
        // Arrange
        var model = new LoginDto
        {
            UserName = "testuser",
            Password = "wrongpassword"
        };

        var user = new User { UserName = model.UserName };

        _mockUserManager.Setup(x => x.FindByNameAsync(model.UserName))
            .ReturnsAsync(user);

        _mockUserManager.Setup(x => x.CheckPasswordAsync(user, model.Password))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Login(model);

        // Assert
        Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
        var unauthorizedResult = result as UnauthorizedObjectResult;
        Assert.That(unauthorizedResult.Value, Is.EqualTo("Incorrect password"));
    }

    [Test]
    public async Task Login_ReturnsOkWithToken_WhenLoginIsSuccessful()
    {
        // Arrange
        var model = new LoginDto
        {
            UserName = "testuser",
            Password = "password1"
        };

        var user = new User { UserName = model.UserName };

        _mockUserManager.Setup(x => x.FindByNameAsync(model.UserName))
            .ReturnsAsync(user);

        _mockUserManager.Setup(x => x.CheckPasswordAsync(user, model.Password))
            .ReturnsAsync(true);

        _mockJwtService.Setup(x => x.Generate(user)).Returns("sampleToken");

        // Act
        var result = await _controller.Login(model);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        var actualToken = GetTokenFromOkObjectResult(okResult);
        Assert.That(actualToken, Is.EqualTo("sampleToken"));
    }

    private string GetTokenFromOkObjectResult(OkObjectResult result)
    {
        var value = result.Value;
        PropertyInfo tokenProperty = value.GetType().GetProperty("Token");
        return tokenProperty.GetValue(value).ToString();
    }
}