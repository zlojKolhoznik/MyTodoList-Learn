using MyTodoList.Data.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTodoList.Data.Repositories
{
    public interface ITodoItemRepository
    {
        Task<FullToDoItem> GetByIdAsync(int id, string? includeProperties = null);
        Task<List<FullToDoItem>> GetAllAsync(string? includeProperties = null);
        Task<FullToDoItem> AddAsync(BriefToDoItem entity);
        Task<FullToDoItem> UpdateAsync(FullToDoItem entity);
        Task<FullToDoItem> DeleteAsync(int id);
    }
}
