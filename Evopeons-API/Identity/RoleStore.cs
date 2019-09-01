using Dapper;
using Dapper.Contrib.Extensions;
using Evopeons.API.Data;
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
    public class RoleStore : IRoleStore<UserRole>
    {
        readonly IConnectionFactory connectionFactory;

        public RoleStore(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<IdentityResult> CreateAsync(UserRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                role.Id = await connection.InsertAsync<UserRole>(role);
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(UserRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                await connection.DeleteAsync(role);
            }

            return IdentityResult.Success;
        }

        public void Dispose()
        {
        }

        public async Task<UserRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<UserRole>("SELECT * FROM UserRoles WHERE Id = '"+ roleId + "'");
            }
        }

        public async Task<UserRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<UserRole>("SELECT * FROM UserRoles WHERE Name = '" + normalizedRoleName + "'");
            }
        }

        public Task<string> GetNormalizedRoleNameAsync(UserRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task<string> GetRoleIdAsync(UserRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(UserRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(UserRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.Name = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetRoleNameAsync(UserRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(UserRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                await connection.UpdateAsync<UserRole>(role);
            }

            return IdentityResult.Success;
        }
    }
}
