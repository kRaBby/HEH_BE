﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Exadel.HEH.Backend.BusinessLogic.DTOs;
using Exadel.HEH.Backend.BusinessLogic.Services.Abstract;
using Exadel.HEH.Backend.DataAccess.Models;
using Exadel.HEH.Backend.DataAccess.Repositories.Abstract;

namespace Exadel.HEH.Backend.BusinessLogic.Services
{
    public class VendorService : BaseService<Vendor, VendorShortDto>, IVendorService
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly IDiscountService _discountService;
        private readonly IMapper _mapper;
        private readonly IHistoryService _historyService;
        private readonly IVendorSearchService _searchService;
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;

        public VendorService(IVendorRepository vendorRepository,
            IDiscountService discountService,
            IMapper mapper,
            IHistoryService historyService,
            IVendorSearchService searchService,
            INotificationService notificationService,
            IUserService userService)
            : base(vendorRepository, mapper)
        {
            _vendorRepository = vendorRepository;
            _discountService = discountService;
            _mapper = mapper;
            _historyService = historyService;
            _searchService = searchService;
            _notificationService = notificationService;
            _userService = userService;
        }

        public async Task<IQueryable<VendorSearchDto>> GetAsync(string searchText)
        {
            var vendorsSearch = await (searchText != null ?
                _searchService.SearchAsync(searchText) : _searchService.SearchAsync());

            return _mapper.Map<IEnumerable<VendorSearchDto>>(vendorsSearch)
                .AsQueryable();
        }

        public async Task<IEnumerable<VendorShortDto>> GetAllFromLocationAsync()
        {
            var user = await _userService.GetProfileAsync();

            var vendors = await _vendorRepository.GetAllAsync();

            var result = new List<Vendor>();

            foreach (var vendor in vendors)
            {
                var countryCities = vendor.Addresses
                    .GroupBy(a => a.CountryId)
                    .Select(g =>
                        new KeyValuePair<Guid, IEnumerable<Guid?>>(
                            g.Key, g.Select(a => a.CityId).Where(i => i.HasValue)))
                    .ToDictionary(a => a.Key, a => a.Value);

                if (countryCities.ContainsKey(user.Address.CountryId) && (!countryCities[user.Address.CountryId].Any()
                                                                          || countryCities[user.Address.CountryId]
                                                                              .Contains(user.Address.CityId)))
                {
                    result.Add(vendor);
                }
            }

            return _mapper.Map<IEnumerable<VendorShortDto>>(result);
        }

        public async Task<VendorDto> GetByIdAsync(Guid id)
        {
           var vendor = await _vendorRepository.GetByIdAsync(id);
           var vendorDiscounts = (await _discountService.GetAllAsync())
               .Where(d => d.VendorId == id);

           return GetVendorDto(vendor, vendorDiscounts);
        }

        public async Task CreateAsync(VendorDto vendor)
        {
            vendor.Id = vendor.Id == Guid.Empty ? Guid.NewGuid() : vendor.Id;

            await _vendorRepository.CreateAsync(_mapper.Map<Vendor>(vendor));

            await _historyService.CreateAsync(UserAction.Add,
                "Created vendor " + vendor.Name);

            await _searchService.CreateAsync(vendor);

            await _notificationService.CreateVendorNotificationsAsync(vendor.Id);

            if (AnyDiscounts(vendor.Discounts))
            {
                InitVendorInfoInDiscounts(vendor);
                var discounts = GetDiscountsFromDto(vendor);

                await _discountService.CreateManyAsync(discounts);
            }
        }

        public async Task UpdateAsync(VendorDto vendor)
        {
            await _vendorRepository.UpdateAsync(_mapper.Map<Vendor>(vendor));
            await _historyService.CreateAsync(UserAction.Edit,
                "Updated vendor " + vendor.Name);

            await _searchService.UpdateAsync(vendor);

            var vendorDiscountsIds = new List<Guid>();

            var anyDiscounts = AnyDiscounts(vendor.Discounts);

            if (anyDiscounts)
            {
                vendorDiscountsIds = vendor.Discounts.Select(d => d.Id).ToList();
            }

            await _discountService.RemoveAsync(d =>
                d.VendorId == vendor.Id && !vendorDiscountsIds.Contains(d.Id));

            if (anyDiscounts)
            {
                InitVendorInfoInDiscounts(vendor);
                RemoveIncorrectDiscountPhones(vendor);

                var discounts = GetDiscountsFromDto(vendor);
                await _discountService.UpdateManyAsync(discounts);
            }
        }

        public async Task RemoveAsync(Guid id)
        {
            var vendor = await _vendorRepository.GetByIdAsync(id);

            await _vendorRepository.RemoveAsync(id);

            await _historyService.CreateAsync(UserAction.Remove,
                "Removed vendor " + vendor.Name);

            await _searchService.RemoveAsync(id);

            await _discountService.RemoveAsync(d => d.VendorId == id);

            await _notificationService.RemoveVendorNotificationsAsync(id);

            await _userService.RemoveVendorSubscriptionsAsync(id);
        }

        private VendorDto GetVendorDto(Vendor vendor, IEnumerable<DiscountShortDto> vendorDiscounts)
        {
            var vendorDto = _mapper.Map<VendorDto>(vendor);
            vendorDto.Discounts = vendorDiscounts;
            return vendorDto;
        }

        private void InitVendorInfoInDiscounts(VendorDto vendor)
        {
            vendor.Discounts.ToList().ForEach(d =>
            {
                d.VendorId = vendor.Id;
                d.VendorName = vendor.Name;
            });
        }

        private IEnumerable<Discount> GetDiscountsFromDto(VendorDto vendor)
        {
            var discounts = new List<Discount>();

            foreach (var vendorDiscount in vendor.Discounts)
            {
                var discount = _mapper.Map<Discount>(vendorDiscount);

                if (vendorDiscount.AddressesIds != null
                    && vendorDiscount.AddressesIds.Any())
                {
                    discount.Addresses = vendor.Addresses.Join(
                        vendorDiscount.AddressesIds,
                        a => a.Id,
                        i => i,
                        (a, i) => _mapper.Map<Address>(a)).ToList();
                }

                discounts.Add(discount);
            }

            return discounts;
        }

        private void RemoveIncorrectDiscountPhones(VendorDto vendor)
        {
            var vendorPhonesIds = vendor.Phones == null ? new List<int>()
                : vendor.Phones.Select(p => p.Id);

            var discounts = vendor.Discounts.ToList();
            discounts.ForEach(d =>
            {
                if (d.PhonesIds != null && d.PhonesIds.Any())
                {
                    var phoneList = d.PhonesIds.ToList();
                    phoneList.RemoveAll(p => !vendorPhonesIds.Contains(p));
                    d.PhonesIds = phoneList;
                }
            });
        }

        private bool AnyDiscounts(IEnumerable<DiscountShortDto> discounts)
        {
            return discounts != null && discounts.Any();
        }
    }
}