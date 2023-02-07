## LiteDB.Identity

The implementation of ASP.NET Core Identity for the [LiteDB](https://github.com/mbdavid/LiteDB) database engine.

__LiteDB.Identity__ will provide quick creation of login, registration, roles, claims, and token functionality for web applications.

__Latest versions supports:__ 
* LiteDB 5.0.15
* .NET 6 and .NET 7
* .NETSTANDARD 2.1
* Microsoft.Extensions.Identity.Stores 6.0.13 and 7.0.2

## How to use it ?
Please install latest version of [LiteDB.Identity](https://www.nuget.org/packages/LiteDB.Identity/) using NuGet: 
```
Install-Package LiteDB.Identity
```
__For ASP.NET Core 7:__
```
Install-Package LiteDB.Identity -Version 1.0.7
```
__For ASP.NET Core 6:__
```
Install-Package LiteDB.Identity -Version 1.0.6
```
__For ASP.NET Core 3.1:__
```
Install-Package LiteDB.Identity -Version 1.0.3
```

Next, in your Startup.cs file add reference to namespace:
	
```csharp
using LiteDB.Identity.Extensions;
```

Add default LiteDb.Identity implementation in ConfigureServices method:

__For ASP.NET Core 6 and 7 :__
```csharp
using LiteDB.Identity.Extensions;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add Identity services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddLiteDBIdentity(connectionString).AddDefaultTokenProviders().AddDefaultUI();
//...
builder.Services.AddControllersWithViews();

var app = builder.Build();
//...
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
```

__For ASP.NET Core 3.1:__
```csharp
        public void ConfigureServices(IServiceCollection services)
        {

            string connectionString = Configuration.GetConnectionString("IdentityLiteDB");
            services.AddLiteDBIdentity(connectionString).AddDefaultTokenProviders().AddDefaultUI();

            services.AddControllersWithViews();
            services.AddRazorPages();
        }
```

__NOTE:__ appsettings.json should contains connection string to your LiteDB file.
For more implementation details please refer to sample [project](https://github.com/quicksln/LiteDB.Identity/tree/master/sample/LiteDB.Identity.Sample).

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
- Great for small and medium size website application based on:
    - ASP.NET Core MVC,
    - Blazor Server,
    - ASP.NET Core WebPages,
- Quick implementation of Authentication and Authorization mechanism for WebAPIs.

### Support

If you have found my contributions to the projects helpful, consider buying me a coffee to fuel my efforts :)
<a href="https://www.buymeacoffee.com/quicksln" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png" alt="Buy Me A Coffee" style="height: 41px !important;width: 174px !important;box-shadow: 0px 3px 2px 0px rgba(190, 190, 190, 0.5) !important;-webkit-box-shadow: 0px 3px 2px 0px rgba(190, 190, 190, 0.5) !important;" ></a>

### References
- LiteDB - [https://www.litedb.org/](https://www.litedb.org/)
- LiteDB Github - [https://github.com/mbdavid/LiteDB](https://github.com/mbdavid/LiteDB)
- AspNetCore Identity - [Introduction](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-3.1&tabs=visual-studio)
- AspNetCore Identity Github - [https://github.com/dotnet/aspnetcore/tree/master/src/Identity](https://github.com/dotnet/aspnetcore/tree/master/src/Identity)

## License

[MIT](http://opensource.org/licenses/MIT)


