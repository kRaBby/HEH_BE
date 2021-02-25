﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exadel.HEH.Backend.BusinessLogic.Providers;
using Exadel.HEH.Backend.BusinessLogic.ValidationServices;
using Exadel.HEH.Backend.DataAccess.Models;
using Exadel.HEH.Backend.DataAccess.Repositories.Abstract;
using Moq;
using Xunit;

namespace Exadel.HEH.Backend.BusinessLogic.Tests.ValidationServicesTests
{
    public class FavoritesValidationServiceTests
    {
        private readonly FavoritesValidationService _validationService;
        private readonly Discount _discount;

        public FavoritesValidationServiceTests()
        {
            var discountRepository = new Mock<IDiscountRepository>();
            var userRepository = new Mock<IUserRepository>();
            var userProvider = new Mock<IUserProvider>();
            var methodProvider = new Mock<IMethodProvider>();
            var discountData = new List<Discount>();
            var userFavorite = new Favorites();

            _discount = new Discount { Id = Guid.NewGuid() };
            userFavorite.DiscountId = _discount.Id;
            var currentUser = new User
            {
                Favorites = new List<Favorites>
                {
                    userFavorite
                }
            };
            discountData.Add(_discount);

            discountRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .Returns((Guid id) => Task.FromResult(discountData.FirstOrDefault(d => d.Id == id)));

            userRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .Returns((Guid id) => Task.FromResult(currentUser));

            methodProvider.Setup(p => p.GetMethodUpperName())
                .Returns(() => "PUT");

            _validationService = new FavoritesValidationService(discountRepository.Object, userRepository.Object,
                userProvider.Object, methodProvider.Object);
        }

        [Fact]
        public async Task CanValidateDiscountExists()
        {
            Assert.True(await _validationService.DiscountExists(_discount.Id, CancellationToken.None));
        }

        [Fact]
        public async Task CanValidateUserFavoritesNotExist()
        {
            Assert.True(await _validationService.UserFavoritesNotExists(_discount.Id, CancellationToken.None));
        }
    }
}