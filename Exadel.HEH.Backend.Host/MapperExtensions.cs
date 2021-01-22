﻿using AutoMapper;
using Exadel.HEH.Backend.Host.Mappings;

namespace Exadel.HEH.Backend.Host
{
    public static class MapperExtensions
    {
        public static IMapper GetMapper()
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new UserProfile());
                mc.AddProfile(new HistoryProfile());
            });

            return mapperConfig.CreateMapper();
        }
    }
}