﻿using System.Diagnostics.CodeAnalysis;
using Exadel.HEH.Backend.BusinessLogic.DTOs;
using Exadel.HEH.Backend.BusinessLogic.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Exadel.HEH.Backend.Host.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ValidatorsExtensions
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddTransient<IValidator<FavoritesShortDto>, FavoritesValidator>();

            services.AddTransient<IValidator<VendorDto>, VendorValidator>();

            services.AddTransient<IValidator<CategoryDto>, CategoryValidator>();

            services.AddTransient<IValidator<TagDto>, TagValidator>();

            services.AddTransient<IValidator<UserNotificationDto>, UserNotificationValidator>();

            return services;
        }
    }
}
