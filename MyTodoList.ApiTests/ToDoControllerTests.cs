using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyTodoList.Api.Controllers;
using MyTodoList.Data.Dto;
using MyTodoList.Data.Repositories;
using System.Security.Claims;

namespace MyTodoList.Api.Tests;

public class ToDoControllerTests
{
    private Mock<IToDoItemRepository> _repositoryMock = default!;
    private ToDoController _controller = default!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<IToDoItemRepository>();
        _controller = new ToDoController(_repositoryMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(
        [
                new Claim(ClaimTypes.NameIdentifier, "TestUserId")
            ]));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnItemsBelongingToUser()
    {
        // Arrange
        var items = new List<FullToDoItem>
            {
                new() { Id = 1, Title = "Task 1", UserId = "TestUserId" },
                new() { Id = 2, Title = "Task 2", UserId = "AnotherUserId" }
            };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<string>())).ReturnsAsync(items);

        // Act
        var result = await _controller.GetAllAsync();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var returnedItems = okResult!.Value as List<FullToDoItem>;
        Assert.That(returnedItems, Is.Not.Null);
        Assert.That(returnedItems!.Count, Is.EqualTo(1));
        Assert.That(returnedItems[0].UserId, Is.EqualTo("TestUserId"));
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnItemWhenAuthorized()
    {
        // Arrange
        var item = new FullToDoItem { Id = 1, Title = "Task 1", UserId = "TestUserId" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, null)).ReturnsAsync(item);

        // Act
        var result = await _controller.GetByIdAsync(1);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var returnedItem = okResult!.Value as FullToDoItem;
        Assert.That(returnedItem, Is.Not.Null);
        Assert.That(returnedItem!.Id, Is.EqualTo(item.Id));
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnUnauthorized_WhenUserIdDoesNotMatch()
    {
        // Arrange
        var item = new FullToDoItem { Id = 1, Title = "Task 1", UserId = "AnotherUserId" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, null)).ReturnsAsync(item);

        // Act
        var result = await _controller.GetByIdAsync(1);

        // Assert
        var unauthorizedResult = result as UnauthorizedObjectResult;
        Assert.That(unauthorizedResult, Is.Not.Null);
    }

    [Test]
    public async Task AddAsync_ShouldAddItemForAuthorizedUser()
    {
        // Arrange
        var briefToDoItem = new BriefToDoItem { Title = "Task 1", Description = "Description" };
        var addedItem = new FullToDoItem { Id = 1, Title = "Task 1", UserId = "TestUserId" };

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<BriefToDoItem>())).ReturnsAsync(addedItem);

        // Act
        var result = await _controller.AddAsync(briefToDoItem);

        // Assert
        var createdResult = result as CreatedAtActionResult;
        Assert.That(createdResult, Is.Not.Null);
        var returnedItem = createdResult!.Value as FullToDoItem;
        Assert.That(returnedItem, Is.Not.Null);
        Assert.That(returnedItem!.UserId, Is.EqualTo("TestUserId"));
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateItemWhenAuthorized()
    {
        // Arrange
        var fullToDoItem = new FullToDoItem { Id = 1, Title = "Updated Task", UserId = "TestUserId" };
        var existingItem = new FullToDoItem { Id = 1, Title = "Old Task", UserId = "TestUserId" };

        _repositoryMock.Setup(r => r.GetByIdAsync(1, null)).ReturnsAsync(existingItem);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<FullToDoItem>())).ReturnsAsync(fullToDoItem);

        // Act
        var result = await _controller.UpdateAsync(1, fullToDoItem);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var updatedItem = okResult!.Value as FullToDoItem;
        Assert.That(updatedItem, Is.Not.Null);
        Assert.That(updatedItem!.Title, Is.EqualTo(fullToDoItem.Title));
    }

    [Test]
    public async Task DeleteAsync_ShouldRemoveItemWhenAuthorized()
    {
        // Arrange
        var item = new FullToDoItem { Id = 1, Title = "Task to Delete", UserId = "TestUserId" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, null)).ReturnsAsync(item);
        _repositoryMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(item);

        // Act
        var result = await _controller.DeleteAsync(1);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var deletedItem = okResult!.Value as FullToDoItem;
        Assert.That(deletedItem, Is.Not.Null);
        Assert.That(deletedItem!.Id, Is.EqualTo(item.Id));
    }
}