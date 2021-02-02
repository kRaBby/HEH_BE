﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exadel.HEH.Backend.DataAccess.Models;
using Exadel.HEH.Backend.DataAccess.Repositories.Abstract;

namespace Exadel.HEH.Backend.DataAccess.Repositories
{
    public class DiscountRepository : MongoRepository<Discount>, IDiscountRepository
    {
        public DiscountRepository(IDbContext context)
            : base(context)
        {
        }

        public IQueryable<Discount> Get()
        {
            return Context.GetAll<Discount>();
        }

        public Task<IEnumerable<Discount>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            return Context.GetAsync<Discount>(d => ids.Contains(d.Id));
        }
    }
}