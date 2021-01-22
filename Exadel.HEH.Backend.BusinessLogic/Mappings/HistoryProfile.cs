﻿using AutoMapper;
using Exadel.HEH.Backend.BusinessLogic.DTOs.Create;
using Exadel.HEH.Backend.BusinessLogic.DTOs.Get;
using Exadel.HEH.Backend.DataAccess.Models;
using Exadel.HEH.Backend.Host.DTOs.Get;

namespace Exadel.HEH.Backend.BusinessLogic.Mappings
{
    public class HistoryProfile : Profile
    {
        public HistoryProfile()
        {
            CreateMap<History, HistoryDto>();

            CreateMap<HistoryCreateDto, History>();
        }
    }
}