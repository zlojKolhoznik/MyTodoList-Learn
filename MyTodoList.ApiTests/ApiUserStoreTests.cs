using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyTodoList.Api.Authentication;
using MyTodoList.Data.Models;
using MyTodoList.Data;

namespace MyTodoList.Api.Tests;

public class ApiUserStoreTests
{
    private ApiDbContext _context = default!;
    private ApiUserStore _userStore = default!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApiDbContext>()
            .UseInMemoryDatabase(databaseName: "MyTodoList")
            .Options;
        _context = new ApiDbContext(options);
        _userStore = new ApiUserStore(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _userStore.Dispose();
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task CreateAsync_AddsUserToDatabaseIfNotExists()
    {
        var user = new User
        {
            Id = "1",
            UserName = "testuser",
            NormalizedUserName = "TESTUSER"
        };
        await _userStore.CreateAsync(user, CancellationToken.None);
        var result = await _context.Users.FirstOrDefaultAsync(u => u.Id == "1");
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(user));
    }

    [Test]
    public async Task CreateAsync_FailsIfUserExists()
    {
        var user = new User
        {
            Id = "1",
            UserName = "testuser",
            NormalizedUserName = "TESTUSER"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        var result = await _userStore.CreateAsync(user, CancellationToken.None);
        Assert.Multiple(() =>
        {
            Assert.That(result.Succeeded, Is.EqualTo(false));
            Assert.That(result.Errors.Count(), Is.EqualTo(1));
            Assert.That(result.Errors.First().Code, Is.EqualTo("UserExists"));
            Assert.That(result.Errors.First().Description, Is.EqualTo($"User with id {user.Id} already exists."));
        });

    }

    [Test]
    public async Task FindByIdAsync_ReturnsUserIfExists()
    {
        var user = new User
        {
            Id = "1",
            UserName = "testuser",
            NormalizedUserName = "TESTUSER"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        var result = await _userStore.FindByIdAsync("1", CancellationToken.None);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(user));
    }

    [Test]
    public async Task FindByIdAsync_ReturnsNullIfNotExists()
    {
        var result = await _userStore.FindByIdAsync("3", CancellationToken.None);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_RemovesUserIfExists()
    {
        var user = new User
        {
            Id = "1",
            UserName = "testuser",
            NormalizedUserName = "TESTUSER"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        var result = await _userStore.DeleteAsync(user, CancellationToken.None);
        var userFromDb = await _context.Users.FirstOrDefaultAsync(u => u.Id == "1");
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(IdentityResult.Success));
            Assert.That(userFromDb, Is.Null);
        });
    }

    [Test]
    public async Task DeleteAsync_FailsIfUserNotExists()
    {
        var user = new User
        {
            Id = "1",
            UserName = "testuser",
            NormalizedUserName = "TESTUSER"
        };
        var result = await _userStore.DeleteAsync(user, CancellationToken.None);
        Assert.Multiple(() =>
        {
            Assert.That(result.Succeeded, Is.EqualTo(false));
            Assert.That(result.Errors.Count(), Is.EqualTo(1));
            Assert.That(result.Errors.First().Code, Is.EqualTo("UserNotFound"));
            Assert.That(result.Errors.First().Description, Is.EqualTo($"User with id {user.Id} does not exist."));
        });
    }

