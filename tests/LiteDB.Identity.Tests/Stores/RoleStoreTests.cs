using FluentAssertions;
using LiteDB.Identity.Models;
using LiteDB.Identity.Tests.Mocks;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
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
        public async Task CreateAsyncTest()
        {
            var manager = services.GetRoleManager();
            var newRole = new LiteDbRole()
            {
                Name = "TestRole"
            };

            var result = await manager.CreateAsync(newRole);
            var role = await manager.FindByNameAsync(newRole.NormalizedName!);

            result.Should().Be(IdentityResult.Success);
            role.Should().NotBeNull();
            role.Should().Match<LiteDbRole>(u => u.Name!.Equals(role!.Name));
        }

        [Fact()]
        public async Task CreateRoleAsyncAndReturnIdTest()
        {
            var manager = services.GetRoleManager();
            var newRole = new LiteDbRole()
            {
                Name = "TestRole"
            };

            var result = await manager.CreateAsync(newRole);
            var role = await manager.FindByNameAsync(newRole.NormalizedName!);

            newRole.Id.Should().NotBeNull();

            newRole.Id.Should().BeOfType<ObjectId>();
            newRole.Id.Should().NotBe(ObjectId.Empty);
            Assert.Equal(newRole.Id, role!.Id);
        }


        [Fact()]
        public async Task DeleteAsyncTest()
        {
            var manager = services.GetRoleManager();
            LiteDbRole newRole = await SetUpRoleAsync(manager);

            var result = await manager.DeleteAsync(newRole);
            var role = await manager.FindByNameAsync(newRole.NormalizedName!);
                                                                   
            result.Should().Be(IdentityResult.Success);
            role.Should().BeNull();
        }

        [Fact()]
        public async Task FindByIdAsyncTest()
        {
            var manager = services.GetRoleManager();
            LiteDbRole newRole = await SetUpRoleAsync(manager);

            var role = await manager.FindByIdAsync(newRole.Id.ToString());

            role.Should().NotBeNull();
            role.Should().Match<LiteDbRole>(u => u.Id == newRole.Id);
        }

        [Fact()]
        public async Task FindByNameAsyncTest()
        {
            var manager = services.GetRoleManager();
            LiteDbRole newRole = await SetUpRoleAsync(manager);

            var role = await manager.FindByNameAsync(newRole.NormalizedName!);

            role.Should().NotBeNull();
            role.Should().Match<LiteDbRole>(u => u.NormalizedName == newRole.NormalizedName);

        }

        [Fact()]
        public async Task GetRoleIdAsyncTest()
        {
            var manager = services.GetRoleManager();
            LiteDbRole newRole = await SetUpRoleAsync(manager);

            var id = await manager.GetRoleIdAsync(newRole);

            id.Should().NotBeNull();
            id.Should().Match(newRole.Id.ToString());
        }

        [Fact()]
        public async Task GetRoleNameAsyncTest()
        {
            var manager = services.GetRoleManager();
            LiteDbRole newRole = await SetUpRoleAsync(manager);

            var name = await manager.GetRoleNameAsync(newRole);

            name.Should().NotBeNull();
            name.Should().Match(newRole.Name);
        }

        [Fact()]
        public async Task SetRoleNameAsyncTest()
        {
            var manager = services.GetRoleManager();
            LiteDbRole newRole = await SetUpRoleAsync(manager);
            var newName = "NewRoleTestName";

            var result = await manager.SetRoleNameAsync(newRole, newName);
            var role = await manager.FindByNameAsync(newRole.NormalizedName!);

            result.Should().Be(IdentityResult.Success);
            role.Should().NotBeNull();
            role.Should().Match<LiteDbRole>(u => u.Name == newName);
        }

        [Fact()]
        public async Task UpdateAsyncTest()
        {
            var manager = services.GetRoleManager();
            LiteDbRole newRole = await SetUpRoleAsync(manager);
            newRole.Name = "NewRoleTestNameV2";

            var result = await manager.UpdateAsync(newRole);
            var role = await manager.FindByNameAsync(newRole.NormalizedName!);

            result.Should().Be(IdentityResult.Success);
            role.Should().NotBeNull();
            role.Should().Match<LiteDbRole>(u => u.Name == "NewRoleTestNameV2");

        }

        [Fact()]
        public async Task AddClaimAsyncTest()
        {
            var manager = services.GetRoleManager();
            LiteDbRole role = await SetUpRoleAsync(manager);
            var claim = new Claim("test", "test");

            var result = await manager.AddClaimAsync(role, claim);
            var claimsForRole = await manager.GetClaimsAsync(role);

            result.Should().Be(IdentityResult.Success);
            claimsForRole.Should().NotBeNull();
            claimsForRole.Count.Should().Be(1);
        }

        [Fact()]
        public async Task RemoveClaimAsyncTest()
        {
            var manager = services.GetRoleManager();
            LiteDbRole role = await SetUpRoleAsync(manager);
            var claim = new Claim("test", "test");
            await manager.AddClaimAsync(role, claim);

            var result = await manager.RemoveClaimAsync(role, claim);
            var claimsForRole = await manager.GetClaimsAsync(role);

            result.Should().Be(IdentityResult.Success);
            claimsForRole.Should().NotBeNull();
            claimsForRole.Count.Should().Be(0);
        }

        [Fact]
        public async Task RoleStoreMethodsThrowWhenArgumentIsNull()
        {
            var manager = services.GetRoleManager();

            await manager.Invoking(m => m.CreateAsync(null!)).Should().ThrowAsync<ArgumentNullException>();
            await manager.Invoking(m => m.DeleteAsync(null!)).Should().ThrowAsync<ArgumentNullException>();
            await manager.Invoking(m => m.FindByIdAsync(null!)).Should().ThrowAsync<ArgumentNullException>();
            await manager.Invoking(m => m.FindByNameAsync(null!)).Should().ThrowAsync<ArgumentNullException>();
            await manager.Invoking(m => m.GetRoleIdAsync(null!)).Should().ThrowAsync<ArgumentNullException>();
            await manager.Invoking(m => m.GetRoleNameAsync(null!)).Should().ThrowAsync<ArgumentNullException>();
            await manager.Invoking(m => m.SetRoleNameAsync(null!, null)).Should().ThrowAsync<ArgumentNullException>();
            await manager.Invoking(m => m.UpdateAsync(null!)).Should().ThrowAsync<ArgumentNullException>();
            await manager.Invoking(m => m.AddClaimAsync(null!,null!)).Should().ThrowAsync<ArgumentNullException>();
            await manager.Invoking(m => m.GetClaimsAsync(null!)).Should().ThrowAsync<ArgumentNullException>();
            await manager.Invoking(m => m.RemoveClaimAsync(null!, null!)).Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task RoleStoreMethodsThrowWhenDisposed()
        {
            var manager = services.GetRoleManager();
            manager.Dispose();

            await manager.Invoking(m => m.CreateAsync(null!)).Should().ThrowAsync<ObjectDisposedException>();
            await manager.Invoking(m => m.DeleteAsync(null!)).Should().ThrowAsync<ObjectDisposedException>();
            await manager.Invoking(m => m.FindByIdAsync(null!)).Should().ThrowAsync<ObjectDisposedException>();
            await manager.Invoking(m => m.FindByNameAsync(null!)).Should().ThrowAsync<ObjectDisposedException>();
            await manager.Invoking(m => m.GetRoleIdAsync(null!)).Should().ThrowAsync<ObjectDisposedException>();
            await manager.Invoking(m => m.GetRoleNameAsync(null!)).Should().ThrowAsync<ObjectDisposedException>();
            await manager.Invoking(m => m.SetRoleNameAsync(null!,null)).Should().ThrowAsync<ObjectDisposedException>();
            await manager.Invoking(m => m.UpdateAsync(null!)).Should().ThrowAsync<ObjectDisposedException>();
            await manager.Invoking(m => m.AddClaimAsync(null!, null!)).Should().ThrowAsync<ObjectDisposedException>();
            await manager.Invoking(m => m.GetClaimsAsync(null!)).Should().ThrowAsync<ObjectDisposedException>();
            await manager.Invoking(m => m.RemoveClaimAsync(null!, null!)).Should().ThrowAsync<ObjectDisposedException>();
        }


        #region Private methods

        private async Task<LiteDbRole> SetUpRoleAsync(RoleManager<LiteDbRole> manager)
        {
            var role = new LiteDbRole()
            {
                Name = "TestRole"
            };

            await manager.CreateAsync(role);

            return role;
        }

        #endregion
    }
}