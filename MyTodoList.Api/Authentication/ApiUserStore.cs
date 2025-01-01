using Microsoft.AspNetCore.Identity;

namespace MyTodoList.Api.Authentication
{
    public class ApiUserStore : IUserStore<User>
    {
        private readonly List<User> _users = new List<User>();

        public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            _users.Add(user);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            _users.Remove(user);
            return Task.FromResult(IdentityResult.Success);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            return Task.FromResult(user);
        }

        public Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = _users.FirstOrDefault(u => u.NormalizedUsername == normalizedUserName);
            return Task.FromResult(user);
        }

        public Task<string?> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUsername);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string?> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Username);
        }

        public Task SetNormalizedUserNameAsync(User user, string? normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUsername = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string? userName, CancellationToken cancellationToken)
        {
            user.Username = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser is null)
            {
                var error = new IdentityError
                {
                    Code = "UserNotFound",
                    Description = $"User with id {user.Id} not found"
                };
                return Task.FromResult(IdentityResult.Failed(error));
            }

            existingUser.Username = user.Username;
            existingUser.NormalizedUsername = user.NormalizedUsername;
            existingUser.PasswordHash = user.PasswordHash;
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
