using FluentAssertions;
using LiteDB.Identity.Models;
using LiteDB.Identity.Tests.Mocks;
using Microsoft.AspNetCore.Identity;
using System;
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
        public void CreateAsyncTest()
        {
            var manager = services.GetUserManager();
            LiteDbUser newUser = new LiteDbUser()
            {
                UserName = "Test",
                Email = "test@test.com",
            };

            var result = manager.CreateAsync(newUser).GetAwaiter().GetResult();
            var user = manager.FindByNameAsync(newUser.NormalizedUserName).GetAwaiter().GetResult();

            result.Should().Be(IdentityResult.Success);
            user.Should().NotBeNull();
            user.Should().Match<LiteDbUser>(u => u.UserName == newUser.UserName && u.Email == newUser.Email);
        }


        [Fact()]
        public void DeleteAsyncTest()
        {
            var manager = services.GetUserManager();
            LiteDbUser newUser = SetUpUser(manager);

            var result = manager.DeleteAsync(newUser).GetAwaiter().GetResult();
            var user = manager.FindByNameAsync(newUser.NormalizedUserName).GetAwaiter().GetResult();

            result.Should().Be(IdentityResult.Success);
            user.Should().BeNull();
        }

        [Fact()]
        public void FindByIdAsyncTest()
        {
            var manager = services.GetUserManager();
            LiteDbUser newUser = SetUpUser(manager);

            var user = manager.FindByIdAsync(newUser.Id.ToString()).GetAwaiter().GetResult();

            user.Should().NotBeNull();
            user.Should().Match<LiteDbUser>(u => u.Id == newUser.Id);
        }

        [Fact()]
        public void FindByNameAsyncTest()
        {
            var manager = services.GetUserManager();
            LiteDbUser newUser = SetUpUser(manager);

            var user = manager.FindByNameAsync(newUser.NormalizedUserName).GetAwaiter().GetResult();

            user.Should().NotBeNull();
            user.Should().Match<LiteDbUser>(u => u.NormalizedUserName == newUser.NormalizedUserName);
        }

        [Fact()]
        public void GetUserIdAsyncTest()
        {
            var manager = services.GetUserManager();
            LiteDbUser newUser = SetUpUser(manager);

            var id = manager.GetUserIdAsync(newUser).GetAwaiter().GetResult();

            id.Should().NotBeNull();
            id.Should().Match(newUser.Id.ToString());
        }

        [Fact()]
        public void GetUserNameAsyncTest()
        {
            var manager = services.GetUserManager();
            LiteDbUser newUser = SetUpUser(manager);

            var userName = manager.GetUserNameAsync(newUser).GetAwaiter().GetResult();

            userName.Should().NotBeNull();
            userName.Should().Match(newUser.UserName);
        }

        [Fact()]
        public void SetUserNameAsyncTest()
        {
            var manager = services.GetUserManager();
            LiteDbUser newUser = SetUpUser(manager);
            var newName = "NewTestName";

            var result = manager.SetUserNameAsync(newUser, newName).GetAwaiter().GetResult();
            var user = manager.FindByNameAsync(newUser.NormalizedUserName).GetAwaiter().GetResult();

            result.Should().Be(IdentityResult.Success);
            user.Should().NotBeNull();
            user.Should().Match<LiteDbUser>(u => u.UserName == newName);
        }

        [Fact()]
        public void UpdateAsyncTest()
        {
            var manager = services.GetUserManager();
            LiteDbUser newUser = SetUpUser(manager);
            newUser.UserName = "NewTestNameV2";
            newUser.Email = "NewTestName@test.com";

            var result = manager.UpdateAsync(newUser).GetAwaiter().GetResult();
            var user = manager.FindByNameAsync(newUser.NormalizedUserName).GetAwaiter().GetResult();

            result.Should().Be(IdentityResult.Success);
            user.Should().NotBeNull();
            user.Should().Match<LiteDbUser>(u => u.UserName == "NewTestNameV2");
            user.Should().Match<LiteDbUser>(u => u.Email== "NewTestName@test.com");
        }

        [Fact]
        public void UserStoreMethodsThrowWhenArgumentIsNull()
        {
            var manager = services.GetUserManager();

            manager.Invoking(m => m.CreateAsync(null)).Should().Throw<ArgumentNullException>();
            manager.Invoking(m => m.DeleteAsync(null)).Should().Throw<ArgumentNullException>();
            manager.Invoking(m => m.FindByIdAsync(null)).Should().Throw<ArgumentNullException>();
            manager.Invoking(m => m.FindByNameAsync(null)).Should().Throw<ArgumentNullException>();
            manager.Invoking(m => m.GetUserIdAsync(null)).Should().Throw<ArgumentNullException>();
            manager.Invoking(m => m.GetUserNameAsync(null)).Should().Throw<ArgumentNullException>();
            manager.Invoking(m => m.SetUserNameAsync(null, null)).Should().Throw<ArgumentNullException>();
            manager.Invoking(m => m.UpdateAsync(null)).Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UserStoreMethodsThrowWhenDisposed()
        {
            var manager = services.GetUserManager();
            manager.Dispose();

            manager.Invoking(m => m.CreateAsync(null)).Should().Throw<ObjectDisposedException>();
            manager.Invoking(m => m.DeleteAsync(null)).Should().Throw<ObjectDisposedException>();
            manager.Invoking(m => m.FindByIdAsync(null)).Should().Throw<ObjectDisposedException>();
            manager.Invoking(m => m.FindByNameAsync(null)).Should().Throw<ObjectDisposedException>();
            manager.Invoking(m => m.GetUserIdAsync(null)).Should().Throw<ObjectDisposedException>();
            manager.Invoking(m => m.GetUserNameAsync(null)).Should().Throw<ObjectDisposedException>();
            manager.Invoking(m => m.SetUserNameAsync(null,null)).Should().Throw<ObjectDisposedException>();
            manager.Invoking(m => m.UpdateAsync(null)).Should().Throw<ObjectDisposedException>();
        }


        #region Private methods

        private LiteDbUser SetUpUser(UserManager<LiteDbUser> manager)
        {
            var user = new LiteDbUser()
            {
                UserName = "Test",
                Email = "test@test.com",
            };

            manager.CreateAsync(user).GetAwaiter().GetResult();

            return user;
        }

        #endregion
    }
}