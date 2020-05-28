using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using LiteDB.Identity.Database;
using LiteDB.Identity.Models;

using Microsoft.AspNetCore.Identity;

namespace LiteDB.Identity.Stores
{
    using LiteDB.Identity.Extensions;

    public class RoleStore<TRole, TRoleClaim> : IQueryableRoleStore<TRole>,
                                                IRoleStore<TRole>,
                                                IRoleClaimStore<TRole>,
                                                IDisposable
        where TRole : LiteDbRole, new()
        where TRoleClaim : LiteDbRoleClaim, new()
    {
        private readonly ILiteCollection<TRole> roles;
        private readonly ILiteCollection<TRoleClaim> roleClaim;

        public RoleStore(ILiteDbIdentityContext dbContext)
        {
            this.roles = dbContext.LiteDatabase.GetCollection<TRole>(typeof(TRole).Name);
            this.roleClaim = dbContext.LiteDatabase.GetCollection<TRoleClaim>(typeof(TRoleClaim).Name);
        }

        public IQueryable<TRole> Roles => roles.FindAll().AsQueryable();

        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            ThrowIfDisposedOrCancellationRequested(cancellationToken);

            role.ThrowArgumentNullExceptionIfNull(nameof(role));

            await Task.Run(() => { roles.Insert(role); }, cancellationToken);

            return IdentityResult.Success;
        }



        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            ThrowIfDisposedOrCancellationRequested(cancellationToken);

            role.ThrowArgumentNullExceptionIfNull(nameof(role));

            await Task.Run(() => { roles.Delete(role.Id); }, cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            ThrowIfDisposedOrCancellationRequested(cancellationToken);

            roleId.ThrowArgumentNullExceptionIfNull(nameof(roleId));

            var result = await Task.Run(() =>
            {
                return roles.FindById(new ObjectId(roleId));
            }, cancellationToken);

            return result;
        }

        public async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            ThrowIfDisposedOrCancellationRequested(cancellationToken);

            normalizedRoleName.ThrowArgumentNullExceptionIfNull(nameof(normalizedRoleName));

            var result = await Task.Run(() =>
            {
                return roles.FindOne(r => r.NormalizedName.Equals(normalizedRoleName, StringComparison.InvariantCultureIgnoreCase) ||
                    r.Name.Equals(normalizedRoleName, StringComparison.InvariantCultureIgnoreCase));
            }, cancellationToken);

            return result;
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            ThrowIfDisposedOrCancellationRequested(cancellationToken);

            role.ThrowArgumentNullExceptionIfNull(nameof(role));

            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            ThrowIfDisposedOrCancellationRequested(cancellationToken);

            role.ThrowArgumentNullExceptionIfNull(nameof(role));

            return Task.FromResult(role.Id == null ? null : role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            ThrowIfDisposedOrCancellationRequested(cancellationToken);

            role.ThrowArgumentNullExceptionIfNull(nameof(role));

            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            ThrowIfDisposedOrCancellationRequested(cancellationToken);

            role.ThrowArgumentNullExceptionIfNull(nameof(role));

            role.NormalizedName = normalizedName.ToUpper();

            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            ThrowIfDisposedOrCancellationRequested(cancellationToken);

            role.ThrowArgumentNullExceptionIfNull(nameof(role));

            role.Name = roleName;
            roles.Update(role);

            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            ThrowIfDisposedOrCancellationRequested(cancellationToken);

            role.ThrowArgumentNullExceptionIfNull(nameof(role));

            roles.Update(role);

            return Task.FromResult(IdentityResult.Success);
        }

        public async Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposedOrCancellationRequested(cancellationToken);

            role.ThrowArgumentNullExceptionIfNull(nameof(role));

            var roleClaims = await Task.Run(() =>
                {
                    return roleClaim.Find(c => c.RoleId == role.Id).Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
                }, cancellationToken);

            return roleClaims;
        }

        public Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            var newRoleClaim = new TRoleClaim { RoleId = role.Id, ClaimType = claim.Type, ClaimValue = claim.Value };
            roleClaim.Insert(newRoleClaim);

            return Task.CompletedTask;
        }

        public Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposedOrCancellationRequested(cancellationToken);

            claim.ThrowArgumentNullExceptionIfNull(nameof(claim));
            role.ThrowArgumentNullExceptionIfNull(nameof(role));

            var claimsToRemove = roleClaim.Query().Where(r => r.RoleId.Equals(role.Id) && r.ClaimValue == claim.Value && r.ClaimType == claim.Type).ToList();

            if (claimsToRemove.Any())
            {
                foreach (var rc in claimsToRemove)
                {
                    roleClaim.Delete(rc.Id);
                }
            }

            return Task.CompletedTask;
        }


        protected void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        #region IDisposable implementation 

        private bool disposed = false;

        // Public implementation of Dispose pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {

            }

            disposed = true;
        }

        ~RoleStore()
        {
            Dispose(false);
        }

        #endregion

        private void ThrowIfDisposedOrCancellationRequested(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
        }
    }
}
