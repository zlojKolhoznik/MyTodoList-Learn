using MyTodoList.Data.Models;

namespace MyTodoList.Api.Services
{
    public interface IJwtService
    {
        string Generate(User user);
    }
}
