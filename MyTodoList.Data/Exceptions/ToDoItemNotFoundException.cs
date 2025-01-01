using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTodoList.Data.Exceptions
{
    public class ToDoItemNotFoundException : Exception
    {
        public ToDoItemNotFoundException(string message) : base(message)
        {
            
        }

        public ToDoItemNotFoundException(string message, int id) : base($"{message} (Item id: {id})")
        {
            Id = id;
        }

        public int? Id { get; init; }
    }
}
