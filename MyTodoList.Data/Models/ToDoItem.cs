using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTodoList.Data.Models
{
    public class ToDoItem
    {
        public int Id { get; set; }
        
        public string Title { get; set; } = null!;
        
        public string? Description { get; set; }
        
        public bool IsCompleted { get; set; }

        public string UserId { get; set; } = null!;

        public User User { get; set; } = null!;
    }
}
