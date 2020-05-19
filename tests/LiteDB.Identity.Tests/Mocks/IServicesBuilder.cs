using LiteDB.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace LiteDB.Identity.Tests.Mocks
{
    internal interface IServicesBuilder
    {
        void Build();
        UserManager<LiteDbUser> GetUserManager();
        RoleManager<LiteDbRole> GetRoleManager();
    }
}
