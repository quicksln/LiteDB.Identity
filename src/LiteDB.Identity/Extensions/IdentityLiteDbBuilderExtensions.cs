using LiteDB.Identity.Models;
using LiteDB.Identity.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using LiteDB.Identity.Database;
using LiteDB.Identity.Validators.Implementations;
using LiteDB.Identity.Validators.Interfaces;
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
        public static IdentityBuilder AddLiteDBIdentity(this IServiceCollection builder, Action<LiteDbIdentityOptions> configuration)
        {
            var options = new LiteDbIdentityOptions();
            configuration?.Invoke(options);

            if (string.IsNullOrEmpty(options.ConnectionString))
            {
                throw new ArgumentNullException(nameof(options.ConnectionString));
            }

            builder.AddScoped<ILiteDbIdentityContext, LiteDbIdentityContext>(c => new LiteDbIdentityContext(options.ConnectionString));
            builder.AddSingleton<IValidator, Validator>();

            return ConfigureStors(builder);
        }

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
            builder.AddSingleton<IValidator, Validator>();

            return ConfigureStors(builder);
        }

        private static IdentityBuilder ConfigureStors(IServiceCollection builder)
        {
            // Identity stores
            builder.TryAddScoped<IUserStore<LiteDbUser>, UserStore<LiteDbUser, LiteDbRole, LiteDbUserRole, LiteDbUserClaim, LiteDbUserLogin, LiteDbUserToken>>();
            builder.TryAddScoped<IRoleStore<LiteDbRole>, RoleStore<LiteDbRole, LiteDbRoleClaim>>();

            var identityBuilder = builder.AddIdentityCore<LiteDbUser>().AddRoles<LiteDbRole>();

            return identityBuilder;
        }
    }

}