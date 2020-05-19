## LiteDB.Identity
Implementation of AspNetCore.Identity for [LiteDB](https://github.com/mbdavid/LiteDB) database engine.

__LiteDB.Identity__ will allow quickly creates users login, registration, roles, claims and tokens functionalities for web application. 

## How to use it ?
Please install latest version of LiteDB.Identity using NuGet: 
```
Install-Package LiteDB.Identity
```

Next, in your Startup.cs file add reference to namespace:
	
```csharp
using LiteDB.Identity.Extensions;
```

Add default LiteDb.Identity implementation in ConfigureServices method:
```csharp
        public void ConfigureServices(IServiceCollection services)
        {

            string connectionString = Configuration.GetConnectionString("IdentityLiteDB");
            services.AddLiteDBIdentity(connectionString).AddDefaultTokenProviders().AddDefaultUI();

            services.AddControllersWithViews();
            services.AddRazorPages();
        }
```

__NOTE :__ appsettings.json should contains connection string to your LiteDB file.
For more implementation details please refer to sample [project]().

### Stores implementation

Following interfaces has been implemented on :
- UserStore :
```csharp
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
                                    IUserTwoFactorRecoveryCodeStore<TUser>
```
- RoleStore :
```csharp
    public class RoleStore<TRole, TRoleClaim> : IQueryableRoleStore<TRole>, 
                                                IRoleStore<TRole>, 
                                                IRoleClaimStore<TRole>
```

### Where to use it ?
- Great for small and medium size AspNetCore Websites,
- Quick implementation of Authentication and Authorization mechanism for WebAPIs.

### References
- LiteDB - [https://www.litedb.org/](https://www.litedb.org/)
- LiteDB Github - [https://github.com/mbdavid/LiteDB](https://github.com/mbdavid/LiteDB)
- AspNetCore Identity - [Introduction](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-3.1&tabs=visual-studio)
- AspNetCore Github - [https://github.com/dotnet/aspnetcore/tree/master/src/Identity](https://github.com/dotnet/aspnetcore/tree/master/src/Identity)

### Documentation and Wiki
in progress …

## License

[MIT](http://opensource.org/licenses/MIT)


