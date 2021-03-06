﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Exadel.HEH.Backend.BusinessLogic.DTOs;
using Exadel.HEH.Backend.BusinessLogic.Options;
using Exadel.HEH.Backend.BusinessLogic.Services.Abstract;
using Exadel.HEH.Backend.DataAccess.Models;
using Exadel.HEH.Backend.DataAccess.Repositories.Abstract;
using Microsoft.Extensions.Options;

namespace Exadel.HEH.Backend.BusinessLogic.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly IFavoritesService _favoritesService;
        private readonly ISearchService<Discount, Discount> _searchService;
        private readonly IHistoryService _historyService;
        private readonly IStatisticsService _statisticsService;

        private readonly IMapper _mapper;
        private readonly NotificationOptions _notificationOptions;
        private readonly INotificationService _notificationService;

        public DiscountService(IDiscountRepository discountRepository,
            IFavoritesService favoritesService,
            IVendorRepository vendorRepository,
            IMapper mapper,
            ISearchService<Discount, Discount> searchService,
            IHistoryService historyService,
            IStatisticsService statisticsService,
            IOptions<NotificationOptions> notificationOptions,
            INotificationService notificationService)
        {
            _discountRepository = discountRepository;
            _favoritesService = favoritesService;
            _vendorRepository = vendorRepository;
            _searchService = searchService;
            _historyService = historyService;
            _statisticsService = statisticsService;

            _mapper = mapper;
            _notificationOptions = notificationOptions.Value;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<DiscountShortDto>> GetAllAsync()
        {
            var discounts = await _discountRepository.GetAllAsync();
            var discountsDto = _mapper.Map<IEnumerable<DiscountShortDto>>(discounts);

            discountsDto = discountsDto.Join(
                discounts,
                dto => dto.Id,
                d => d.Id,
                (dto, d) =>
                {
                    if (d.Addresses != null)
                    {
                        dto.AddressesIds = d.Addresses.Select(a => a.Id).ToList();
                    }

                    return dto;
                });

            return discountsDto;
        }

        public async Task<IQueryable<DiscountDto>> GetAsync(string searchText)
        {
            var discounts = await _searchService.SearchAsync(searchText);

            var discountsDto = _mapper.Map<IEnumerable<DiscountDto>>(discounts);
            var discountsDtoList = discountsDto.ToList();

            var areInFavorites = await _favoritesService.DiscountsAreInFavorites(
                discountsDtoList.Select(d => d.Id));

            var discountsQueryable = discountsDtoList.Join(
                areInFavorites,
                d => d.Id,
                a => a.Key,
                (discount, a) =>
                {
                    discount.IsFavorite = a.Value;
                    return discount;
                }).AsQueryable();

            return discountsQueryable;
        }

        public async Task<DiscountExtendedDto> GetByIdAsync(Guid id)
        {
            var discount = await _discountRepository.GetByIdAsync(id);
            var vendor = await _vendorRepository.GetByIdAsync(discount.VendorId);

            var discountDto = _mapper.Map<DiscountExtendedDto>(discount);

            discountDto.IsFavorite = await _favoritesService.DiscountIsInFavorites(discountDto.Id);
            discountDto.Links = _mapper.Map<IEnumerable<LinkDto>>(vendor.Links);
            discountDto.WorkingHours = vendor.WorkingHours;

            discountDto.Phones = _mapper.Map<IEnumerable<PhoneDto>>(vendor.Phones.Join(
                discount.PhonesIds,
                p => p.Id,
                i => i,
                (p, i) => p));

            return discountDto;
        }

        public IQueryable<Discount> GetHot()
        {
            var nowDate = DateTime.UtcNow.Date;

            var supposedEndDate = nowDate.AddDays(nowDate.DayOfWeek == DayOfWeek.Friday ?
                    _notificationOptions.HotDiscountWeekendDaysLeft : _notificationOptions.HotDiscountDaysLeft);

            var hotDiscounts = _discountRepository.Get()
                .Where(d => d.EndDate >= nowDate && d.EndDate < supposedEndDate);

            return hotDiscounts;
        }

        public async Task CreateManyAsync(IEnumerable<Discount> discounts)
        {
            var discountsList = discounts.ToList();
            discountsList.ForEach(GenerateId);

            await _discountRepository.CreateManyAsync(discountsList);

            discountsList.ForEach(CreateHistorySearchNotificationItems);
        }

        public async Task UpdateManyAsync(IEnumerable<Discount> discounts)
        {
            var allDiscountsIds = _discountRepository.Get().Select(d => d.Id).ToList();

            var discountsList = discounts.ToList();

            discountsList.Where(d => !allDiscountsIds.Contains(d.Id))
                .ToList().ForEach(GenerateId);

            await _discountRepository.UpdateManyAsync(discountsList);

            foreach (var discount in discountsList)
            {
                if (allDiscountsIds.Contains(discount.Id))
                {
                    await _historyService.CreateAsync(UserAction.Edit,
                        "Updated discount " + discount.Conditions);

                    await _searchService.UpdateAsync(discount);
                }
                else
                {
                    CreateHistorySearchNotificationItems(discount);
                }
            }
        }

        public async Task RemoveAsync(Expression<Func<Discount, bool>> expression)
        {
            var discountsToRemove = _discountRepository.Get().Where(expression).ToList();

            discountsToRemove.ForEach(d =>
            {
                _searchService.RemoveAsync(d.Id);

                _historyService.CreateAsync(UserAction.Remove,
                    "Removed discount " + d.Conditions);

                _statisticsService.RemoveAsync(d.Id);

                _notificationService.RemoveDiscountNotificationsAsync(d.Id);
            });

            await _discountRepository.RemoveAsync(expression);

            var discountsToRemoveIds = discountsToRemove.Select(d => d.Id);
            await _favoritesService.RemoveManyAsync(discountsToRemoveIds);
        }

        private void CreateHistorySearchNotificationItems(Discount discount)
        {
            _historyService.CreateAsync(UserAction.Add,
                "Created discount " + discount.Conditions);

            _searchService.CreateAsync(discount);

            _notificationService.CreateDiscountNotificationsAsync(discount);
        }

        private void GenerateId(Discount discount)
        {
            discount.Id = discount.Id == Guid.Empty ?
                Guid.NewGuid() : discount.Id;
        }
    }
}