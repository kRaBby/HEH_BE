﻿using System;

namespace Exadel.HEH.Backend.BusinessLogic.DTOs.Get
{
    public class AddressDto
    {
        public Guid Id { get; set; }

        public Guid CountryId { get; set; }

        public Guid CityId { get; set; }

        public string Street { get; set; }
    }
}