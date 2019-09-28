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
    public class RoleStore : IRoleStore<ApplicationRole>
    {
        readonly IConnectionFactory connectionFactory;

        public RoleStore(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                role.Id = await connection.InsertAsync<ApplicationRole>(role);
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
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

        public async Task<ApplicationRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<ApplicationRole>("SELECT * FROM UserRoles WHERE Id = '"+ roleId + "'");
            }
        }

        public async Task<ApplicationRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<ApplicationRole>("SELECT * FROM UserRoles WHERE Name = '" + normalizedRoleName + "'");
            }
        }

        public Task<string> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(ApplicationRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.Name = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetRoleNameAsync(ApplicationRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                await connection.UpdateAsync<ApplicationRole>(role);
            }

            return IdentityResult.Success;
        }
    }
}
