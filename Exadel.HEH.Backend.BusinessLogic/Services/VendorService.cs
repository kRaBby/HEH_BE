﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Exadel.HEH.Backend.BusinessLogic.DTOs.Get;
using Exadel.HEH.Backend.BusinessLogic.Services.Abstract;
using Exadel.HEH.Backend.DataAccess.Models;
using Exadel.HEH.Backend.DataAccess.Repositories.Abstract;

namespace Exadel.HEH.Backend.BusinessLogic.Services
{
    public class VendorService : BaseService<Vendor, VendorDto>
    {
        public VendorService(IRepository<Vendor> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}