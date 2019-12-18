using Dapper;
using Dapper.Contrib.Extensions;
using Evopeons.API.Data;
using Evopeons.API.Entities;
using Evopeons.API.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Evopeons.API.Identity
{
    public class UserStore : IUserStore<ApplicationUser>, IUserEmailStore<ApplicationUser>,
    IUserTwoFactorStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IQueryableUserStore<ApplicationUser>
    {
        readonly IConnectionFactory connectionFactory;

        public IQueryable<ApplicationUser> Users => (connectionFactory.GetConnection() as IDbConnection).GetAll<ApplicationUser>().AsQueryable(); 

        public UserStore(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }



        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                await connection.InsertAsync<ApplicationUser>(user);
            }
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                await connection.DeleteAsync<ApplicationUser>(user);
            }
            return IdentityResult.Success;
        }


        public async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<ApplicationUser>("SELECT * FROM Users WHERE EmailNormalized ='" + normalizedEmail + "'");
            }
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<ApplicationUser>("SELECT * FROM Users WHERE Id ='"+userId+"'");
            }
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<ApplicationUser>("SELECT * FROM Users WHERE UsernameNormalized ='" + normalizedUserName + "'");
            }
        }

        public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailNormalized);
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UsernameNormalized);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Username);
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersAsync()
        {
            using (var connection = connectionFactory.GetConnection())
            {
                return await connection.GetAllAsync<ApplicationUser>();
            }
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.FromResult(0);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.EmailNormalized = normalizedEmail;
            return Task.FromResult(0);
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.UsernameNormalized = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }


        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            user.Username = userName;
            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                await connection.UpdateAsync(user);
            }

            return IdentityResult.Success;
        }
        public void Dispose()
        {

        }
    }
}
