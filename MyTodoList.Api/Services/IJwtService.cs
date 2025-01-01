using MyTodoList.Data;

namespace MyTodoList.Api.Services
{
    public interface IJwtService
    {
        string Generate(User user);
    }
}
