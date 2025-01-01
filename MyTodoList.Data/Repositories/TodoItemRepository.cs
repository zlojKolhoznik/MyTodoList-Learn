using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyTodoList.Data.Dto;
using MyTodoList.Data.Exceptions;
using MyTodoList.Data.Models;

namespace MyTodoList.Data.Repositories
{
    public class ToDoItemRepository : ITodoItemRepository
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;

        public ToDoItemRepository(ApiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<FullToDoItem> AddAsync(BriefToDoItem entity)
        {
            var toDoItem = new ToDoItem
            {
                Title = entity.Title,
                Description = entity.Description,
                UserId = entity.UserId,
                IsCompleted = false
            };
            await _context.ToDoItems.AddAsync(toDoItem);
            await _context.SaveChangesAsync();
            return _mapper.Map<FullToDoItem>(toDoItem);
        }

        public async Task<FullToDoItem> DeleteAsync(int id)
        {
            EnsureEntityExists(id);
            var entity = (await _context.ToDoItems.FindAsync(id))!;
            _context.ToDoItems.Remove(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<FullToDoItem>(entity);
        }

        public async Task<List<FullToDoItem>> GetAllAsync(string? includeProperties = null)
        {
            var query = _context.ToDoItems.AsQueryable();
            IncludeProperties(query, includeProperties);
            return await query.Select(x => _mapper.Map<FullToDoItem>(x)).ToListAsync();
        }

        public async Task<FullToDoItem> GetByIdAsync(int id, string? includeProperties = null)
        {
            var query = _context.ToDoItems.AsQueryable();
            IncludeProperties(query, includeProperties);
            EnsureEntityExists(id);
            return _mapper.Map<FullToDoItem>(await query.FirstOrDefaultAsync(x => x.Id == id));
        }

        public async Task<FullToDoItem> UpdateAsync(FullToDoItem entity)
        {
            EnsureEntityExists(entity.Id);
            var toDoItem = await _context.ToDoItems.FindAsync(entity.Id);
            toDoItem!.Title = entity.Title;
            toDoItem.Description = entity.Description;
            toDoItem.IsCompleted = entity.IsCompleted;
            _context.ToDoItems.Update(toDoItem);
            await _context.SaveChangesAsync();
            return _mapper.Map<FullToDoItem>(toDoItem);
        }

        private static void IncludeProperties(IQueryable<ToDoItem> query, string? includeProperties)
        {
            includeProperties?.Split(',').ToList().ForEach(x => query.Include(x));
        }

        private void EnsureEntityExists(int id)
        {
            if (!_context.ToDoItems.Any(x => x.Id == id))
            {
                throw new ToDoItemNotFoundException("Entity not found.");
            }
        }
    }
}
