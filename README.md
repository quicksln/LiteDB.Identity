## LiteDB.Identity

🚀 Start a seamless authentication experience with __LiteDB.Identity__ 🚀

The revolutionary implementation of ASP.NET Core Identity tailored for the [LiteDB](https://github.com/mbdavid/LiteDB) database engine.
__LiteDB.Identity__ will provide quick creation of login, registration, roles, claims, and token functionality for web applications.

💡This isn't just a tool - it's your passport to an efficient authentication experience. 💡

__Latest versions supports:__ 
* LiteDB 5.0.17
* .NET 8 
* .NETSTANDARD 2.1
* Microsoft.Extensions.Identity.Core   8.0.0
* Microsoft.Extensions.Identity.Stores 8.0.0
### Support
If you have found my contributions to the projects helpful, consider __[buying me a coffee](https://www.buymeacoffee.com/quicksln)__ to fuel my efforts :)
<br/><a href="https://www.buymeacoffee.com/quicksln" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png" alt="Buy Me A Coffee" style="height: 41px !important;width: 174px !important;box-shadow: 0px 3px 2px 0px rgba(190, 190, 190, 0.5) !important;-webkit-box-shadow: 0px 3px 2px 0px rgba(190, 190, 190, 0.5) !important;" ></a>

## How to use it ?
Please install latest version of [LiteDB.Identity](https://www.nuget.org/packages/LiteDB.Identity/) using NuGet: 
```
Install-Package LiteDB.Identity
```
__For ASP.NET Core 8:__
```
Install-Package LiteDB.Identity -Version 1.0.8
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

Add default LiteDb.Identity implementation in Program.cs file:

__For ASP.NET Core 8 :__
```csharp
using Microsoft.AspNetCore.Identity;
using LiteDB.Identity.Extensions;
using LiteDB.Identity.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddLiteDBIdentity(connectionString);
builder.Services.AddDefaultIdentity<LiteDbUser>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
```

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

### References
- LiteDB - [https://www.litedb.org/](https://www.litedb.org/)
- LiteDB Identity Async - [https://github.com/devinSpitz/LiteDB.Identity.Async](https://github.com/devinSpitz/LiteDB.Identity.Async)
- LiteDB Github - [https://github.com/mbdavid/LiteDB](https://github.com/mbdavid/LiteDB)
- AspNetCore Identity - [Introduction](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-3.1&tabs=visual-studio)
- AspNetCore Identity Github - [https://github.com/dotnet/aspnetcore/tree/master/src/Identity](https://github.com/dotnet/aspnetcore/tree/master/src/Identity)

## License

[MIT](http://opensource.org/licenses/MIT)