﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exadel.HEH.Backend.BusinessLogic.DTOs.Get;
using Exadel.HEH.Backend.BusinessLogic.Services.Abstract;
using Exadel.HEH.Backend.Host.Controllers;
using Moq;
using Xunit;

namespace Exadel.HEH.Backend.Host.Tests
{
    public class FavoritesControllerTests
    {
        private readonly FavoritesController _controller;
        private readonly List<FavoritesDto> _data;
        private FavoritesDto _favorites;

        public FavoritesControllerTests()
        {
            var service = new Mock<IFavoritesService>();
            _controller = new FavoritesController(service.Object);
            _data = new List<FavoritesDto>();
            InitTestData();

            service.Setup(s => s.GetAllAsync()).Returns(Task.FromResult((IEnumerable<FavoritesDto>)_data));

            service.Setup(s => s.CreateAsync(It.IsAny<FavoritesDto>()))
                .Callback((FavoritesDto item) =>
                {
                    _data.Add(item);
                })
                .Returns(Task.CompletedTask);

            service.Setup(s => s.UpdateAsync(It.IsAny<FavoritesDto>()))
                .Callback((FavoritesDto item) =>
                {
                    var oldItem = _data.Single();
                    if (oldItem != null)
                    {
                        _data.Remove(oldItem);
                        _data.Add(item);
                    }
                })
                .Returns(Task.CompletedTask);

            service.Setup(r => r.RemoveAsync(It.IsAny<Guid>()))
                .Callback((Guid id) => { _data.RemoveAt(0); })
                .Returns(Task.CompletedTask);
        }

        [Fact]
        public async Task CanGetAllAsync()
        {
            _data.Add(_favorites);
            var result = await _controller.GetAllAsync();
            Assert.Single(result);
        }

        [Fact]
        public async Task CanCreateAsync()
        {
            await _controller.CreateAsync(_favorites);
            Assert.Single(_data);
        }

        [Fact]
        public async Task CanUpdateAsync()
        {
            _data.Add(_favorites);
            var newFavorites = new FavoritesDto
            {
                Id = _favorites.Id,
                Note = "New note"
            };
            await _controller.UpdateAsync(newFavorites);
            Assert.NotEqual(_data.Single().Note, _favorites.Note);
        }

        [Fact]
        public async Task CanRemoveAsync()
        {
            _data.Add(_favorites);
            await _controller.RemoveAsync(_favorites.Id);
            Assert.Empty(_data);
        }

        private void InitTestData()
        {
            _favorites = new FavoritesDto
            {
                Id = Guid.NewGuid(),
                Note = "Note1",
                Addresses = new List<AddressDto>
                {
                    new AddressDto
                    {
                        CityId = Guid.NewGuid(),
                        CountryId = Guid.NewGuid(),
                        Street = "street"
                    }
                },
                Phones = new List<PhoneDto>
                {
                    new PhoneDto
                    {
                        Id = Guid.NewGuid(),
                        Number = "+375441111111"
                    }
                },
                CategoryId = Guid.NewGuid(),
                Conditions = "Conditions",
                TagsIds = new List<Guid> { Guid.NewGuid() },
                VendorId = Guid.NewGuid(),
                VendorName = "Vendor",
                PromoCode = "new promo code",
                StartDate = new DateTime(2021, 1, 20),
                EndDate = new DateTime(2021, 1, 25)
            };
        }
    }
}