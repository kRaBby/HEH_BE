﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exadel.HEH.Backend.DataAccess.Extensions;
using Exadel.HEH.Backend.DataAccess.Models;
using Exadel.HEH.Backend.DataAccess.Repositories;
using Xunit;

namespace Exadel.HEH.Backend.DataAccess.Tests
{
    public class DiscountRepositoryTests : BaseRepositoryTests<Discount>
    {
        private readonly DiscountRepository _repository;
        private Discount _discount;

        public DiscountRepositoryTests()
        {
            _repository = new DiscountRepository(Context.Object);
            InitTestData();
        }

        [Fact]
        public void CanGet()
        {
            Collection.Add(_discount);
            var result = _repository.Get();

            Assert.Single(result);
        }

        [Fact]
        public async Task CanGetAllAsync()
        {
            Collection.Add(_discount);
            var result = await _repository.GetAllAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task CanGetAsync()
        {
            Collection.Add(_discount);
            var result = await _repository.GetAsync(d => d.Id == _discount.Id);

            Assert.Single(result);
        }

        [Fact]
        public async Task CanGetByIdAsync()
        {
            Collection.Add(_discount);
            var result = await _repository.GetByIdAsync(_discount.Id);

            Assert.Equal(_discount, result);
        }

        [Fact]
        public async Task CanGetByIdsAsync()
        {
            Collection.Add(_discount);
            var result = await _repository.GetByIdsAsync(new List<Guid> { _discount.Id });

            Assert.Single(result);
        }

        [Fact]
        public async Task CanCreateManyAsync()
        {
            await _repository.CreateManyAsync(new List<Discount> { _discount });
            var discount = Collection.FirstOrDefault(x => x.Id == _discount.Id);

            Assert.NotNull(discount);
        }

        [Fact]
        public async Task CanUpdateAsync()
        {
            Collection.Add(_discount.DeepClone());

            _discount.PromoCode = "new promo code";
            await _repository.UpdateAsync(_discount);

            Assert.True(Collection.Single(x => x.Id == _discount.Id).PromoCode.Equals("new promo code"));
        }

        [Fact]
        public async Task CanUpdateManyAsync()
        {
            Collection.Add(_discount.DeepClone());

            _discount.PromoCode = "new promo code";
            await _repository.UpdateManyAsync(new List<Discount> { _discount });

            Assert.True(Collection.Single(x => x.Id == _discount.Id).PromoCode.Equals("new promo code"));
        }

        [Fact]
        public async Task CanRemoveAsync()
        {
            Collection.Add(_discount);

            await _repository.RemoveAsync(d => d.VendorId == _discount.VendorId);

            Assert.Empty(Collection);
        }

        private void InitTestData()
        {
            _discount = new Discount
            {
                Id = Guid.NewGuid(),
                PhonesIds = new List<int>
                {
                    2
                },
                CategoryId = Guid.NewGuid(),
                Conditions = "Conditions",
                TagsIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                VendorId = Guid.NewGuid(),
                VendorName = "Vendor",
                PromoCode = "new promo code",
                StartDate = new DateTime(2021, 1, 20),
                EndDate = new DateTime(2021, 1, 25)
            };
        }
    }
}