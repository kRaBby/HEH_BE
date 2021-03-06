﻿using System.Diagnostics.CodeAnalysis;
using Exadel.HEH.Backend.BusinessLogic.Services;
using Exadel.HEH.Backend.BusinessLogic.Services.Abstract;
using Exadel.HEH.Backend.DataAccess.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Exadel.HEH.Backend.Host.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class BusinessServicesExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services,
            IWebHostEnvironment env)
        {
            services.AddScoped<IStatisticsService, StatisticsService>();

            services.AddScoped<INotificationService, NotificationService>();

            services.AddSingleton<IEmailService, EmailService>();

            services.AddSingleton<ISmtpClientWrapper, SmtpClientWrapper>();

            services.AddSingleton<INotificationScheduler, NotificationScheduler>();

            if (env.IsDevelopment())
            {
                services.AddScoped<ISearchService<Discount, Discount>, LocalDiscountSearchService>();
                services.AddScoped<IVendorSearchService, LocalVendorSearchService>();
            }
            else
            {
                services.AddScoped<ISearchService<Discount, Discount>, LuceneDiscountSearchService>();
                services.AddScoped<IVendorSearchService, LuceneVendorSearchService>();
            }

            services.AddScoped<IExportService, ExcelExportService>();

            return services;
        }
    }
}