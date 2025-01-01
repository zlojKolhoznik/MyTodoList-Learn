﻿namespace MyTodoList.Api.Authentication
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Username { get; set; }
        public string? NormalizedUsername { get; set; }
        public string PasswordHash { get; set; } = null!;
    }
}
