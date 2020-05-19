using LiteDB;
using LiteDB.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace LiteDB.Identity.Stores
{
    public class UserStore<TUser, TRole, TUserRole, TUserClaim, TUserLogin, TUserToken> : 
                                    IUserLoginStore<TUser>, 
                                    IUserStore<TUser>,
                                    IUserRoleStore<TUser>,
                                    IUserClaimStore<TUser>, 
                                    IUserPasswordStore<TUser>, 
                                    IUserSecurityStampStore<TUser>, 
                                    IUserEmailStore<TUser>, 
                                    IUserLockoutStore<TUser>, 
                                    IUserPhoneNumberStore<TUser>, 
                                    IQueryableUserStore<TUser>, 
                                    IUserTwoFactorStore<TUser>,
                                    IUserAuthenticationTokenStore<TUser>,
                                    IUserAuthenticatorKeyStore<TUser>,
                                    IUserTwoFactorRecoveryCodeStore<TUser>,
                                    IDisposable
        where TUser : LiteDbUser, new()
        where TRole : LiteDbRole, new()
        where TUserRole : LiteDbUserRole, new()
        where TUserClaim : LiteDbUserClaim, new()
        where TUserLogin : LiteDbUserLogin, new()
        where TUserToken : LiteDbUserToken, new()
    {
        private readonly ILiteCollection<TUser> users;
        private readonly ILiteCollection<TUserRole> userRoles;
        private readonly ILiteCollection<TUserClaim> userClaims;
        private readonly ILiteCollection<TRole> roles;
        private readonly ILiteCollection<TUserLogin> userLogins;
        private readonly ILiteCollection<TUserToken> userTokens;

        public UserStore(LiteDB.Identity.Database.ILiteDbIdentityContext dbContext)
        {
            users = dbContext.LiteDatabase.GetCollection<TUser>(nameof(LiteDbUser));
            userRoles = dbContext.LiteDatabase.GetCollection<TUserRole>(nameof(LiteDbUserRole));
            userClaims = dbContext.LiteDatabase.GetCollection<TUserClaim>(nameof(LiteDbUserClaim));
            roles = dbContext.LiteDatabase.GetCollection<TRole>(nameof(LiteDbRole));
            userLogins = dbContext.LiteDatabase.GetCollection<TUserLogin>(nameof(LiteDbUserLogin));
            userTokens = dbContext.LiteDatabase.GetCollection<TUserToken>(nameof(LiteDbUserToken));
        }
        public IQueryable<TUser> Users => users.FindAll().ToList().AsQueryable();

        public Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            foreach (var claim in claims)
            {
                var uc = new TUserClaim { UserId = user.Id, ClaimType = claim.Type, ClaimValue = claim.Value };
                userClaims.Insert(uc);
            }

            return Task.CompletedTask;
        }

        public Task AddToRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException(nameof(normalizedRoleName));
            }
            var role = roles.FindOne(r => r.NormalizedName == normalizedRoleName);
            
            if (role == null)
            {
                throw new InvalidOperationException($"Role not found {normalizedRoleName}");
            }

            userRoles.Insert(new TUserRole { RoleId = role.Id, UserId = user.Id });

            return Task.CompletedTask;
        }


        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await Task.Run(() => { users.Insert(user); }, cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await Task.Run(() => { users.Delete(user.Id); }, cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var user =  await Task.Run(() => {
                return users.FindOne(u => u.NormalizedEmail.Equals(normalizedEmail, StringComparison.InvariantCultureIgnoreCase));
            }, cancellationToken);

            return user;
        }

        public async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if(string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var user = await Task.Run(() => {
                return users.FindOne(u => u.Id == new ObjectId(userId));
            }, cancellationToken);

            return user;
        }

        public async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(normalizedUserName))
            {
                throw new ArgumentNullException(nameof(normalizedUserName));
            }

            var user = await Task.Run(() => {
                return users.FindOne(u => u.NormalizedUserName == normalizedUserName);
            }, cancellationToken);

            return user;
        }

        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.AccessFailedCount);
        }

        public async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var claims = await Task.Run(() => 
            {
                return userClaims.Find(uc => uc.UserId == user.Id).Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
            }, cancellationToken);

            return claims;
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.LockoutEnd);
        }

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var roleList = await Task.Run(() =>
            {
                var rolesIds = userRoles.Query().Where(u => u.UserId == user.Id).Select(r => r.RoleId).ToList();
                if (!rolesIds.Any()) return new List<string>();

                return roles.Query().Where(r => rolesIds.Contains(r.Id)).Select(r => r.Name).ToList();
            }, cancellationToken);

            return roleList;
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.SecurityStamp);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.Id == null ? null : user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.UserName);
        }

        public async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            var userList = await Task.Run(() => 
            { 
                var usersIds = userClaims.Query().Where(c => c.ClaimValue == claim.Value && c.ClaimType == claim.Type).Select(c => c.UserId).ToList();
                if (!usersIds.Any()) return new List<TUser>();

                return users.Query().Where(u => usersIds.Contains(u.Id)).ToList();
            }, cancellationToken);

            return userList;
        }

        public async Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (string.IsNullOrEmpty(normalizedRoleName))
            {
                throw new ArgumentNullException(nameof(normalizedRoleName));
            }

            var role = roles.FindOne(r=>r.NormalizedName == normalizedRoleName);

            if (role == null) return new List<TUser>();

            var userList = await Task.Run(() => {
                var usersIds = userRoles.Query().Where(r => r.RoleId == role.Id).Select(c => c.UserId).ToList();
                if (!usersIds.Any()) return new List<TUser>();

                return users.Query().Where(u => usersIds.Contains(u.Id)).ToList();

            }, cancellationToken);

            return userList;
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> IsInRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (string.IsNullOrEmpty(normalizedRoleName))
            {
                throw new ArgumentNullException(nameof(normalizedRoleName));
            }

            var role = roles.FindOne(r => r.NormalizedName == normalizedRoleName);
            if (role == null) return Task.FromResult(false);

            var exists = userRoles.FindOne(r => r.RoleId == role.Id && r.UserId == user.Id);

            return Task.FromResult(exists != null);
        }

        public Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }
            foreach (var claim in claims)
            {
                var matchedClaims = userClaims.Query().Where(u => u.UserId.Equals(user.Id) && u.ClaimValue == claim.Value && u.ClaimType == claim.Type).ToList();
                foreach (var c in matchedClaims)
                {
                    userClaims.Delete(c.Id);
                }
            }

            return Task.CompletedTask;
        }

        public Task RemoveFromRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException(nameof(normalizedRoleName));
            }
            var role = roles.FindOne(r => r.NormalizedName == normalizedRoleName);
            if (role == null) return Task.CompletedTask;

            var userRole = userRoles.FindOne(r => r.UserId == user.Id && r.RoleId == role.Id);
            if (userRole != null)
            {
                userRoles.Delete(userRole.Id);
            }
            
            return Task.CompletedTask;
        }

        public Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }
            if (newClaim == null)
            {
                throw new ArgumentNullException(nameof(newClaim));
            }

            var claims = userClaims.Query().Where(u => u.UserId.Equals(user.Id) && u.ClaimValue == claim.Value && u.ClaimType == claim.Type).ToList();
            foreach (var c in claims)
            {
                c.ClaimValue = newClaim.Value;
                c.ClaimType = newClaim.Type;
            }

            return Task.CompletedTask;
        }


        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.AccessFailedCount = 0;
            return Task.CompletedTask;
        }

        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.LockoutEnd = lockoutEnd;
            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (stamp == null)
            {
                throw new ArgumentNullException(nameof(stamp));
            }
            user.SecurityStamp = stamp;
            return Task.CompletedTask;
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            users.Update(user);

            return Task.FromResult(IdentityResult.Success);
        }


        public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            var userLogin = new TUserLogin() {
                UserId = user.Id,
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName,
                ProviderKey = login.ProviderKey
            };

            userLogins.Insert(userLogin);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrEmpty(loginProvider) || string.IsNullOrEmpty(providerKey))
            {
                throw new ArgumentNullException("Login provider argumment missing");
            }

            var userLogin = userLogins.FindOne(u => u.UserId == user.Id && u.LoginProvider == loginProvider && u.ProviderKey == providerKey);
            if(userLogin != null)
            {
                userLogins.Delete(userLogin.Id);
            }

            return Task.FromResult(IdentityResult.Success);
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            List<UserLoginInfo> result = new List<UserLoginInfo>();
            var logins = userLogins.Query().Where(u => u.UserId == user.Id).ToList();

            if (!logins.Any()) return await Task.FromResult(result);

            result = logins.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey, l.ProviderDisplayName)).ToList();

            return await Task.FromResult(result);
        }

        public async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (string.IsNullOrEmpty(loginProvider) || string.IsNullOrEmpty(providerKey))
            {
                throw new ArgumentNullException("Login provider argumment missing");
            }

            List<UserLoginInfo> result = new List<UserLoginInfo>();
            var login = userLogins.Query().Where(u => u.ProviderKey == providerKey && u.LoginProvider == loginProvider).FirstOrDefault();

            if (login != null) {
                return await Task.Run(()=> {
                    return users.FindOne(u => u.Id == login.UserId);
                }, cancellationToken);
            }
            
            return null;
        }

        public Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrEmpty(loginProvider) || string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Token provider argumment missing");
            }

            var token = userTokens.Query().Where(t => t.UserId == user.Id && t.LoginProvider == loginProvider && t.Name == name).FirstOrDefault();
            if(token== null) 
            {
                userTokens.Insert(new TUserToken
                {
                    UserId = user.Id,
                    LoginProvider= loginProvider,
                    Name = name,
                    Value = value
                });
            } else {
                token.Value = value;
                userTokens.Update(token);
            }
             return Task.FromResult(IdentityResult.Success);
        }

        public Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrEmpty(loginProvider) || string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Token provider argumment missing");
            }

            var token = userTokens.Query().Where(t => t.UserId == user.Id && t.LoginProvider == loginProvider && t.Name == name).FirstOrDefault();
            if (token != null)
            {
                userTokens.Delete(token.Id);
            }
            return Task.FromResult(IdentityResult.Success);
        }

        public async Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrEmpty(loginProvider) || string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Token provider argumment missing");
            }

            var token = await Task.Run(() => { 
                return userTokens.Query().Where(t => t.UserId == user.Id && t.LoginProvider == loginProvider && t.Name == name).FirstOrDefault();
            }, cancellationToken);

            return token?.Value;
        }

        private const string InternalLoginProvider = "[AspNetUserStore]";
        private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
        private const string RecoveryCodeTokenName = "RecoveryCodes";

        public Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken)
        {
            return SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);
        }

        public Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken)
        {
            return GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);
        }

        public Task ReplaceCodesAsync(TUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            var mergedCodes = string.Join(";", recoveryCodes);
            return SetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, mergedCodes, cancellationToken);
        }

        public async Task<bool> RedeemCodeAsync(TUser user, string code, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            var codes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? string.Empty;
            var splitCodes = codes.Split(';');
            if (splitCodes.Contains(code))
            {
                var updatedCodes = new List<string>(splitCodes.Where(s => s != code));
                await ReplaceCodesAsync(user, updatedCodes, cancellationToken);
                return true;
            }
            return false;
        }

        public async Task<int> CountCodesAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var codes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? string.Empty;
            return codes.Length > 0 ? codes.Split(';').Length : 0;
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

        ~UserStore()
        {
            Dispose(false);
        }

        #endregion
    }
}
