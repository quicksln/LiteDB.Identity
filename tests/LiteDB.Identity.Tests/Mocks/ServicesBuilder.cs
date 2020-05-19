using LiteDB.Identity.Database;
using LiteDB.Identity.Models;
using LiteDB.Identity.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace LiteDB.Identity.Tests.Mocks
{
    internal class ServicesBuilder : IServicesBuilder
    {
        private readonly IServiceCollection services;
        public ServicesBuilder()
        {
            services = new ServiceCollection();
        }
        public void Build()
        {
            services.AddHttpContextAccessor();
            services.AddLogging();
            services.AddScoped<ILiteDbIdentityContext, LiteDbIdentityContextMock>();

            services.AddScoped<IUserStore<LiteDbUser>, UserStore<LiteDbUser, LiteDbRole, LiteDbUserRole, LiteDbUserClaim, LiteDbUserLogin, LiteDbUserToken>>();
            services.AddScoped<IRoleStore<LiteDbRole>, RoleStore<LiteDbRole, LiteDbRoleClaim>>();

            services.AddIdentity<LiteDbUser, LiteDbRole>();
        }

        public RoleManager<LiteDbRole> GetRoleManager()
        {
            return services.BuildServiceProvider().GetService<RoleManager<LiteDbRole>>();
        }

        public UserManager<LiteDbUser> GetUserManager()
        {
            return services.BuildServiceProvider().GetService<UserManager<LiteDbUser>>();
        }
    }
}
