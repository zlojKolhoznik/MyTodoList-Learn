using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyTodoList.Data.Dto;
using MyTodoList.Data.Exceptions;
using MyTodoList.Data.Models;
using MyTodoList.Data.Repositories;

namespace MyTodoList.Data.Tests;

public class TodoItemRepositoryTests
{
    public class ToDoItemRepositoryTests
    {
        private ApiDbContext _context;
        private IMapper _mapper;
        private ToDoItemRepository _repository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase("ToDoItemTestDb")
                .Options;

            _context = new ApiDbContext(options);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ToDoItem, FullToDoItem>();
                cfg.CreateMap<FullToDoItem, ToDoItem>();
            });
            _mapper = mapperConfig.CreateMapper();

            _repository = new ToDoItemRepository(_context, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddAsync_ShouldAddToDoItemAndReturnMappedDto()
        {
            var briefToDoItem = new BriefToDoItem
            {
                Title = "Test Title",
                Description = "Test Description",
                UserId = "User123"
            };

            var result = await _repository.AddAsync(briefToDoItem);

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Title, Is.EqualTo(briefToDoItem.Title));
                Assert.That(result.Description, Is.EqualTo(briefToDoItem.Description));
                Assert.That(result.IsCompleted, Is.False);
            });

            var addedEntity = await _context.ToDoItems.FirstOrDefaultAsync();
            Assert.That(addedEntity, Is.Not.Null);
            Assert.That(addedEntity!.Title, Is.EqualTo(briefToDoItem.Title));
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveToDoItemAndReturnMappedDto()
        {
            var toDoItem = new ToDoItem { Title = "Test Title", Description = "Test Description", UserId = "User123" };
            await _context.ToDoItems.AddAsync(toDoItem);
            await _context.SaveChangesAsync();

            var result = await _repository.DeleteAsync(toDoItem.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo(toDoItem.Title));

            var deletedEntity = await _context.ToDoItems.FindAsync(toDoItem.Id);
            Assert.That(deletedEntity, Is.Null);
        }

        [Test]
        public void DeleteAsync_ShouldThrowException_WhenEntityDoesNotExist()
        {
            Assert.ThrowsAsync<ToDoItemNotFoundException>(async () =>
            {
                await _repository.DeleteAsync(999); // Non-existent ID
            });
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnMappedDtos()
        {
            var toDoItems = new List<ToDoItem>
            {
                new() { Title = "Title1", Description = "Description1", UserId = "User1" },
                new() { Title = "Title2", Description = "Description2", UserId = "User2" }
            };

            await _context.ToDoItems.AddRangeAsync(toDoItems);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(result[0].Title, Is.EqualTo("Title1"));
                Assert.That(result[1].Title, Is.EqualTo("Title2"));
            });
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnMappedDto()
        {
            var toDoItem = new ToDoItem { Title = "Test Title", Description = "Test Description", UserId = "User123" };
            await _context.ToDoItems.AddAsync(toDoItem);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(toDoItem.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo(toDoItem.Title));
        }

        [Test]
        public void GetByIdAsync_ShouldThrowException_WhenEntityDoesNotExist()
        {
            Assert.ThrowsAsync<ToDoItemNotFoundException>(async () =>
            {
                await _repository.GetByIdAsync(999); // Non-existent ID
            });
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateEntityAndReturnMappedDto()
        {
            var toDoItem = new ToDoItem { Title = "Old Title", Description = "Old Description", UserId = "User123" };
            await _context.ToDoItems.AddAsync(toDoItem);
            await _context.SaveChangesAsync();

            var updatedDto = new FullToDoItem
            {
                Id = toDoItem.Id,
                Title = "New Title",
                Description = "New Description",
                UserId = toDoItem.UserId,
                IsCompleted = true
            };

            var result = await _repository.UpdateAsync(updatedDto);

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Title, Is.EqualTo(updatedDto.Title));
                Assert.That(result.Description, Is.EqualTo(updatedDto.Description));
            });
            Assert.That(result.IsCompleted, Is.True);

            var updatedEntity = await _context.ToDoItems.FindAsync(toDoItem.Id);
            Assert.That(updatedEntity, Is.Not.Null);
            Assert.That(updatedEntity!.Title, Is.EqualTo(updatedDto.Title));
        }
    }
}
