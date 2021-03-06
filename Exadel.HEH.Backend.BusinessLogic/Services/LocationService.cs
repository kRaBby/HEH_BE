﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Exadel.HEH.Backend.BusinessLogic.DTOs;
using Exadel.HEH.Backend.BusinessLogic.Services.Abstract;
using Exadel.HEH.Backend.DataAccess.Models;
using Exadel.HEH.Backend.DataAccess.Repositories.Abstract;

namespace Exadel.HEH.Backend.BusinessLogic.Services
{
    public class LocationService : BaseService<Location, LocationDto>, ILocationService
    {
        private readonly ILocationRepository _locationRepository;

        public LocationService(ILocationRepository locationRepository, IMapper mapper)
            : base(locationRepository, mapper)
        {
            _locationRepository = locationRepository;
        }

        public async Task<IEnumerable<LocationDto>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            var locations = await _locationRepository.GetByIdsAsync(ids);

            return Mapper.Map<IEnumerable<LocationDto>>(locations);
        }
    }
}
