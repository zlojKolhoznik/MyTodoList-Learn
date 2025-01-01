namespace MyTodoList.Api.Authentication.Models
{
    public class RegisterDto
    {
        public string UserName { get; set; } = null!;
        
        public string Password { get; set; } = null!;

        public string ConfirmPassword { get; set; } = null!;
    }
}
