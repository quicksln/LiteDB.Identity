using LiteDB.Identity.Database;
using LiteDB.Identity.Extensions;
using LiteDB.Identity.Models;
using LiteDB.Identity.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace LiteDB.Identity.Tests.Mocks
{
    internal class ServicesBuilder : IServicesBuilder
    {
        private readonly IServiceCollection services;
        private ServiceProvider provider;
        public ServicesBuilder()
        {
            services = new ServiceCollection();
        }
        public void Build()
        {
            services.AddHttpContextAccessor();
            services.AddLogging();
            services.AddLiteDBIdentity("Filename=:memory:;");
            provider = services.BuildServiceProvider();
        }

        public RoleManager<LiteDbRole> GetRoleManager()
        {
            return provider.GetService<RoleManager<LiteDbRole>>();
        }

        public UserManager<LiteDbUser> GetUserManager()
        {
            return provider.GetService<UserManager<LiteDbUser>>();
        }
    }
}
