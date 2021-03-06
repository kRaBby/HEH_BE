﻿using AutoMapper;
using Exadel.HEH.Backend.BusinessLogic.Mappings;

namespace Exadel.HEH.Backend.BusinessLogic.Extensions
{
    public static class MapperExtensions
    {
        static MapperExtensions()
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new CategoryProfile());
                mc.AddProfile(new UserProfile());
                mc.AddProfile(new FavoritesProfile());
                mc.AddProfile(new HistoryProfile());
                mc.AddProfile(new VendorProfile());
                mc.AddProfile(new TagProfile());
                mc.AddProfile(new DiscountProfile());
                mc.AddProfile(new AddressProfile());
                mc.AddProfile(new PhoneProfile());
                mc.AddProfile(new LocationProfile());
                mc.AddProfile(new CityProfile());
                mc.AddProfile(new NotificationProfile());

                mc.AllowNullDestinationValues = false;
                mc.AllowNullCollections = false;
            });

            mapperConfig.AssertConfigurationIsValid();

            Mapper = mapperConfig.CreateMapper();
        }

        public static IMapper Mapper { get; }
    }
}