﻿using System;
using System.Collections.Generic;

namespace Exadel.HEH.Backend.BusinessLogic.DTOs
{
    public abstract class DiscountBaseDto
    {
        public Guid Id { get; set; }

        public string Conditions { get; set; }

        public IList<Guid> TagsIds { get; set; }

        public Guid VendorId { get; set; }

        public string VendorName { get; set; }

        public string PromoCode { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public Guid CategoryId { get; set; }
    }
}