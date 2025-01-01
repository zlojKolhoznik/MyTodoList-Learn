using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyTodoList.Data;
using MyTodoList.Data.Models;

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
            try
            {
                await ValidateUserDoesNotExist(user.Id);
                await _context.Users.AddAsync(user, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return CreateFailedResult("UserExists", ex.Message);
            }
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                await ValidateUserExists(user.Id);
                _context.Users.Remove(user);
                await _context.SaveChangesAsync(cancellationToken);
                return IdentityResult.Success;
            }
            catch (InvalidOperationException ex)
            {
                return CreateFailedResult("UserNotFound", ex.Message);
            }
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

        public async Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            return user;
        }

        public async Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken);
            return user;
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

        public async Task SetNormalizedUserNameAsync(User user, string? normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task SetUserNameAsync(User user, string? userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                await ValidateUserExists(user.Id);
                await UpdateUser(user);
                return IdentityResult.Success;
            }
            catch (InvalidOperationException ex)
            {
                return CreateFailedResult("UserNotFound", ex.Message);
            }
        }

        public async Task SetPasswordHashAsync(User user, string? passwordHash, CancellationToken cancellationToken)
        {
            try
            {
                user.PasswordHash = passwordHash;
                await _context.SaveChangesAsync(cancellationToken);
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
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        private async Task ValidateUserExists(string userId)
        {
            if (await _context.Users.FirstOrDefaultAsync(u => u.Id == userId) is null)
            {
                throw new InvalidOperationException($"User with id {userId} does not exist.");
            }
        }

        private async Task ValidateUserDoesNotExist(string userId)
        {
            if (await _context.Users.AnyAsync(u => u.Id == userId))
            {
                throw new InvalidOperationException($"User with id {userId} already exists.");
            }
        }

        private async Task UpdateUser(User user)
        {
            var existingUser = await _context.Users.FirstAsync(u => u.Id == user.Id);
            existingUser.UserName = user.UserName;
            existingUser.NormalizedUserName = user.NormalizedUserName;
            existingUser.PasswordHash = user.PasswordHash;
            await _context.SaveChangesAsync();
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
