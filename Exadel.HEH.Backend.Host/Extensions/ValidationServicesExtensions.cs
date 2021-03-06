﻿using System.Diagnostics.CodeAnalysis;
using Exadel.HEH.Backend.BusinessLogic.ValidationServices;
using Exadel.HEH.Backend.BusinessLogic.ValidationServices.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace Exadel.HEH.Backend.Host.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ValidationServicesExtensions
    {
        public static IServiceCollection AddValidationServices(this IServiceCollection services)
        {
            services.AddScoped<IFavoritesValidationService, FavoritesValidationService>();

            services.AddScoped<ICategoryValidationService, CategoryValidationService>();

            services.AddScoped<ITagValidationService, TagValidationService>();

            services.AddScoped<IVendorValidationService, VendorValidationService>();

            services.AddScoped<IDiscountValidationService, DiscountValidationService>();

            services.AddScoped<IUserValidationService, UserValidationService>();

            services.AddScoped<INotificationValidationService, NotificationValidationService>();

            return services;
        }
    }
}
