using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTodoList.Data.Dto
{
    public class BriefToDoItem
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string UserId { get; set; } = null!;
    }
}
