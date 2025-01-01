namespace MyTodoList.Data
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? UserName { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? PasswordHash { get; set; }
    }
}
