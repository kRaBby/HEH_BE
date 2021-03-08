﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Exadel.HEH.Backend.DataAccess.Repositories.Abstract;
using Exadel.HEH.Backend.Host.Identity.Store;
using IdentityServer4.Models;
using Moq;
using Xunit;

namespace Exadel.HEH.Backend.Host.Tests
{
    public class IdentityResourceTests
    {
        private readonly CustomResourceStore _store;
        private readonly IList<IdentityResource> _identityData;
        private readonly IList<ApiResource> _resourceData;
        private readonly IList<ApiScope> _scopeData;
        private readonly IdentityResource _identityResource;
        private readonly ApiResource _apiResource;
        private readonly ApiScope _apiScope;

        public IdentityResourceTests()
        {
            var repository = new Mock<IIdentityRepository>();
            _identityResource = new IdentityResource { Name = "Test identity name" };
            _identityData = new List<IdentityResource> { _identityResource };
            _apiResource = new ApiResource { Name = "Test resource name" };
            _resourceData = new List<ApiResource> { _apiResource };
            _apiScope = new ApiScope { Name = "Test scope name" };
            _scopeData = new List<ApiScope> { _apiScope };

            repository.Setup(r => r.GetAsync<IdentityResource>(It.IsAny<Expression<Func<IdentityResource, bool>>>()))
                .Returns(() => Task.FromResult(_identityData.Where(x => x.Name == _identityResource.Name)));
            repository.Setup(r => r.GetAsync<ApiResource>(It.IsAny<Expression<Func<ApiResource, bool>>>()))
                .Returns(() => Task.FromResult(_resourceData.Where(x => x.Name == _apiResource.Name)));
            repository.Setup(r => r.GetAsync<ApiScope>(It.IsAny<Expression<Func<ApiScope, bool>>>()))
                .Returns(() => Task.FromResult(_scopeData.Where(x => x.Name == _apiScope.Name)));

            _store = new CustomResourceStore(repository.Object);
        }

        [Fact]
        public async Task CanFindIdentityResourcesByScopeNameAsync()
        {
            var result = await _store.FindIdentityResourcesByScopeNameAsync(new List<string> { _identityResource.Name });
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task CanFindApiScopesByNameAsync()
        {
            var result = await _store.FindApiScopesByNameAsync(new List<string> { _apiScope.Name });
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task CanFindApiResourcesByScopeNameAsync()
        {
            var result = await _store.FindApiResourcesByScopeNameAsync(new List<string> { _apiResource.Name });
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task CanFindApiResourcesByNameAsync()
        {
            var result = await _store.FindApiResourcesByNameAsync(new List<string> { _identityResource.Name });
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task CanGetAllResourcesAsync()
        {
            var result = await _store.GetAllResourcesAsync();
            Assert.NotNull(result);
        }
    }
}