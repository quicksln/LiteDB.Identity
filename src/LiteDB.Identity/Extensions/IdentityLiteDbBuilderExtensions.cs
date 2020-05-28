namespace LiteDB.Identity.Extensions
{
    using System;

    using LiteDB.Identity.Database;
    using LiteDB.Identity.Models;
    using LiteDB.Identity.Stores;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

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

            builder.AddScoped<ILiteDbIdentityContext, LiteDbIdentityContext>(c => new LiteDbIdentityContext(connectionString));

            // Identity stores
            builder.TryAddScoped<IUserStore<LiteDbUser>, UserStore<LiteDbUser, LiteDbRole, LiteDbUserRole, LiteDbUserClaim, LiteDbUserLogin, LiteDbUserToken>>();
            builder.TryAddScoped<IRoleStore<LiteDbRole>, RoleStore<LiteDbRole, LiteDbRoleClaim>>();

            var identityBuilder = builder.AddIdentity<LiteDbUser, LiteDbRole>();
            return identityBuilder;
        }
    }

}