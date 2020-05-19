using LiteDB.Identity.Models;
using LiteDB.Identity.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace LiteDB.Identity.Extensions
{
    /// <summary>
    /// Contains extension methods for <see cref="IServiceCollection"/> to add identity LiteDB stores.
    /// </summary>
    public static class IdentityLiteDbBuilderExtensions
    {
        /// <summary>
        /// Adds LiteDB identity default configuration to IServiceCollection.
        /// </summary>
        public static IdentityBuilder AddLiteDBIdentity(this IServiceCollection builder, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }
            builder.AddScoped<LiteDB.Identity.Database.ILiteDbIdentityContext, LiteDB.Identity.Database.LiteDbIdentityContext>(c => new LiteDB.Identity.Database.LiteDbIdentityContext(connectionString));

            // Identity stores
            builder.TryAddScoped<IUserStore<LiteDbUser>, UserStore<LiteDbUser, LiteDbRole, LiteDbUserRole, LiteDbUserClaim, LiteDbUserLogin, LiteDbUserToken>>();
            builder.TryAddScoped<IRoleStore<LiteDbRole>, RoleStore<LiteDbRole, LiteDbRoleClaim>>();

            var identityBuilder = builder.AddIdentity<LiteDbUser, LiteDbRole>();
            return identityBuilder;
        }
    }

}