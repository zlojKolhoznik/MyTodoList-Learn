using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTodoList.Data.Dto;
using MyTodoList.Data.Repositories;
using System.Security.Claims;

namespace MyTodoList.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ToDoController : ControllerBase
    {
        private readonly IToDoItemRepository _todoItemRepository;

        public ToDoController(IToDoItemRepository todoItemRepository)
        {
            _todoItemRepository = todoItemRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var items = await _todoItemRepository.GetAllAsync();
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
            items = items.Where(x => x.UserId == userId).ToList();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var item = await _todoItemRepository.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
            if (item.UserId != userId)
            {
                return Unauthorized($"Item with id {id} does not belong to the current user.");
            }
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] BriefToDoItem item)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
            item.UserId = userId;
            var newItem = await _todoItemRepository.AddAsync(item);
            return CreatedAtAction("GetById", new { id = newItem.Id }, newItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, FullToDoItem item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }
            var existingItem = await _todoItemRepository.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
            if (existingItem.UserId != userId)
            {
                return Unauthorized($"Item with id {id} does not belong to the current user.");
            }
            item.UserId = userId;
            item.Id = id;
            var updatedItem = await _todoItemRepository.UpdateAsync(item);
            return Ok(updatedItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var item = await _todoItemRepository.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
            if (item.UserId != userId)
            {
                return Unauthorized($"Item with id {id} does not belong to the current user.");
            }
            var deletedItem = await _todoItemRepository.DeleteAsync(id);
            return Ok(deletedItem);
        }
    }
}
