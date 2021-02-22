﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Exadel.HEH.Backend.BusinessLogic.DTOs;
using Exadel.HEH.Backend.BusinessLogic.Providers;
using Exadel.HEH.Backend.BusinessLogic.Services.Abstract;
using Exadel.HEH.Backend.DataAccess.Models;
using Exadel.HEH.Backend.DataAccess.Repositories.Abstract;

namespace Exadel.HEH.Backend.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserProvider _userProvider;
        private readonly IHistoryService _historyService;
        private readonly IMapper _mapper;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            IUserProvider userProvider,
            IHistoryService historyService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userProvider = userProvider;
            _historyService = historyService;
        }

        public IQueryable<UserShortDto> Get()
        {
            var currentUserId = _userProvider.GetUserId();
            var users = _userRepository.Get().Where(u => u.Id != currentUserId);

            return users.ProjectTo<UserShortDto>(_mapper.ConfigurationProvider);
        }

        public async Task<UserShortDto> GetByIdAsync(Guid id)
        {
            var result = await _userRepository.GetByIdAsync(id);
            return _mapper.Map<UserShortDto>(result);
        }

        public async Task<UserDto> GetProfileAsync()
        {
            var result = await _userRepository.GetByIdAsync(_userProvider.GetUserId());
            return _mapper.Map<UserDto>(result);
        }

        public Task<IEnumerable<string>> GetEmailsForNotificationsAsync(
            IEnumerable<Guid> categoryIds,
            IEnumerable<Guid> tagIds,
            IEnumerable<Guid> vendorIds)
        {
            var allUsers = _userRepository.Get();
            var result = new List<string>();

            foreach (var user in allUsers)
            {
                if (user.CategoryNotifications.Intersect(categoryIds).Any()
                    || user.TagNotifications.Intersect(tagIds).Any()
                    || user.VendorNotifications.Intersect(vendorIds).Any())
                {
                    result.Add(user.Email);
                }
            }

            return result;
        }

        public async Task UpdateNotificationsAsync(NotificationDto notifications)
        {
            var userId = _userProvider.GetUserId();
            var user = await _userRepository.GetByIdAsync(userId);

            user.NewVendorNotificationIsOn = notifications.NewVendorNotificationIsOn;
            user.NewDiscountNotificationIsOn = notifications.NewDiscountNotificationIsOn;
            user.HotDiscountsNotificationIsOn = notifications.HotDiscountsNotificationIsOn;
            user.AllNotificationsAreOn = notifications.AllNotificationsAreOn;

            user.TagNotifications = notifications.TagNotifications;
            user.CategoryNotifications = notifications.CategoryNotifications;
            user.VendorNotifications = notifications.VendorNotifications;

            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdateStatusAsync(Guid id, bool isActive)
        {
            var user = await _userRepository.GetByIdAsync(id);
            user.IsActive = isActive;
            await _userRepository.UpdateAsync(user);
            await _historyService.CreateAsync(UserAction.Edit,
                "Updated user " + user.Name + " status");
        }

        public async Task UpdateRoleAsync(Guid id, UserRole role)
        {
            var user = await _userRepository.GetByIdAsync(id);
            user.Role = role;
            await _userRepository.UpdateAsync(user);
            await _historyService.CreateAsync(UserAction.Edit,
                "Updated user " + user.Name + " role");
        }
    }
}