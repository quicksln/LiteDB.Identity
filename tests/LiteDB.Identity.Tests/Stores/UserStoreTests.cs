using FluentAssertions;
using LiteDB;
using LiteDB.Identity.Models;
using LiteDB.Identity.Tests.Mocks;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCore.Identity.LiteDB.Stores.Tests
{
    public class UserStoreTests
    {
        private readonly IServicesBuilder services;
        public UserStoreTests() {
            services = new ServicesBuilder();
            services.Build();
        }

        [Fact()]
        public async Task CreateAsyncTest()
        {
            var manager = services.GetUserManager();
            LiteDbUser newUser = new LiteDbUser()
            {
                UserName = "Test",
                Email = "test@test.com",
            };

            var result = await manager.CreateAsync(newUser);
            var user = await manager.FindByNameAsync(newUser.NormalizedUserName!);

            result.Should().Be(IdentityResult.Success);
            user.Should().NotBeNull();
            user.Should().Match<LiteDbUser>(u => u.UserName == newUser.UserName && u.Email == newUser.Email);
        }

        [Fact()]
        public async Task CreateUserAsyncAndReturnIdTest()
        {
            var manager = services.GetUserManager();
            LiteDbUser newUser = new LiteDbUser()
            {
                UserName = "Test",
                Email = "test@test.com",
            };

            var result = await manager.CreateAsync(newUser);
            var user = await manager.FindByNameAsync(newUser.NormalizedUserName!);

            newUser.Id.Should().NotBeNull();

            newUser.Id.Should().BeOfType<ObjectId>();
            newUser.Id.Should().NotBe(ObjectId.Empty);
            Assert.Equal(newUser.Id, user!.Id);
        }


        [Fact()]
        public async Task DeleteAsyncTest()
        {
            var manager = services.GetUserManager();
            LiteDbUser newUser = await SetUpUserAsync(manager);

            var result = await manager.DeleteAsync(newUser);
            var user = await manager.FindByNameAsync(newUser.NormalizedUserName!);

            result.Should().Be(IdentityResult.Success);
            user.Should().BeNull();
        }

        [Fact()]
        public async Task FindByIdAsyncTest()
        {
            var manager = services.GetUserManager();
            LiteDbUser newUser = await SetUpUserAsync(manager);

            var user = await manager.FindByIdAsync(newUser.Id.ToString());

            user.Should().NotBeNull();
            user.Should().Match<LiteDbUser>(u => u.Id == newUser.Id);
        }

        [Fact()]
        public async Task FindByNameAsyncTest()
        {
            var manager = services.GetUserManager();
            LiteDbUser newUser = await SetUpUserAsync(manager);

            var user = await manager.FindByNameAsync(newUser.NormalizedUserName!);

            user.Should().NotBeNull();
            user.Should().Match<LiteDbUser>(u => u.NormalizedUserName == newUser.NormalizedUserName);
        }

        [Fact()]
        public async Task GetUserIdAsyncTest()
        {
            var manager = services.GetUserManager();
            LiteDbUser newUser = await SetUpUserAsync(manager);

            var id = await manager.GetUserIdAsync(newUser);

            id.Should().NotBeNull();
            id.Should().Match(newUser.Id.ToString());
        }

        [Fact()]
        public async Task GetUserNameAsyncTest()
        {
            var manager = services.GetUserManager();
            LiteDbUser newUser = await SetUpUserAsync(manager);

            var userName = await manager.GetUserNameAsync(newUser);

            userName.Should().NotBeNull();
            userName.Should().Match(newUser.UserName);
        }

        [Fact()]
        public async Task SetUserNameAsyncTest()
        {
            var manager = services.GetUserManager();
            LiteDbUser newUser = await SetUpUserAsync(manager);
            var newName = "NewTestName";

            var result = await manager.SetUserNameAsync(newUser, newName);
            var user = await manager.FindByNameAsync(newUser.NormalizedUserName!);

            result.Should().Be(IdentityResult.Success);
            user.Should().NotBeNull();
            user.Should().Match<LiteDbUser>(u => u.UserName == newName);
        }

        [Fact()]
        public async Task UpdateAsyncTest()
        {
            var manager = services.GetUserManager();
            LiteDbUser newUser = await SetUpUserAsync(manager);
            newUser.UserName = "NewTestNameV2";
            newUser.Email = "NewTestName@test.com";

            var result = await manager.UpdateAsync(newUser);
            var user = await manager.FindByNameAsync(newUser.NormalizedUserName!);

            result.Should().Be(IdentityResult.Success);
            user.Should().NotBeNull();
            user.Should().Match<LiteDbUser>(u => u.UserName == "NewTestNameV2");
            user.Should().Match<LiteDbUser>(u => u.Email== "NewTestName@test.com");
        }

        [Fact]
        public async Task UserStoreMethodsThrowWhenArgumentIsNull()
        {
            var manager = services.GetUserManager();

            await manager.Invoking(m => m.CreateAsync(null!)).Should().ThrowAsync<ArgumentNullException>();
            await manager.Invoking(m => m.DeleteAsync(null!)).Should().ThrowAsync<ArgumentNullException>();
            await manager.Invoking(m => m.FindByIdAsync(null!)).Should().ThrowAsync<ArgumentNullException>();
            await manager.Invoking(m => m.FindByNameAsync(null!)).Should().ThrowAsync<ArgumentNullException>();
            await manager.Invoking(m => m.GetUserIdAsync(null!)).Should().ThrowAsync<ArgumentNullException>();
            await manager.Invoking(m => m.GetUserNameAsync(null!)).Should().ThrowAsync<ArgumentNullException>();
            await manager.Invoking(m => m.SetUserNameAsync(null!, null!)).Should().ThrowAsync<ArgumentNullException>();
            await manager.Invoking(m => m.UpdateAsync(null!)).Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UserStoreMethodsThrowWhenDisposed()
        {
            var manager = services.GetUserManager();
            manager.Dispose();

            await manager.Invoking(m => m.CreateAsync(null!)).Should().ThrowAsync<ObjectDisposedException>();
            await manager.Invoking(m => m.DeleteAsync(null!)).Should().ThrowAsync<ObjectDisposedException>();
            await manager.Invoking(m => m.FindByIdAsync(null!)).Should().ThrowAsync<ObjectDisposedException>();
            await manager.Invoking(m => m.FindByNameAsync(null!)).Should().ThrowAsync<ObjectDisposedException>();
            await manager.Invoking(m => m.GetUserIdAsync(null!)).Should().ThrowAsync<ObjectDisposedException>();
            await manager.Invoking(m => m.GetUserNameAsync(null!)).Should().ThrowAsync<ObjectDisposedException>();
            await manager.Invoking(m => m.SetUserNameAsync(null!,null!)).Should().ThrowAsync<ObjectDisposedException>();
            await manager.Invoking(m => m.UpdateAsync(null!)).Should().ThrowAsync<ObjectDisposedException>();
        }

        #region Private methods

        private async Task<LiteDbUser> SetUpUserAsync(UserManager<LiteDbUser> manager)
        {
            var user = new LiteDbUser()
            {
                UserName = "Test",
                Email = "test@test.com",
            };

            await  manager.CreateAsync(user);

            return user;
        }

        #endregion
    }


    public class LiteDbUserView : LiteDbUser
    {
        public string IdStr
        {
            get
            {
                return base.Id.ToString();
            }
            set
            {
                if (value != null)
                    base.Id = new ObjectId(value);

            }
        }
    }
}