    [Test]
    public async Task UpdateAsync_UpdatesUserIfExists()
    {
        var user = new User
        {
            Id = "1",
            UserName = "testuser",
            NormalizedUserName = "TESTUSER"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        user.UserName = "newuser";
        user.NormalizedUserName = "NEWUSER";
        var result = await _userStore.UpdateAsync(user, CancellationToken.None);
        var userFromDb = await _context.Users.FirstOrDefaultAsync(u => u.Id == "1");
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(IdentityResult.Success));
            Assert.That(userFromDb, Is.EqualTo(user));
        });
    }

    [Test]
    public async Task UpdateAsync_FailsIfUserNotExists()
    {
        var user = new User
        {
            Id = "1",
            UserName = "testuser",
            NormalizedUserName = "TESTUSER"
        };
        var result = await _userStore.UpdateAsync(user, CancellationToken.None);
        Assert.Multiple(() =>
        {
            Assert.That(result.Succeeded, Is.EqualTo(false));
            Assert.That(result.Errors.Count(), Is.EqualTo(1));
            Assert.That(result.Errors.First().Code, Is.EqualTo("UserNotFound"));
            Assert.That(result.Errors.First().Description, Is.EqualTo($"User with id {user.Id} does not exist."));
        });
    }

    [Test]
    public async Task SetPasswordHashAsync_SetsPasswordHash()
    {
        var user = new User
        {
            Id = "1",
            UserName = "testuser",
            NormalizedUserName = "TESTUSER"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        var passwordHash = "passwordhash";
        await _userStore.SetPasswordHashAsync(user, passwordHash, CancellationToken.None);
        var userFromDb = await _context.Users.FirstOrDefaultAsync(u => u.Id == "1");
        Assert.That(userFromDb.PasswordHash, Is.EqualTo(passwordHash));
    }

    [Test]
    public async Task FindByNameAsync_ReturnsUserIfExists()
    {
        var user = new User
        {
            Id = "1",
            UserName = "testuser",
            NormalizedUserName = "TESTUSER"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        var result = await _userStore.FindByNameAsync("TESTUSER", CancellationToken.None);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(user));
    }

    [Test]
    public async Task FindByNameAsync_ReturnsNullIfNotExists()
    {
        var result = await _userStore.FindByNameAsync("TESTUSER", CancellationToken.None);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetNormalizedUserNameAsync_ReturnsNormalizedUserName()
    {
        var user = new User
        {
            Id = "1",
            UserName = "testuser",
            NormalizedUserName = "TESTUSER"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        var result = await _userStore.GetNormalizedUserNameAsync(user, CancellationToken.None);
        Assert.That(result, Is.EqualTo(user.NormalizedUserName));
    }

    [Test]
    public async Task GetUserIdAsync_ReturnsUserId()
    {
        var user = new User
        {
            Id = "1",
            UserName = "testuser",
            NormalizedUserName = "TESTUSER"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        var result = await _userStore.GetUserIdAsync(user, CancellationToken.None);
        Assert.That(result, Is.EqualTo(user.Id));
    }

    [Test]
    public async Task GetUserNameAsync_ReturnsUserName()
    {
        var user = new User
        {
            Id = "1",
            UserName = "testuser",
            NormalizedUserName = "TESTUSER"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        var result = await _userStore.GetUserNameAsync(user, CancellationToken.None);
        Assert.That(result, Is.EqualTo(user.UserName));
    }

    [Test]
    public async Task SetNormalizedUserNameAsync_SetsNormalizedUserName()
    {
        var user = new User
        {
            Id = "1",
            UserName = "testuser",
            NormalizedUserName = "TESTUSER"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        var normalizedName = "NEWUSER";
        await _userStore.SetNormalizedUserNameAsync(user, normalizedName, CancellationToken.None);
        var userFromDb = await _context.Users.FirstOrDefaultAsync(u => u.Id == "1");
        Assert.That(userFromDb.NormalizedUserName, Is.EqualTo(normalizedName));
    }

    [Test]
    public async Task SetUserNameAsync_SetsUserName()
    {
        var user = new User
        {
            Id = "1",
            UserName = "testuser",
            NormalizedUserName = "TESTUSER"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        var userName = "newuser";
        await _userStore.SetUserNameAsync(user, userName, CancellationToken.None);
        var userFromDb = await _context.Users.FirstOrDefaultAsync(u => u.Id == "1");
        Assert.That(userFromDb.UserName, Is.EqualTo(userName));
    }

    [Test]
    public async Task GetPasswordHashAsync_ReturnsPasswordHash()
    {
        var user = new User
        {
            Id = "1",
            UserName = "testuser",
            NormalizedUserName = "TESTUSER",
            PasswordHash = "passwordhash"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        var result = await _userStore.GetPasswordHashAsync(user, CancellationToken.None);
        Assert.That(result, Is.EqualTo(user.PasswordHash));
    }

    [Test]
    public async Task HasPasswordAsync_ReturnsTrueIfPasswordHashExists()
    {
        var user = new User
        {
            Id = "1",
            UserName = "testuser",
            NormalizedUserName = "TESTUSER",
            PasswordHash = "passwordhash"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        var result = await _userStore.HasPasswordAsync(user, CancellationToken.None);
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task HasPasswordAsync_ReturnsFalseIfPasswordHashNotExists()
    {
        var user = new User
        {
            Id = "1",
            UserName = "testuser",
            NormalizedUserName = "TESTUSER"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        var result = await _userStore.HasPasswordAsync(user, CancellationToken.None);
        Assert.That(result, Is.False);
    }
}