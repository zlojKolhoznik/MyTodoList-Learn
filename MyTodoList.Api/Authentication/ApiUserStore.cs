using Microsoft.AspNetCore.Identity;
using MyTodoList.Data;

namespace MyTodoList.Api.Authentication
{
    public class ApiUserStore : IUserPasswordStore<User>
    {
        private readonly ApiDbContext _context;

        public ApiUserStore(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed resources
            }
            // Dispose unmanaged resources
        }

        public Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            return Task.FromResult(user);
        }

        public Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = _context.Users.FirstOrDefault(u => u.NormalizedUserName == normalizedUserName);
            return Task.FromResult(user);
        }

        public Task<string?> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string?> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(User user, string? normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string? userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                ValidateUserExists(user.Id);
                UpdateUser(user);
                return Task.FromResult(IdentityResult.Success);
            }
            catch (InvalidOperationException ex)
            {
                return Task.FromResult(CreateFailedResult("UserNotFound", ex.Message));
            }
        }

        public Task SetPasswordHashAsync(User user, string? passwordHash, CancellationToken cancellationToken)
        {
            try
            {
                ValidateUserExists(user.Id);
                user.PasswordHash = passwordHash;
                return Task.CompletedTask;
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Failed to set password hash: {ex.Message}");
            }
        }

        public Task<string?> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash is not null);
        }

        private void ValidateUserExists(string userId)
        {
            if (_context.Users.FirstOrDefault(u => u.Id == userId) is null)
            {
                throw new InvalidOperationException($"User with id {userId} not found");
            }
        }

        private void UpdateUser(User user)
        {
            var existingUser = _context.Users.First(u => u.Id == user.Id);
            existingUser.UserName = user.UserName;
            existingUser.NormalizedUserName = user.NormalizedUserName;
            existingUser.PasswordHash = user.PasswordHash;
        }

        private IdentityResult CreateFailedResult(string code, string description)
        {
            var error = new IdentityError
            {
                Code = code,
                Description = description
            };
            return IdentityResult.Failed(error);
        }
    }
}
