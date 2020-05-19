using FluentAssertions;
using LiteDB.Identity.Models;
using LiteDB.Identity.Tests.Mocks;
using Microsoft.AspNetCore.Identity;
using System;
using Xunit;

namespace LiteDB.Identity.Tests.Stores
{
    public class RoleStoreTests
    {
        private readonly LiteDB.Identity.Tests.Mocks.IServicesBuilder services;

        public RoleStoreTests() {
            services = new ServicesBuilder();
            services.Build();
        }


        [Fact()]
        public void CreateAsyncTest()
        {
            var manager = services.GetRoleManager();
            var newRole = new LiteDbRole()
            {
                Name = "TestRole"
            };

            var result = manager.CreateAsync(newRole).GetAwaiter().GetResult();
            var role = manager.FindByNameAsync(newRole.NormalizedName).GetAwaiter().GetResult();

            result.Should().Be(IdentityResult.Success);
            role.Should().NotBeNull();
            role.Should().Match<LiteDbRole>(u => u.Name.Equals(role.Name));
        }


        [Fact()]
        public void DeleteAsyncTest()
        {
            var manager = services.GetRoleManager();
            LiteDbRole newRole = SetUpRole(manager);

            var result = manager.DeleteAsync(newRole).GetAwaiter().GetResult();
            var role = manager.FindByNameAsync(newRole.NormalizedName).GetAwaiter().GetResult();
                                                                   
            result.Should().Be(IdentityResult.Success);
            role.Should().BeNull();
        }

        [Fact()]
        public void FindByIdAsyncTest()
        {
            var manager = services.GetRoleManager();
            LiteDbRole newRole = SetUpRole(manager);

            var role = manager.FindByIdAsync(newRole.Id.ToString()).GetAwaiter().GetResult();

            role.Should().NotBeNull();
            role.Should().Match<LiteDbRole>(u => u.Id == newRole.Id);
        }

        [Fact()]
        public void FindByNameAsyncTest()
        {
            var manager = services.GetRoleManager();
            LiteDbRole newRole = SetUpRole(manager);

            var role = manager.FindByNameAsync(newRole.NormalizedName).GetAwaiter().GetResult();

            role.Should().NotBeNull();
            role.Should().Match<LiteDbRole>(u => u.NormalizedName == newRole.NormalizedName);

        }

        [Fact()]
        public void GetRoleIdAsync()
        {
            var manager = services.GetRoleManager();
            LiteDbRole newRole = SetUpRole(manager);

            var id = manager.GetRoleIdAsync(newRole).GetAwaiter().GetResult();

            id.Should().NotBeNull();
            id.Should().Match(newRole.Id.ToString());
        }

        [Fact()]
        public void GetRoleNameAsync()
        {
            var manager = services.GetRoleManager();
            LiteDbRole newRole = SetUpRole(manager);

            var name = manager.GetRoleNameAsync(newRole).GetAwaiter().GetResult();

            name.Should().NotBeNull();
            name.Should().Match(newRole.Name);
        }

        [Fact()]
        public void SetRoleNameAsync()
        {
            var manager = services.GetRoleManager();
            LiteDbRole newRole = SetUpRole(manager);
            var newName = "NewRoleTestName";

            var result = manager.SetRoleNameAsync(newRole, newName).GetAwaiter().GetResult();
            var role = manager.FindByNameAsync(newRole.NormalizedName).GetAwaiter().GetResult();

            result.Should().Be(IdentityResult.Success);
            role.Should().NotBeNull();
            role.Should().Match<LiteDbRole>(u => u.Name == newName);
        }

        [Fact()]
        public void UpdateAsync()
        {
            var manager = services.GetRoleManager();
            LiteDbRole newRole = SetUpRole(manager);
            newRole.Name = "NewRoleTestNameV2";

            var result = manager.UpdateAsync(newRole).GetAwaiter().GetResult();
            var role = manager.FindByNameAsync(newRole.NormalizedName).GetAwaiter().GetResult();

            result.Should().Be(IdentityResult.Success);
            role.Should().NotBeNull();
            role.Should().Match<LiteDbRole>(u => u.Name == "NewRoleTestNameV2");

        }

        [Fact]
        public void RoleStoreMethodsThrowWhenArgumentIsNull()
        {
            var manager = services.GetRoleManager();

            manager.Invoking(m => m.CreateAsync(null)).Should().Throw<ArgumentNullException>();
            manager.Invoking(m => m.DeleteAsync(null)).Should().Throw<ArgumentNullException>();
            manager.Invoking(m => m.FindByIdAsync(null)).Should().Throw<ArgumentNullException>();
            manager.Invoking(m => m.FindByNameAsync(null)).Should().Throw<ArgumentNullException>();
            manager.Invoking(m => m.GetRoleIdAsync(null)).Should().Throw<ArgumentNullException>();
            manager.Invoking(m => m.GetRoleNameAsync(null)).Should().Throw<ArgumentNullException>();
            manager.Invoking(m => m.SetRoleNameAsync(null, null)).Should().Throw<ArgumentNullException>();
            manager.Invoking(m => m.UpdateAsync(null)).Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RoleStoreMethodsThrowWhenDisposed()
        {
            var manager = services.GetRoleManager();
            manager.Dispose();

            manager.Invoking(m => m.CreateAsync(null)).Should().Throw<ObjectDisposedException>();
            manager.Invoking(m => m.DeleteAsync(null)).Should().Throw<ObjectDisposedException>();
            manager.Invoking(m => m.FindByIdAsync(null)).Should().Throw<ObjectDisposedException>();
            manager.Invoking(m => m.FindByNameAsync(null)).Should().Throw<ObjectDisposedException>();
            manager.Invoking(m => m.GetRoleIdAsync(null)).Should().Throw<ObjectDisposedException>();
            manager.Invoking(m => m.GetRoleNameAsync(null)).Should().Throw<ObjectDisposedException>();
            manager.Invoking(m => m.SetRoleNameAsync(null,null)).Should().Throw<ObjectDisposedException>();
            manager.Invoking(m => m.UpdateAsync(null)).Should().Throw<ObjectDisposedException>();
        }


        #region Private methods

        private LiteDbRole SetUpRole(RoleManager<LiteDbRole> manager)
        {
            var role = new LiteDbRole()
            {
                Name = "TestRole"
            };

            manager.CreateAsync(role).GetAwaiter().GetResult();

            return role;
        }

        #endregion
    }
}